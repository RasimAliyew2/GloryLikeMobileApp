using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp;

public partial class SkillEvidenceViewModel : ObservableObject
{
    private readonly SkillVerificationState _skillVerificationState;

    public SkillEvidenceViewModel(SkillVerificationState skillVerificationState)
    {
        _skillVerificationState = skillVerificationState;

        Steps.Add(new IdentityStepModel { Title = "Identity", IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Career", IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Skills", IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Verify", SubTitle = "4 min", IsActive = true });
        Steps.Add(new IdentityStepModel { Title = "Prefs" });
        Steps.Add(new IdentityStepModel { Title = "Ready" });
    }

    private bool _isLoaded;

    public ObservableCollection<IdentityStepModel> Steps { get; } = new();

    public ObservableCollection<SkillEvidenceItem> Skills { get; } = new();

    [ObservableProperty]
    private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    [RelayCommand]
    private void Load()
    {
        if (_isLoaded)
            return;

        Skills.Clear();

        var selectedSkills = _skillVerificationState.SelectedSkills;

        if (selectedSkills.Count == 0)
        {
            Skills.Add(new SkillEvidenceItem("Selected skill") { IsExpanded = true });
        }
        else
        {
            foreach (var skill in selectedSkills)
            {
                Skills.Add(new SkillEvidenceItem(skill.SkillName)
                {
                    IsExpanded = Skills.Count == 0
                });
            }
        }

        _isLoaded = true;
    }

    [RelayCommand]
    private void ToggleSkill(SkillEvidenceItem? item)
    {
        if (item is null)
            return;

        item.IsExpanded = !item.IsExpanded;
    }

    [RelayCommand]
    private async Task AttachCertificateAsync(SkillEvidenceItem? item)
    {
        if (item is null)
            return;

        try
        {
            var fileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/pdf", "application/xml", "text/xml", "application/json" } },
                { DevicePlatform.iOS, new[] { "com.adobe.pdf", "public.xml", "public.json" } },
                { DevicePlatform.WinUI, new[] { ".pdf", ".xml", ".json" } },
                { DevicePlatform.MacCatalyst, new[] { "pdf", "xml", "json" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select certificate file",
                FileTypes = fileType
            });

            if (result is null)
                return;

            item.AttachedCertificateFileName = result.FileName;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void SelectCompanyType(ChoiceChipItem? chip)
    {
        if (chip is null)
            return;

        var owner = Skills.FirstOrDefault(x => x.CompanyTypes.Contains(chip));
        if (owner is null)
            return;

        foreach (var item in owner.CompanyTypes)
            item.IsSelected = false;

        chip.IsSelected = true;
        owner.SelectedCompanyType = chip.Text;
    }

    [RelayCommand]
    private void SelectDuration(ChoiceChipItem? chip)
    {
        if (chip is null)
            return;

        var owner = Skills.FirstOrDefault(x => x.Durations.Contains(chip));
        if (owner is null)
            return;

        foreach (var item in owner.Durations)
            item.IsSelected = false;

        chip.IsSelected = true;
        owner.SelectedDuration = chip.Text;
    }

    [RelayCommand]
    private async Task SubmitCertificateAsync(SkillEvidenceItem? item)
    {
        if (item is null)
            return;

        item.CredibilityScore = Math.Max(item.CredibilityScore, 35);

        await Application.Current!.MainPage!.DisplayAlert(
            "Certificate saved",
            $"{item.SkillName} üçün certificate məlumatı saxlandı.",
            "OK");
    }

    [RelayCommand]
    private async Task SubmitEmployerHistoryAsync(SkillEvidenceItem? item)
    {
        if (item is null)
            return;

        item.CredibilityScore = Math.Max(item.CredibilityScore, 65);

        await Application.Current!.MainPage!.DisplayAlert(
            "Employer history saved",
            $"{item.SkillName} üçün employer history məlumatı saxlandı.",
            "OK");
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SkipForNowAsync()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Skipped",
            "Verification evidence skipped for now.",
            "OK");
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        await Shell.Current.GoToAsync(nameof(CareerPreferencesPage));
      // await Application.Current!.MainPage!.DisplayAlert(
      //     "Continue",
      //     "Növbəti onboarding page hələ qoşulmayıb. Preferences/Ready page hazır olanda bu command ora yönləndiriləcək.",
      //     "OK");
    } //

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasError));
    }
}

public partial class SkillEvidenceItem : ObservableObject
{
    public SkillEvidenceItem(string skillName)
    {
        SkillName = skillName;

        CompanyTypes.Add(new ChoiceChipItem("Startup"));
        CompanyTypes.Add(new ChoiceChipItem("SMB"));
        CompanyTypes.Add(new ChoiceChipItem("Enterprise"));
        CompanyTypes.Add(new ChoiceChipItem("Multinational"));

        Durations.Add(new ChoiceChipItem("<1 year"));
        Durations.Add(new ChoiceChipItem("1–2 years"));
        Durations.Add(new ChoiceChipItem("3–5 years"));
        Durations.Add(new ChoiceChipItem("5+ years"));

        Industries.Add("Industry...");
        Industries.Add("Technology");
        Industries.Add("Banking / Finance");
        Industries.Add("Retail");
        Industries.Add("Telecom");
        Industries.Add("Healthcare");
        Industries.Add("Education");
        Industries.Add("Other");

        SelectedIndustry = Industries[0];
    }

    public string SkillName { get; }

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private int credibilityScore;

    public string CredibilityText => $"Credibility {CredibilityScore}/100";

    [ObservableProperty]
    private string? certificateName;

    [ObservableProperty]
    private string? issuer;

    [ObservableProperty]
    private string? attachedCertificateFileName;

    [ObservableProperty]
    private string? companyName;

    [ObservableProperty]
    private string? selectedCompanyType;

    [ObservableProperty]
    private string? selectedIndustry;

    [ObservableProperty]
    private string? role;

    [ObservableProperty]
    private string? selectedDuration;

    public ObservableCollection<ChoiceChipItem> CompanyTypes { get; } = new();

    public ObservableCollection<ChoiceChipItem> Durations { get; } = new();

    public ObservableCollection<string> Industries { get; } = new();

    public bool HasAttachedCertificate => !string.IsNullOrWhiteSpace(AttachedCertificateFileName);

    partial void OnCredibilityScoreChanged(int value)
    {
        OnPropertyChanged(nameof(CredibilityText));
    }

    partial void OnAttachedCertificateFileNameChanged(string? value)
    {
        OnPropertyChanged(nameof(HasAttachedCertificate));
    }
}

public partial class ChoiceChipItem : ObservableObject
{
    public ChoiceChipItem(string text)
    {
        Text = text;
    }

    public string Text { get; }

    [ObservableProperty]
    private bool isSelected;
}
