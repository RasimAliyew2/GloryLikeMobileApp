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
    private readonly IUserSession userSession;
    [ObservableProperty] private UserInfo userInfo;
    private bool _isLoaded;

    public SkillsViewModel(
        BottomMenuState menuState,
        ISkillAndJobApiService skillAndJobApiService,
        IUserSession userSession, UserInfo userInfo,
        SkillVerificationState? skillVerificationState = null) : base(menuState)
    {
        _skillAndJobApiService = skillAndJobApiService;
        _skillVerificationState = skillVerificationState;
        this.userSession = userSession;
        UserInfo = userInfo;
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

            //var jobFamilies = await _skillAndJobApiService.GetJobFamiliesAsync();

            var selectedJob = userInfo.Job;

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
                    JobFamilyName = selectedJob.JobName
                }))
                .Where(x => !string.IsNullOrWhiteSpace(x.SkillName))
                .GroupBy(x => x.SkillName.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(x => x.SkillName)
                .ToList();

            AvailableSkills.Clear();

            foreach (var skill in allSkills)
                AvailableSkills.Add(skill);

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

        AddedSkills.Add(new ProfileSkillItem
        {
            SkillId = SelectedAvailableSkill.SkillId,
            SkillName = SelectedAvailableSkill.SkillName,
            PositionName = SelectedAvailableSkill.PositionName,
            SeniorityName = SelectedAvailableSkill.SeniorityName,
            UsedIn = selectedCompanies
        });

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
                PositionId = 0
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
