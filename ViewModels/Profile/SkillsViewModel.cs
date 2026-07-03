using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Profile;

public partial class SkillsViewModel : BaseViewModel
{
    private readonly ISkillAndJobApiService _skillAndJobApiService;
    private readonly SkillVerificationState? _skillVerificationState;
    private readonly IUserSession _userSession;

    [ObservableProperty]
    private UserInfo userInfo;

    private bool _isLoaded;

    public SkillsViewModel(
        BottomMenuState menuState,
        ISkillAndJobApiService skillAndJobApiService,
        IUserSession userSession,
        UserInfo userInfo,
        SkillVerificationState? skillVerificationState = null)
        : base(menuState)
    {
        _skillAndJobApiService = skillAndJobApiService;
        _skillVerificationState = skillVerificationState;
        _userSession = userSession;
        UserInfo = userSession.CurrentUser ?? userInfo;

        _userSession.CurrentUser ??= UserInfo;

        MenuState.Select(BottomTab.Profile);

        CompanyOptions.Add(new CompanyChip("Azercell Telecom", true));
        CompanyOptions.Add(new CompanyChip("Umico", false));
    }

    public ObservableCollection<AvailableSkillItem> AvailableSkills { get; } = new();

    public ObservableCollection<CompanyChip> CompanyOptions { get; } = new();

    public ObservableCollection<ProfileSkillItem> AddedSkills { get; } = new();

    [ObservableProperty]
    private AvailableSkillItem? selectedAvailableSkill;

    [ObservableProperty]
    private bool isAddSkillVisible;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasAddedSkills => AddedSkills.Count > 0;

    public bool HasNoSkills => AddedSkills.Count == 0;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            LoadSkillsSelectedDuringSignup();
            LoadAvailableSkillsFromSelectedJob();

            _isLoaded = true;
            RefreshSkillState();
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

        var skillItem = new ProfileSkillItem
        {
            SkillId = SelectedAvailableSkill.SkillId,
            SkillName = SelectedAvailableSkill.SkillName,
            PositionName = SelectedAvailableSkill.PositionName,
            SeniorityName = SelectedAvailableSkill.SeniorityName,
            JobFamilyName = SelectedAvailableSkill.JobFamilyName,
            SkillComplexity = SelectedAvailableSkill.SkillComplexity,
            UsedIn = selectedCompanies,
            Knowledge = 0,
            Experience = 0,
            Depth = 0,
            Credibility = 0
        };

        AddedSkills.Add(skillItem);
        SaveProfileSkillToSession(skillItem);

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

        var currentUser = _userSession.CurrentUser;
        var stored = currentUser?.SelectedSkills.FirstOrDefault(x => x.SkillId == item.SkillId);

        if (stored is not null)
            currentUser!.SelectedSkills.Remove(stored);

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
                PositionId = 0,
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
    private async Task AddExperienceAsync()
    {
        await Shell.Current.DisplayAlert("Experience", "Experience əlavə etmə formu hələ qoşulmayıb.", "OK");
    }

    [RelayCommand]
    private async Task EditProfileAsync()
    {
        await Shell.Current.DisplayAlert("Edit profile", "Edit profile clicked", "OK");
    }

    private void LoadSkillsSelectedDuringSignup()
    {
        AddedSkills.Clear();

        var currentUser = _userSession.CurrentUser ?? UserInfo;
        _userSession.CurrentUser ??= currentUser;

        foreach (var skill in currentUser.SelectedSkills)
        {
            if (AddedSkills.Any(x => x.SkillId == skill.SkillId))
                continue;

            AddedSkills.Add(new ProfileSkillItem
            {
                SkillId = skill.SkillId,
                SkillName = skill.SkillName,
                PositionName = skill.PositionName,
                SeniorityName = skill.SeniorityName,
                JobFamilyName = skill.JobFamilyName,
                SkillComplexity = skill.SkillComplexity,
                Knowledge = skill.Knowledge,
                Experience = skill.Experience,
                Depth = skill.Depth,
                Credibility = skill.Credibility,
                DepthScore = skill.DepthScore,
                DepthTier = skill.DepthTier,
                OwnershipLevel = skill.OwnershipLevel,
                TaskComplexity = skill.TaskComplexity,
                UsedIn = new List<string> { "Selected during sign-up" }
            });
        }
    }

