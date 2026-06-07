using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;
using Microsoft.Maui.Storage;

namespace MetanetA_MobileApp.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    [ObservableProperty] private IUserSession userSession;

    [ObservableProperty] private ImageSource profileImage;

    [ObservableProperty] private bool isPersonalInfoExpanded;

    public ObservableCollection<PurchaseHistoryItem> PurchaseHistory { get; } = new();

    // şəkli yadda saxlamaq üçün (memory-də)
    private byte[] _profileBytes;

    // Reset üçün snapshot
    private UserInfo _snapshot;

    public ProfileViewModel(IUserSession userSession, BottomMenuState bottomMenu)
        : base(bottomMenu)
    {
        UserSession = userSession;

        // Bottom menu tab seçimi
        MenuState.Select(BottomTab.Profile);

        // Default şəkil (istəsən dəyiş)
        ProfileImage = ImageSource.FromFile("profile_placeholder.png");

        // Snapshot (Reset üçün)
        CaptureSnapshot();

        // Demo alış tarixçəsi
  
        //PurchaseHistory.Add(new PurchaseHistoryItem
        //{
        //    Date = DateTime.Now.AddDays(-5),
        //    Title = "Boya (Rokol)",
        //    Quantity = 2,
        //    Price = 40
        //});
    }

    private void CaptureSnapshot()
    {
        if (UserSession?.CurrentUser == null)
            return;

        var u = UserSession.CurrentUser;

        _snapshot = new UserInfo
        {
            Name = u.Name,
            Surname = u.Surname,
            FatherName = u.FatherName,
            City = u.City,
            Village = u.Village,
            Job = u.Job,
            PhoneNumber = u.PhoneNumber,
            BirthDate = u.BirthDate
        };
    }

    private void RestoreSnapshot()
    {
        if (_snapshot == null || UserSession?.CurrentUser == null)
            return;

        var u = UserSession.CurrentUser;

        u.Name = _snapshot.Name;
        u.Surname = _snapshot.Surname;
        u.FatherName = _snapshot.FatherName;
        u.City = _snapshot.City;
        u.Village = _snapshot.Village;
        u.Job = _snapshot.Job;
        u.PhoneNumber = _snapshot.PhoneNumber;
        u.BirthDate = _snapshot.BirthDate;
    }

    [RelayCommand]
    private void TogglePersonalInfo()
    {
        IsPersonalInfoExpanded = !IsPersonalInfoExpanded;
    }

    [RelayCommand]
    private async Task PersonalInfo()
    {
        await Shell.Current.GoToAsync($"//{nameof(ProfileUserPage)}");
    }
  [RelayCommand]
    private async Task ChangePhoto()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Profil şəkli seçin",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null)
                return;

            await using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            _profileBytes = ms.ToArray();

            ProfileImage = ImageSource.FromStream(() => new MemoryStream(_profileBytes));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Xəta", $"Şəkil seçilmədi: {ex.Message}", "OK");
        }
    }


    [RelayCommand]
    private async Task Cards()
    {
        // hələlik boş – amma event var
        await Shell.Current.DisplayAlert("Kartlarım", "Bu bölmə tezliklə əlavə olunacaq.", "OK");
    }

    [RelayCommand]
    private async Task Save()
    {
        // TODO: burda API/DB save edəcəksən
        CaptureSnapshot();
        await Shell.Current.DisplayAlert("Yadda saxlanıldı", "Məlumatlar yadda saxlanıldı.", "OK");
    }

    [RelayCommand]
    private void Reset()
    {
        RestoreSnapshot();
    }
}
