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

        UserInfo = userInfo;
        _userSession.CurrentUser ??= UserInfo;

        MenuState.Select(BottomTab.Profile);

        CompanyOptions.Add(new CompanyChip("Azercell Telecom", true));
        CompanyOptions.Add(new CompanyChip("Umico", false));
    }

    public ObservableCollection<AvailableSkillItem> AvailableSkills { get; } = new();

    public ObservableCollection<CompanyChip> CompanyOptions { get; } = new();

    public ObservableCollection<ProfileSkillItem> AddedSkills { get; } = new();

    [ObservableProperty]
    private UserInfo userInfo;

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

            LoadAvailableSkillsFromSelectedJob();
            LoadSkillsSelectedDuringSignup();

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
            PositionName = SelectedAvailableSkill.PositionName,
            SeniorityName = SelectedAvailableSkill.SeniorityName,
            JobFamilyName = SelectedAvailableSkill.JobFamilyName,
            SkillComplexity = SelectedAvailableSkill.SkillComplexity,
            UsedIn = selectedCompanies
        };

        AddedSkills.Add(profileSkill);
        SaveSkillToSession(profileSkill);

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

    private void LoadAvailableSkillsFromSelectedJob()
    {
        AvailableSkills.Clear();

        var currentUser = GetCurrentUser();
        var selectedJob = currentUser.Job;

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
            if (AddedSkills.Any(x => x.SkillId == selectedSkill.SkillId))
                continue;

            AddedSkills.Add(new ProfileSkillItem
            {
                SkillId = selectedSkill.SkillId,
                SkillName = selectedSkill.SkillName,
                PositionName = selectedSkill.PositionName,
                SeniorityName = selectedSkill.SeniorityName,
                JobFamilyName = selectedSkill.JobFamilyName,
                SkillComplexity = selectedSkill.SkillComplexity,
                UsedIn = new List<string> { "Selected during sign up" }
            });
        }

        RefreshSkillState();
    }

    private void SaveSkillToSession(ProfileSkillItem item)
    {
        var currentUser = GetCurrentUser();

        var exists = currentUser.SelectedSkills.Any(x =>
            x.SkillId == item.SkillId ||
            x.SkillName.Equals(item.SkillName, StringComparison.OrdinalIgnoreCase));

        if (exists)
            return;

        currentUser.SelectedSkills.Add(new UserSkillInfo
        {
            SkillId = item.SkillId,
            SkillName = item.SkillName,
            PositionName = item.PositionName,
            SeniorityName = item.SeniorityName,
            JobFamilyName = item.JobFamilyName,
            SkillComplexity = item.SkillComplexity
        });

        _userSession.CurrentUser = currentUser;
    }

    private void RemoveSkillFromSession(ProfileSkillItem item)
    {
        var currentUser = GetCurrentUser();

        var saved = currentUser.SelectedSkills.FirstOrDefault(x =>
            x.SkillId == item.SkillId ||
            x.SkillName.Equals(item.SkillName, StringComparison.OrdinalIgnoreCase));

        if (saved is not null)
            currentUser.SelectedSkills.Remove(saved);

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

    public int PositionId { get; set; }

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

    public double KnowledgeRatio => Knowledge / 100d;

    public double ExperienceRatio => Experience / 100d;

    public double DepthRatio => Depth / 100d;
}
