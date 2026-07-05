using System.Net.Http.Json;
using System.Text.Json;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.GetDataFromServer;

public sealed class UserProfileDataApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public UserProfileDataApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<UserProfileDataResult> SaveAsync(UserInfo? userInfo)
    {
        if (userInfo is null || userInfo.Id <= 0)
            return UserProfileDataResult.Ok("UserId yoxdur. Profile data SQL-ə göndərilmədi.");

        try
        {
            var request = BuildSaveRequest(userInfo);
            using var response = await _httpClient.PostAsJsonAsync("api/UserProfileData/save", request, JsonOptions);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return UserProfileDataResult.Fail(ExtractMessage(body, response.ReasonPhrase ?? "Profile data save alınmadı."));

            var data = Deserialize<UserProfileDataResponse>(body);
            return UserProfileDataResult.Ok(data?.Message ?? "Profile data SQL-də saxlandı.", data);
        }
        catch (Exception ex)
        {
            return UserProfileDataResult.Fail(ex.Message);
        }
    }

    public async Task<UserProfileDataResult> LoadIntoUserAsync(UserInfo? userInfo)
    {
        if (userInfo is null || userInfo.Id <= 0)
            return UserProfileDataResult.Ok("UserId yoxdur. Profile data yüklənmədi.");

        try
        {
            using var response = await _httpClient.GetAsync($"api/UserProfileData/{userInfo.Id}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return UserProfileDataResult.Fail(ExtractMessage(body, response.ReasonPhrase ?? "Profile data load alınmadı."));

            var data = Deserialize<UserProfileDataResponse>(body);
            if (data is null)
                return UserProfileDataResult.Fail("Profile data cavabı oxunmadı.");

            ApplyToUser(userInfo, data);
            return UserProfileDataResult.Ok(data.Message, data);
        }
        catch (Exception ex)
        {
            return UserProfileDataResult.Fail(ex.Message);
        }
    }

    private static SaveUserProfileDataRequest BuildSaveRequest(UserInfo userInfo)
    {
        return new SaveUserProfileDataRequest
        {
            UserId = userInfo.Id,
            Skills = userInfo.SelectedSkills
                .Where(x => !string.IsNullOrWhiteSpace(x.SkillName))
                .GroupBy(x => NormalizeKey(x.SkillName))
                .Select(x => ToDto(x.First()))
                .ToList(),
            Experiences = userInfo.WorkExperiences
                .Where(x => !string.IsNullOrWhiteSpace(x.CompanyName))
                .Select(ToDto)
                .ToList()
        };
    }

    private static void ApplyToUser(UserInfo userInfo, UserProfileDataResponse response)
    {
        userInfo.SelectedSkills.Clear();
        foreach (var skill in response.Skills)
            userInfo.SelectedSkills.Add(ToModel(skill));

        userInfo.WorkExperiences.Clear();
        foreach (var experience in response.Experiences)
            userInfo.WorkExperiences.Add(ToModel(experience));
    }

    private static UserSkillProfileDto ToDto(UserSkillInfo skill)
    {
        return new UserSkillProfileDto
        {
            SkillId = skill.SkillId,
            SkillName = skill.SkillName,
            PositionId = skill.PositionId,
            PositionName = skill.PositionName,
            SeniorityId = skill.SeniorityId,
            SeniorityName = skill.SeniorityName,
            JobFamilyId = skill.JobFamilyId,
            JobFamilyName = skill.JobFamilyName,
            SkillComplexity = string.IsNullOrWhiteSpace(skill.SkillComplexity) ? "medium" : skill.SkillComplexity,
            Status = string.IsNullOrWhiteSpace(skill.Status) ? "self_declared" : skill.Status,
            IsVerified = skill.IsVerified,
            KnowledgeScore = skill.KnowledgeScore,
            ExperienceScore = skill.ExperienceScore,
            DepthScore = skill.DepthScore,
            CredibilityScore = skill.CredibilityScore,
            TaskComplexity = skill.TaskComplexity,
            OwnershipLevel = skill.OwnershipLevel,
            DepthTier = skill.DepthTier,
            ContextScore = skill.ContextScore,
            ComplexityScore = skill.ComplexityScore,
            OwnershipScore = skill.OwnershipScore,
            ResultScore = skill.ResultScore
        };
    }

    private static UserWorkExperienceProfileDto ToDto(UserWorkExperienceInfo experience)
    {
        return new UserWorkExperienceProfileDto
        {
            CompanyName = experience.CompanyName,
            PositionName = experience.PositionName,
            StartYear = experience.StartYear,
            EndYear = experience.EndYear,
            FileName = experience.FileName
        };
    }

    private static UserSkillInfo ToModel(UserSkillProfileDto skill)
    {
        return new UserSkillInfo
        {
            SkillId = skill.SkillId,
            SkillName = skill.SkillName,
            PositionId = skill.PositionId,
            PositionName = skill.PositionName,
            SeniorityId = skill.SeniorityId,
            SeniorityName = skill.SeniorityName,
            JobFamilyId = skill.JobFamilyId,
            JobFamilyName = skill.JobFamilyName,
            SkillComplexity = string.IsNullOrWhiteSpace(skill.SkillComplexity) ? "medium" : skill.SkillComplexity,
            Status = string.IsNullOrWhiteSpace(skill.Status) ? "self_declared" : skill.Status,
            IsVerified = skill.IsVerified,
            KnowledgeScore = skill.KnowledgeScore,
            ExperienceScore = skill.ExperienceScore,
            DepthScore = skill.DepthScore,
            CredibilityScore = skill.CredibilityScore,
            TaskComplexity = skill.TaskComplexity,
            OwnershipLevel = skill.OwnershipLevel,
            DepthTier = skill.DepthTier,
            ContextScore = skill.ContextScore,
            ComplexityScore = skill.ComplexityScore,
            OwnershipScore = skill.OwnershipScore,
            ResultScore = skill.ResultScore
        };
    }

    private static UserWorkExperienceInfo ToModel(UserWorkExperienceProfileDto experience)
    {
        return new UserWorkExperienceInfo
        {
            CompanyName = experience.CompanyName,
            PositionName = experience.PositionName,
            StartYear = experience.StartYear,
            EndYear = experience.EndYear,
            FileName = experience.FileName
        };
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static T? Deserialize<T>(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(body, JsonOptions);
        }
        catch
        {
            return default;
        }
    }

    private static string ExtractMessage(string? body, string fallback)
    {
        if (string.IsNullOrWhiteSpace(body))
            return fallback;

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("message", out var message))
                return message.GetString() ?? fallback;
            if (doc.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? fallback;
        }
        catch
        {
            return body.Trim();
        }

        return fallback;
    }
}