    private void LoadAvailableSkillsFromSelectedJob()
    {
        AvailableSkills.Clear();

        var selectedJob = (_userSession.CurrentUser ?? UserInfo).Job;

        if (selectedJob is null)
            return;

        var allSkills = selectedJob.Seniorities
            .SelectMany(seniority => seniority.Positions.Select(position => new
            {
                seniority,
                position
            }))
            .SelectMany(x => x.position.Skills.Select(skill => new AvailableSkillItem
            {
                SkillId = skill.Id,
                SkillName = skill.SkillName,
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

    private void SaveProfileSkillToSession(ProfileSkillItem item)
    {
        var currentUser = _userSession.CurrentUser ?? UserInfo;
        _userSession.CurrentUser ??= currentUser;

        var existing = currentUser.SelectedSkills.FirstOrDefault(x => x.SkillId == item.SkillId);

        if (existing is null)
        {
            currentUser.SelectedSkills.Add(new UserSkillInfo
            {
                SkillId = item.SkillId,
                SkillName = item.SkillName,
                PositionName = item.PositionName,
                SeniorityName = item.SeniorityName,
                JobFamilyName = item.JobFamilyName,
                SkillComplexity = item.SkillComplexity,
                Knowledge = item.Knowledge,
                Experience = item.Experience,
                Depth = item.Depth,
                Credibility = item.Credibility,
                DepthScore = item.DepthScore,
                DepthTier = item.DepthTier,
                OwnershipLevel = item.OwnershipLevel,
                TaskComplexity = item.TaskComplexity
            });

            return;
        }

        existing.SkillName = item.SkillName;
        existing.PositionName = item.PositionName;
        existing.SeniorityName = item.SeniorityName;
        existing.JobFamilyName = item.JobFamilyName;
        existing.SkillComplexity = item.SkillComplexity;
        existing.Knowledge = item.Knowledge;
        existing.Experience = item.Experience;
        existing.Depth = item.Depth;
        existing.Credibility = item.Credibility;
        existing.DepthScore = item.DepthScore;
        existing.DepthTier = item.DepthTier;
        existing.OwnershipLevel = item.OwnershipLevel;
        existing.TaskComplexity = item.TaskComplexity;
    }

    partial void OnIsAddSkillVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(IsAddSkillVisible));
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
}

public partial class CompanyChip : ObservableObject
{
    public CompanyChip(string name, bool isSelected = false)
    {
        Name = name;
        IsSelected = isSelected;
    }

    public string Name { get; }

    [ObservableProperty]
    private bool isSelected;
}

public class AvailableSkillItem
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public string PositionName { get; set; } = string.Empty;

    public string SeniorityName { get; set; } = string.Empty;

    public string JobFamilyName { get; set; } = string.Empty;

    public string SkillComplexity { get; set; } = "medium";

    public string DisplayName => string.IsNullOrWhiteSpace(PositionName)
        ? SkillName
        : $"{SkillName} · {PositionName}";
}

public class ProfileSkillItem
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public string PositionName { get; set; } = string.Empty;

    public string SeniorityName { get; set; } = string.Empty;

    public string JobFamilyName { get; set; } = string.Empty;

    public string SkillComplexity { get; set; } = "medium";

    public List<string> UsedIn { get; set; } = new();

    public string UsedInText => UsedIn.Count == 0
        ? "Not linked to experience"
        : $"Used in: {string.Join(", ", UsedIn)}";

    public int Knowledge { get; set; }

    public int Experience { get; set; }

    public int Depth { get; set; }

    public int Credibility { get; set; }

    public int DepthScore { get; set; }

    public string TaskComplexity { get; set; } = string.Empty;

    public string OwnershipLevel { get; set; } = string.Empty;

    public string DepthTier { get; set; } = string.Empty;

    public double KnowledgeRatio => Knowledge / 100d;

    public double ExperienceRatio => Experience / 100d;

    public double DepthRatio => Depth / 100d;
}
