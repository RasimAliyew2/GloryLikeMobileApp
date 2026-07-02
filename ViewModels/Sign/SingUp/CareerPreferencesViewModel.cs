using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp;

public partial class CareerPreferencesViewModel : ObservableObject
{
    public ObservableCollection<PreferenceChipItem> TargetRoles { get; } = new();

    public ObservableCollection<PreferenceChipItem> WorkModes { get; } = new()
    {
        new("Remote"),
        new("Hybrid", true),
        new("On-site")
    };

    public ObservableCollection<PreferenceChipItem> Seniorities { get; } = new()
    {
        new("Junior"),
        new("Middle"),
        new("Senior", true),
        new("Lead"),
        new("Head")
    };

    public ObservableCollection<PreferenceChipItem> Languages { get; } = new()
    {
        new("Azerbaijani", true),
        new("Russian", true),
        new("English"),
        new("Turkish"),
        new("German"),
        new("French"),
        new("Arabic")
    };

    public ObservableCollection<PreferenceChipItem> EmploymentTypes { get; } = new()
    {
        new("Full-time"),
        new("Part-time", true),
        new("Contract"),
        new("Freelance"),
        new("Internship")
    };

    public ObservableCollection<PreferenceChipItem> RelocationCountries { get; } = new()
    {
        new("Azerbaijan"),
        new("Kazakhstan"),
        new("Turkey"),
        new("Germany"),
        new("United Kingdom"),
        new("United States"),
        new("Netherlands"),
        new("Georgia")
    };

    [ObservableProperty]
    private string targetRoleText = string.Empty;

    [ObservableProperty]
    private bool openToRelocation = true;

    [RelayCommand]
    private void AddTargetRole()
    {
        var text = TargetRoleText?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return;

        if (!TargetRoles.Any(x => x.Text.Equals(text, StringComparison.OrdinalIgnoreCase)))
            TargetRoles.Add(new PreferenceChipItem(text, true));

        TargetRoleText = string.Empty;
    }

    [RelayCommand]
    private void RemoveTargetRole(PreferenceChipItem? item)
    {
        if (item is null)
            return;

        TargetRoles.Remove(item);
    }

    [RelayCommand]
    private void SelectWorkMode(PreferenceChipItem? item)
    {
        SelectSingle(WorkModes, item);
    }

    [RelayCommand]
    private void SelectSeniority(PreferenceChipItem? item)
    {
        SelectSingle(Seniorities, item);
    }

    [RelayCommand]
    private void ToggleLanguage(PreferenceChipItem? item)
    {
        Toggle(item);
    }

    [RelayCommand]
    private void ToggleEmploymentType(PreferenceChipItem? item)
    {
        Toggle(item);
    }

    [RelayCommand]
    private void ToggleOpenToRelocation()
    {
        OpenToRelocation = !OpenToRelocation;
    }

    [RelayCommand]
    private void ToggleRelocationCountry(PreferenceChipItem? item)
    {
        Toggle(item);
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        await Shell.Current.GoToAsync(nameof(ProfileReviewPage));
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        await Shell.Current.GoToAsync(nameof(ProfileReviewPage));
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private static void SelectSingle(IEnumerable<PreferenceChipItem> items, PreferenceChipItem? selectedItem)
    {
        if (selectedItem is null)
            return;

        foreach (var item in items)
            item.IsSelected = item == selectedItem;
    }

    private static void Toggle(PreferenceChipItem? item)
    {
        if (item is null)
            return;

        item.IsSelected = !item.IsSelected;
    }
}

public partial class PreferenceChipItem : ObservableObject
{
    public PreferenceChipItem(string text, bool isSelected = false)
    {
        Text = text;
        this.isSelected = isSelected;
    }

    public string Text { get; }

    [ObservableProperty]
    private bool isSelected;
}
