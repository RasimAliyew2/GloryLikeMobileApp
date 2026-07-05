using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Profile;

public partial class SkillsViewModel : BaseViewModel
{
    private readonly ISkillAndJobApiService _skillAndJobApiService;
    private readonly SkillVerificationState? _skillVerificationState;
    private readonly IUserSession _userSession;
    private readonly UserProfileDataApiService? _profileDataApiService;
    private bool _isLoaded;

    public SkillsViewModel(
        BottomMenuState menuState,
        ISkillAndJobApiService skillAndJobApiService,
        IUserSession userSession,
        UserInfo userInfo,
        SkillVerificationState? skillVerificationState = null,
        HttpClient? httpClient = null) : base(menuState)
    {
        _skillAndJobApiService = skillAndJobApiService;
        _skillVerificationState = skillVerificationState;
        _userSession = userSession;
        _profileDataApiService = httpClient is null ? null : new UserProfileDataApiService(httpClient);

        UserInfo = userInfo;
        _userSession.CurrentUser ??= UserInfo;

        MenuState.Select(BottomTab.Profile);

        CompanyOptions.Add(new CompanyChip("Azercell Telecom", true));
        CompanyOptions.Add(new CompanyChip("Umico", false));
    }

    public ObservableCollection<AvailableSkillItem> AvailableSkills { get; } = new();
    public ObservableCollection<CompanyChip> CompanyOptions { get; } = new();
    public ObservableCollection<ProfileSkillItem> AddedSkills { get; } = new();
    public ObservableCollection<ProfileExperienceItem> AddedExperiences { get; } = new();

    [ObservableProperty] private UserInfo userInfo;
    [ObservableProperty] private AvailableSkillItem? selectedAvailableSkill;
    [ObservableProperty] private bool isAddSkillVisible;
    [ObservableProperty] private bool isAddExperienceVisible;
    [ObservableProperty] private string experienceCompanyName = string.Empty;
    [ObservableProperty] private string experiencePositionName = string.Empty;
    [ObservableProperty] private string experienceStartYear = string.Empty;
    [ObservableProperty] private string experienceEndYear = "Present";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasAddedSkills => AddedSkills.Count > 0;
    public bool HasNoSkills => AddedSkills.Count == 0;
    public bool HasAddedExperiences => AddedExperiences.Count > 0;
    public bool HasNoExperiences => AddedExperiences.Count == 0;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var currentUser = GetCurrentUser();

            // App yenidən açılıb sign in olunubsa, SQL-də saxlanmış skills/experience-ləri çəkirik.
            if (_profileDataApiService is not null && currentUser.Id > 0)
                await _profileDataApiService.LoadIntoUserAsync(currentUser);

            LoadAvailableSkillsFromSelectedJob();
            LoadSkillsSelectedDuringSignup();
            LoadExperiencesFromSession();

            _isLoaded = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowAddSkill()
    {
        IsAddSkillVisible = true;
        IsAddExperienceVisible = false;

        if (SelectedAvailableSkill is null && AvailableSkills.Count > 0)
            SelectedAvailableSkill = AvailableSkills.First();
    }

    [RelayCommand]
    private void CancelAddSkill()
    {
        IsAddSkillVisible = false;
        SelectedAvailableSkill = null;

        foreach (var company in CompanyOptions)
            company.IsSelected = false;

        if (CompanyOptions.Count > 0)
            CompanyOptions[0].IsSelected = true;
    }

    [RelayCommand]
    private void ToggleCompany(CompanyChip? company)
    {
        if (company is null)
            return;

        company.IsSelected = !company.IsSelected;
    }