public sealed class UserProfileDataResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserProfileDataResponse? Data { get; set; }

    public static UserProfileDataResult Ok(string message, UserProfileDataResponse? data = null)
    {
        return new UserProfileDataResult { Success = true, Message = message, Data = data };
    }

    public static UserProfileDataResult Fail(string message)
    {
        return new UserProfileDataResult { Success = false, Message = message };
    }
}

public sealed class SaveUserProfileDataRequest
{
    public int UserId { get; set; }
    public List<UserSkillProfileDto> Skills { get; set; } = new();
    public List<UserWorkExperienceProfileDto> Experiences { get; set; } = new();
}

public sealed class UserProfileDataResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public List<UserSkillProfileDto> Skills { get; set; } = new();
    public List<UserWorkExperienceProfileDto> Experiences { get; set; } = new();
}

public sealed class UserSkillProfileDto
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public int SeniorityId { get; set; }
    public string SeniorityName { get; set; } = string.Empty;
    public int JobFamilyId { get; set; }
    public string JobFamilyName { get; set; } = string.Empty;
    public string SkillComplexity { get; set; } = "medium";
    public string Status { get; set; } = "self_declared";
    public bool IsVerified { get; set; }
    public double KnowledgeScore { get; set; }
    public double ExperienceScore { get; set; }
    public double DepthScore { get; set; }
    public double CredibilityScore { get; set; }
    public string TaskComplexity { get; set; } = string.Empty;
    public string OwnershipLevel { get; set; } = string.Empty;
    public string DepthTier { get; set; } = string.Empty;
    public double ContextScore { get; set; }
    public double ComplexityScore { get; set; }
    public double OwnershipScore { get; set; }
    public double ResultScore { get; set; }
}

public sealed class UserWorkExperienceProfileDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string StartYear { get; set; } = string.Empty;
    public string EndYear { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
