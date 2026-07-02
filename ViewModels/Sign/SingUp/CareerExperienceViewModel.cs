using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp;

public partial class CareerExperienceViewModel : ObservableObject
{
    public ObservableCollection<IdentityStepModel> Steps { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? statusMessage;

    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    public CareerExperienceViewModel()
    {
        Steps.Add(new IdentityStepModel { Title = "Identity", SubTitle = "", IsActive = false, IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Career", SubTitle = "3 min", IsActive = true, IsCompleted = false });
        Steps.Add(new IdentityStepModel { Title = "Skills", SubTitle = "", IsActive = false });
        Steps.Add(new IdentityStepModel { Title = "Verify", SubTitle = "", IsActive = false });
        Steps.Add(new IdentityStepModel { Title = "Prefs", SubTitle = "", IsActive = false });
        Steps.Add(new IdentityStepModel { Title = "Ready", SubTitle = "", IsActive = false });
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync($"//{nameof(VerifyIdentityPage)}");
    }

    [RelayCommand]
    private async Task SkipForNow()
    {
        await Shell.Current.GoToAsync($"//{nameof(SkillsSelectionPage)}");
    }

    [RelayCommand]
    private async Task ImportFromEmas()
    {
        try
        {
            IsBusy = true;
            StatusMessage = null;

            // ƏMAS integration real API ilə qoşulanda bu hissə dəyişdiriləcək.
            await Task.Delay(500);

            await Application.Current!.MainPage!.DisplayAlert(
                "ƏMAS import",
                "ƏMAS import bağlantısı hələ qoşulmayıb. Hələlik skills mərhələsinə keçilir.",
                "OK");

            await Shell.Current.GoToAsync($"//{nameof(SkillsSelectionPage)}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadFile()
    {
        try
        {
            IsBusy = true;
            StatusMessage = null;

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Upload work experience file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "application/pdf", "text/xml", "application/xml", "application/json", "text/plain" } },
                    { DevicePlatform.iOS, new[] { "com.adobe.pdf", "public.xml", "public.json", "public.text" } },
                    { DevicePlatform.WinUI, new[] { ".pdf", ".xml", ".json" } },
                    { DevicePlatform.macOS, new[] { "pdf", "xml", "json" } }
                })
            });

            if (result is null)
                return;

            StatusMessage = $"Selected file: {result.FileName}. File parsing will be connected later.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnStatusMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasStatusMessage));
    }
}
