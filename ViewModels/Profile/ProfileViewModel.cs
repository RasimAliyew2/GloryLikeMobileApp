using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;

namespace MetanetA_MobileApp.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserSession _userSession;

    public ObservableCollection<ProfileSkillCardItem> Skills { get; } = new();
    public ObservableCollection<ProfileExperienceCardItem> Experiences { get; } = new();

    [ObservableProperty] private string fullName = "User";
    [ObservableProperty] private string initials = "U";
    [ObservableProperty] private string roleTitle = "Open to work";
    [ObservableProperty] private string profileCompletionText = "Profile: 0%";
    [ObservableProperty] private double profileCompletionProgress;
    [ObservableProperty] private string matchScoreText = "--";
    [ObservableProperty] private string trustScoreText = "--";
    [ObservableProperty] private string verificationLevel = "Basic";
    [ObservableProperty] private string strengthScoreText = "--";
    [ObservableProperty] private string strongestRoleText = "✧ Strongest role: add skills to calculate profile strength.";
    [ObservableProperty] private bool hasNoSkills = true;
    [ObservableProperty] private bool hasNoExperiences = true;

    public ProfileViewModel(IUserSession userSession,BottomMenuState bottomMenuState) : base(bottomMenuState)
    {
        _userSession = userSession;
        Refresh();
    }

    public void Refresh()
    {
        var user = _userSession.CurrentUser;
        if (user is null)
        {
            ApplyEmptyState();
            return;
        }

        FullName = BuildFullName(user);
        Initials = BuildInitials(FullName);
        RoleTitle = BuildRoleTitle(user);

        LoadSkills(user);
        LoadExperiences(user);
        CalculateReadiness();
        CalculateProfileCompletion();
    }

    private void ApplyEmptyState()
    {
        FullName = "User";
        Initials = "U";
        RoleTitle = "Open to work";
        Skills.Clear();
        Experiences.Clear();
        HasNoSkills = true;
        HasNoExperiences = true;
        MatchScoreText = "--";
        TrustScoreText = "--";
        VerificationLevel = "Basic";
        StrengthScoreText = "--";
        StrongestRoleText = "✧ Strongest role: add skills to calculate profile strength.";
        ProfileCompletionText = "Profile: 0%";
        ProfileCompletionProgress = 0;
    }

    private void LoadSkills(object user)
    {
        Skills.Clear();

        foreach (var skill in GetEnumerableProperty(user, "SelectedSkills"))
        {
            var skillName = FirstText(skill, "SkillName", "Name", "Title");
            if (string.IsNullOrWhiteSpace(skillName))
                continue;

            var knowledge = FirstNumber(skill, "KnowledgeScore", "Knowledge");
            var experience = FirstNumber(skill, "ExperienceScore", "Experience");
            var score = FirstNumber(skill, "CredibilityScore", "Credibility", "Score", "DepthScore");

            if (score <= 0 && (knowledge > 0 || experience > 0))
                score = (knowledge * 0.45d) + (experience * 0.55d);

            var category = FirstText(skill, "JobFamilyName", "PositionName", "SeniorityName", "SkillComplexity");
            if (string.IsNullOrWhiteSpace(category))
                category = "Skill";

            var isVerified = FirstBool(skill, "IsVerified");
            var status = FirstText(skill, "Status").Trim().ToLowerInvariant();
            var badge = isVerified || status == "verified"
                ? "🛡 Verified"
                : status == "confirmed"
                    ? "✓ Confirmed"
                    : status == "in_review"
                        ? "◷ In review"
                        : "Self-declared";

            Skills.Add(new ProfileSkillCardItem
            {
                SkillName = skillName,
                Category = category,
                Score = Clamp(score),
                Knowledge = Clamp(knowledge),
                Experience = Clamp(experience),
                BadgeText = badge
            });
        }

        HasNoSkills = Skills.Count == 0;
    }

    private void LoadExperiences(object user)
    {
        Experiences.Clear();

        foreach (var experience in GetEnumerableProperty(user, "WorkExperiences", "Experiences"))
        {
            var companyName = FirstText(experience, "CompanyName", "Company", "EmployerName");
            var positionName = FirstText(experience, "PositionName", "Position", "JobTitle", "Title");
            var startYear = FirstText(experience, "StartYear", "From", "StartDate");
            var endYear = FirstText(experience, "EndYear", "Ending", "EndDate");

            if (string.IsNullOrWhiteSpace(companyName) && string.IsNullOrWhiteSpace(positionName))
                continue;

            Experiences.Add(new ProfileExperienceCardItem
            {
                CompanyName = string.IsNullOrWhiteSpace(companyName) ? "Company" : companyName,
                PositionName = string.IsNullOrWhiteSpace(positionName) ? "Position" : positionName,
                StartYear = startYear,
                EndYear = endYear
            });
        }

        HasNoExperiences = Experiences.Count == 0;
    }

    private void CalculateReadiness()
    {
        if (Skills.Count == 0)
        {
            MatchScoreText = "--";
            TrustScoreText = "--";
            VerificationLevel = "Basic";
            StrengthScoreText = "--";
            StrongestRoleText = "✧ Strongest role: add skills to calculate profile strength.";
            return;
        }

        var averageScore = Skills.Average(x => x.Score);
        var averageKnowledge = Skills.Average(x => x.Knowledge);
        var averageExperience = Skills.Average(x => x.Experience);
        var strength = (averageKnowledge * 0.45d) + (averageExperience * 0.55d);
        var verifiedCount = Skills.Count(x => x.BadgeText.Contains("Verified", StringComparison.OrdinalIgnoreCase));

        MatchScoreText = RoundHalfUp(averageScore).ToString();
        TrustScoreText = RoundHalfUp(averageScore).ToString();
        StrengthScoreText = RoundHalfUp(strength).ToString() + "%";
        VerificationLevel = verifiedCount > 0 ? "Verified" : "Basic";
        StrongestRoleText = $"✧ Strongest role: {RoleTitle}. Tap to see all target roles.";
    }

    private void CalculateProfileCompletion()
    {
        var score = 35;

        if (FullName != "User") score += 15;
        if (!string.IsNullOrWhiteSpace(RoleTitle) && RoleTitle != "Open to work") score += 10;
        if (Skills.Count > 0) score += Math.Min(25, Skills.Count * 8);
        if (Experiences.Count > 0) score += Math.Min(15, Experiences.Count * 8);

        score = Math.Clamp(score, 0, 100);
        ProfileCompletionText = $"Profile: {score}%";
        ProfileCompletionProgress = score / 100d;
    }

    private static string BuildFullName(object user)
    {
        var name = FirstText(user, "Name", "FirstName");
        var surname = FirstText(user, "Surname", "LastName");
        var username = FirstText(user, "UserName", "Username");

        var fullName = $"{name} {surname}".Trim();
        return !string.IsNullOrWhiteSpace(fullName)
            ? fullName
            : !string.IsNullOrWhiteSpace(username)
                ? username
                : "User";
    }

    private static string BuildRoleTitle(object user)
    {
        var job = GetPropertyValue(user, "Job");
        if (job is null)
            return "Open to work";

        if (job is string jobText)
            return string.IsNullOrWhiteSpace(jobText) ? "Open to work" : jobText;

        var jobName = FirstText(job, "JobName", "Name", "Title");
        return string.IsNullOrWhiteSpace(jobName) ? "Open to work" : jobName;
    }

    private static string BuildInitials(string fullName)
    {
        var parts = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .ToList();

        if (parts.Count == 0)
            return "U";

        return string.Concat(parts.Select(x => char.ToUpperInvariant(x[0])));
    }

    private static IEnumerable<object> GetEnumerableProperty(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var value = GetPropertyValue(source, propertyName);
            if (value is null || value is string)
                continue;

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is not null)
                        yield return item;
                }

                yield break;
            }
        }
    }

    private static object? GetPropertyValue(object source, string propertyName)
    {
        return source.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
            ?.GetValue(source);
    }

    private static string FirstText(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var value = GetPropertyValue(source, propertyName)?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return string.Empty;
    }

    private static double FirstNumber(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var value = GetPropertyValue(source, propertyName);
            if (value is null)
                continue;

            if (value is double d) return d;
            if (value is float f) return f;
            if (value is decimal m) return (double)m;
            if (value is int i) return i;
            if (value is long l) return l;

            if (double.TryParse(value.ToString(), out var parsed))
                return parsed;
        }

        return 0;
    }

    private static bool FirstBool(object source, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            var value = GetPropertyValue(source, propertyName);
            if (value is bool b)
                return b;

            if (bool.TryParse(value?.ToString(), out var parsed))
                return parsed;
        }

        return false;
    }

    private static double Clamp(double value) => Math.Clamp(value, 0, 100);

    private static int RoundHalfUp(double value) => (int)Math.Floor(value + 0.5d);
}

public sealed class ProfileSkillCardItem
{
    public string SkillName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;
    public double Score { get; set; }
    public double Knowledge { get; set; }
    public double Experience { get; set; }
    public string ScoreText => RoundHalfUp(Score).ToString();
    public string KnowledgeText => RoundHalfUp(Knowledge).ToString();
    public string ExperienceText => RoundHalfUp(Experience).ToString();
    public double KnowledgeProgress => Clamp(Knowledge) / 100d;
    public double ExperienceProgress => Clamp(Experience) / 100d;
    private static double Clamp(double value) => Math.Clamp(value, 0, 100);
    private static int RoundHalfUp(double value) => (int)Math.Floor(value + 0.5d);
}

public sealed class ProfileExperienceCardItem
{
    public string CompanyName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string StartYear { get; set; } = string.Empty;
    public string EndYear { get; set; } = string.Empty;

    public string PeriodText
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(StartYear) && !string.IsNullOrWhiteSpace(EndYear))
                return $"{StartYear} - {EndYear}";

            if (!string.IsNullOrWhiteSpace(StartYear))
                return $"From {StartYear}";

            if (!string.IsNullOrWhiteSpace(EndYear))
                return $"Until {EndYear}";

            return "Experience period not specified";
        }
    }
}
