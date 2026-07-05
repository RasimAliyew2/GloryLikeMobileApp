using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model;

public partial class UserInfo : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string userName = string.Empty;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string surname = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string phoneNumber = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private JobFamily? job;

    // Sign-up zamanı seçilən və VerifySkillsPage-də score alan skill-lər.
    public List<UserSkillInfo> SelectedSkills { get; set; } = new();

    // SkillsPage-də əlavə edilən iş təcrübələri.
    public List<UserWorkExperienceInfo> WorkExperiences { get; set; } = new();
}
