using System.Collections.Generic;
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

    /// <summary>
    /// Skills selected during sign-up and the scores collected during VerifySkillsPage.
    /// SkillsPage reads this list and renders the skill cards.
    /// </summary>
    public List<UserSkillInfo> SelectedSkills { get; set; } = new();
}
