using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp;

public partial class ProfileReviewViewModel : ObservableObject
{
    [ObservableProperty]
    private int profileStrength = 5;

    [ObservableProperty]
    private int hiringReadiness = 0;

    public ObservableCollection<ProfileReviewInsightItem> Insights { get; } = new()
    {
        new("Verify 1 more skill to increase your match score by ~8%."),
        new("Identity verification adds 20 trust points and significantly improves employer confidence."),
        new("Add 4 more skills to reach the recommended minimum for strong matching.")
    };

    [RelayCommand]
    private async Task FinishAsync()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Profile ready",
            "Your onboarding profile has been prepared.",
            "OK");
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        await FinishAsync();
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

public class ProfileReviewInsightItem
{
    public ProfileReviewInsightItem(string text)
    {
        Text = text;
    }

    public string Text { get; }
}