    [RelayCommand]
    private async Task AddSelectedSkillAsync()
    {
        if (SelectedAvailableSkill is null)
        {
            await Shell.Current.DisplayAlert("Skill", "Əlavə etmək üçün skill seç.", "OK");
            return;
        }

        var alreadyExists = AddedSkills.Any(x =>
            x.SkillId == SelectedAvailableSkill.SkillId ||
            x.SkillName.Equals(SelectedAvailableSkill.SkillName, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            await Shell.Current.DisplayAlert("Skill", "Bu skill artıq əlavə olunub.", "OK");
            return;
        }

        var selectedCompanies = CompanyOptions
            .Where(x => x.IsSelected)
            .Select(x => x.Name)
            .ToList();

        if (selectedCompanies.Count == 0)
            selectedCompanies.Add("Not linked to experience");

        var profileSkill = new ProfileSkillItem
        {
            SkillId = SelectedAvailableSkill.SkillId,
            SkillName = SelectedAvailableSkill.SkillName,
            PositionId = SelectedAvailableSkill.PositionId,
            PositionName = SelectedAvailableSkill.PositionName,
            SeniorityName = SelectedAvailableSkill.SeniorityName,
            JobFamilyName = SelectedAvailableSkill.JobFamilyName,
            SkillComplexity = SelectedAvailableSkill.SkillComplexity,
            UsedIn = selectedCompanies
        };

        AddedSkills.Add(profileSkill);
        SaveSkillToSession(profileSkill);
        await SaveProfileDataQuietlyAsync();

        IsAddSkillVisible = false;
        SelectedAvailableSkill = null;
        RefreshSkillState();
    }

    [RelayCommand]
    private void RemoveSkill(ProfileSkillItem? item)
    {
        if (item is null)
            return;

        AddedSkills.Remove(item);
        RemoveSkillFromSession(item);
        _ = SaveProfileDataQuietlyAsync();
        RefreshSkillState();
    }

    [RelayCommand]
    private async Task AssessDepthAsync(ProfileSkillItem? item)
    {
        if (item is null)
            return;

        if (_skillVerificationState is not null)
        {
            var skill = new Skill
            {
                Id = item.SkillId,
                SkillName = item.SkillName,
                PositionId = item.PositionId,
                SkillComplexity = item.SkillComplexity
            };

            _skillVerificationState.SetSelectedSkills(
                new[] { skill },
                item.SeniorityName,
                language: "az");
        }

        await Shell.Current.GoToAsync(nameof(VerifySkillsPage));
    }

    [RelayCommand]
    private void ShowAddExperience()
    {
        IsAddExperienceVisible = true;
        IsAddSkillVisible = false;
    }

    [RelayCommand]
    private void CancelAddExperience()
    {
        IsAddExperienceVisible = false;
        ClearExperienceForm();
    }

    [RelayCommand]
    private async Task UploadExperienceFileAsync()
    {
        await Shell.Current.DisplayAlert(
            "Upload File",
            "PDF / XML / JSON upload flow hələ backend parser-ə qoşulmayıb.",
            "OK");
    }

    [RelayCommand]
    private async Task AddExperienceAsync()
    {
        var company = ExperienceCompanyName.Trim();
        var position = ExperiencePositionName.Trim();
        var start = ExperienceStartYear.Trim();
        var end = ExperienceEndYear.Trim();

        if (string.IsNullOrWhiteSpace(company))
        {
            await Shell.Current.DisplayAlert("Experience", "Company name boş ola bilməz.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(position))
        {
            await Shell.Current.DisplayAlert("Experience", "Position boş ola bilməz.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(start))
        {
            await Shell.Current.DisplayAlert("Experience", "Başlama ili boş ola bilməz.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(end))
            end = "Present";

        var item = new ProfileExperienceItem
        {
            CompanyName = company,
            PositionName = position,
            StartYear = start,
            EndYear = end
        };

        AddedExperiences.Add(item);
        SaveExperienceToSession(item);
        AddCompanyOptionIfMissing(company);
        await SaveProfileDataQuietlyAsync();

        IsAddExperienceVisible = false;
        ClearExperienceForm();
        RefreshExperienceState();
    }

    [RelayCommand]
    private void RemoveExperience(ProfileExperienceItem? item)
    {
        if (item is null)
            return;

        AddedExperiences.Remove(item);
        RemoveExperienceFromSession(item);
        _ = SaveProfileDataQuietlyAsync();
        RefreshExperienceState();
    }

    [RelayCommand]
    private async Task EditProfileAsync()
    {
        await Shell.Current.DisplayAlert("Edit profile", "Edit profile clicked", "OK");
    }

    private void LoadAvailableSkillsFromSelectedJob()
    {
        AvailableSkills.Clear();

        var currentUser = GetCurrentUser();
        var selectedJob = currentUser.Job;

        if (selectedJob is null)
            return;

        var allSkills = selectedJob.Seniorities
            .SelectMany(seniority => seniority.Positions.Select(position => new { seniority, position }))
            .SelectMany(x => x.position.Skills.Select(skill => new AvailableSkillItem
            {
                SkillId = skill.Id,
                SkillName = skill.SkillName,
                PositionId = skill.PositionId,
                PositionName = x.position.Name,
                SeniorityName = x.seniority.Name,
                JobFamilyName = selectedJob.JobName,
                SkillComplexity = string.IsNullOrWhiteSpace(skill.SkillComplexity)
                    ? "medium"
                    : skill.SkillComplexity.Trim().ToLowerInvariant()
            }))
            .Where(x => !string.IsNullOrWhiteSpace(x.SkillName))
            .GroupBy(x => x.SkillName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(x => x.SkillName)
            .ToList();

        foreach (var skill in allSkills)
            AvailableSkills.Add(skill);
    }

    private void LoadSkillsSelectedDuringSignup()
    {
        AddedSkills.Clear();

        var currentUser = GetCurrentUser();

        foreach (var selectedSkill in currentUser.SelectedSkills)
        {
            if (AddedSkills.Any(x => x.SkillId == selectedSkill.SkillId && x.SkillId > 0))
                continue;

            if (AddedSkills.Any(x => x.SkillName.Equals(selectedSkill.SkillName, StringComparison.OrdinalIgnoreCase)))
                continue;

            AddedSkills.Add(new ProfileSkillItem
            {
                SkillId = selectedSkill.SkillId,
                SkillName = selectedSkill.SkillName,
                PositionId = selectedSkill.PositionId,
                PositionName = selectedSkill.PositionName,
                SeniorityName = selectedSkill.SeniorityName,
                JobFamilyName = selectedSkill.JobFamilyName,
                SkillComplexity = selectedSkill.SkillComplexity,
                Knowledge = selectedSkill.KnowledgeScore,
                Experience = selectedSkill.ExperienceScore,
                Depth = selectedSkill.Depth,
                Credibility = selectedSkill.Credibility,
                UsedIn = new List<string> { "Selected during sign up" }
            });
        }

        RefreshSkillState();
    }

    private void LoadExperiencesFromSession()
    {
        AddedExperiences.Clear();

        var currentUser = GetCurrentUser();

        foreach (var experience in currentUser.WorkExperiences)
        {
            AddedExperiences.Add(new ProfileExperienceItem
            {
                CompanyName = experience.CompanyName,
                PositionName = experience.PositionName,
                StartYear = experience.StartYear,
                EndYear = experience.EndYear
            });

            AddCompanyOptionIfMissing(experience.CompanyName);
        }

        RefreshExperienceState();
    }

    private void SaveSkillToSession(ProfileSkillItem item)
    {
        var currentUser = GetCurrentUser();

        var existing = currentUser.SelectedSkills.FirstOrDefault(x =>
            (x.SkillId > 0 && x.SkillId == item.SkillId) ||
            x.SkillName.Equals(item.SkillName, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
        {
            currentUser.SelectedSkills.Add(new UserSkillInfo
            {
                SkillId = item.SkillId,
                SkillName = item.SkillName,
                PositionId = item.PositionId,
                PositionName = item.PositionName,
                SeniorityName = item.SeniorityName,
                JobFamilyName = item.JobFamilyName,
                SkillComplexity = item.SkillComplexity,
                Knowledge = item.Knowledge,
                Experience = item.Experience,
                Depth = item.Depth,
                Credibility = item.Credibility,
                Status = item.Credibility > 0 ? "verified" : "self_declared",
                IsVerified = item.Credibility > 0
            });
        }

        _userSession.CurrentUser = currentUser;
    }

    private void RemoveSkillFromSession(ProfileSkillItem item)
    {
        var currentUser = GetCurrentUser();

        var saved = currentUser.SelectedSkills.FirstOrDefault(x =>
            (x.SkillId > 0 && x.SkillId == item.SkillId) ||
            x.SkillName.Equals(item.SkillName, StringComparison.OrdinalIgnoreCase));

        if (saved is not null)
            currentUser.SelectedSkills.Remove(saved);

        _userSession.CurrentUser = currentUser;
    }

    private void SaveExperienceToSession(ProfileExperienceItem item)
    {
        var currentUser = GetCurrentUser();

        var exists = currentUser.WorkExperiences.Any(x =>
            x.CompanyName.Equals(item.CompanyName, StringComparison.OrdinalIgnoreCase) &&
            x.PositionName.Equals(item.PositionName, StringComparison.OrdinalIgnoreCase) &&
            x.StartYear.Equals(item.StartYear, StringComparison.OrdinalIgnoreCase));

        if (exists)
            return;

        currentUser.WorkExperiences.Add(new UserWorkExperienceInfo
        {
            CompanyName = item.CompanyName,
            PositionName = item.PositionName,
            StartYear = item.StartYear,
            EndYear = item.EndYear
        });

        _userSession.CurrentUser = currentUser;
    }

    private void RemoveExperienceFromSession(ProfileExperienceItem item)
    {
        var currentUser = GetCurrentUser();

        var saved = currentUser.WorkExperiences.FirstOrDefault(x =>
            x.CompanyName.Equals(item.CompanyName, StringComparison.OrdinalIgnoreCase) &&
            x.PositionName.Equals(item.PositionName, StringComparison.OrdinalIgnoreCase) &&
            x.StartYear.Equals(item.StartYear, StringComparison.OrdinalIgnoreCase));

        if (saved is not null)
            currentUser.WorkExperiences.Remove(saved);

        _userSession.CurrentUser = currentUser;
    }

    private UserInfo GetCurrentUser()
    {
        if (_userSession.CurrentUser is not null)
        {
            UserInfo = _userSession.CurrentUser;
            return _userSession.CurrentUser;
        }

        _userSession.CurrentUser = UserInfo;
        return UserInfo;
    }

    private async Task SaveProfileDataQuietlyAsync()
    {
        var user = _userSession.CurrentUser;
        if (_profileDataApiService is null || user is null || user.Id <= 0)
            return;

        var result = await _profileDataApiService.SaveAsync(user);
        if (!result.Success)
            ErrorMessage = result.Message;
    }

    private void AddCompanyOptionIfMissing(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return;

        var exists = CompanyOptions.Any(x => x.Name.Equals(companyName.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!exists)
            CompanyOptions.Add(new CompanyChip(companyName.Trim(), false));
    }

    private void ClearExperienceForm()
    {
        ExperienceCompanyName = string.Empty;
        ExperiencePositionName = string.Empty;
        ExperienceStartYear = string.Empty;
        ExperienceEndYear = "Present";
    }

    partial void OnIsAddSkillVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(IsAddSkillVisible));
    }

    partial void OnIsAddExperienceVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(IsAddExperienceVisible));
    }

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasError));
    }

    private void RefreshSkillState()
    {
        OnPropertyChanged(nameof(HasAddedSkills));
        OnPropertyChanged(nameof(HasNoSkills));
    }

    private void RefreshExperienceState()
    {
        OnPropertyChanged(nameof(HasAddedExperiences));
        OnPropertyChanged(nameof(HasNoExperiences));
    }
}

public partial class CompanyChip : ObservableObject
{
    public CompanyChip(string name, bool isSelected = false)
    {
        Name = name;
        IsSelected = isSelected;
    }

    public string Name { get; }

    [ObservableProperty] private bool isSelected;
}

public class AvailableSkillItem
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public string SeniorityName { get; set; } = string.Empty;
    public string JobFamilyName { get; set; } = string.Empty;
    public string SkillComplexity { get; set; } = "medium";
    public string DisplayName => string.IsNullOrWhiteSpace(PositionName) ? SkillName : $"{SkillName} · {PositionName}";
}

public class ProfileSkillItem
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public string SeniorityName { get; set; } = string.Empty;
    public string JobFamilyName { get; set; } = string.Empty;
    public string SkillComplexity { get; set; } = "medium";
    public List<string> UsedIn { get; set; } = new();
    public string UsedInText => UsedIn.Count == 0 ? "Not linked to experience" : $"Used in: {string.Join(", ", UsedIn)}";
    public double Knowledge { get; set; }
    public double Experience { get; set; }
    public double Depth { get; set; }
    public double Credibility { get; set; }
    public double KnowledgeRatio => Knowledge / 100d;
    public double ExperienceRatio => Experience / 100d;
    public double DepthRatio => Depth / 100d;
}

public class ProfileExperienceItem
{
    public string CompanyName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string StartYear { get; set; } = string.Empty;
    public string EndYear { get; set; } = "Present";
    public string PeriodText => string.IsNullOrWhiteSpace(EndYear) ? StartYear : $"{StartYear} – {EndYear}";

    public string DurationText
    {
        get
        {
            if (!int.TryParse(StartYear, out var start))
                return string.Empty;

            var normalizedEnd = EndYear?.Trim();
            var end = string.IsNullOrWhiteSpace(normalizedEnd) || normalizedEnd.Equals("Present", StringComparison.OrdinalIgnoreCase)
                ? DateTime.UtcNow.Year
                : int.TryParse(normalizedEnd, out var parsedEnd)
                    ? parsedEnd
                    : start;

            var years = Math.Max(end - start, 0);
            return years == 1 ? "1 year" : $"{years} years";
        }
    }

    public bool HasDuration => !string.IsNullOrWhiteSpace(DurationText);
}
