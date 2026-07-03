using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model;

public partial class UserInfo : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string surname = string.Empty;

    [ObservableProperty]
    private JobFamily? job;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public ObservableCollection<UserSkillInfo> SelectedSkills { get; } = new();
}
