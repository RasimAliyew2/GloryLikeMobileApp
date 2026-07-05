using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model;

public partial class UserInfo : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string surname = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    // GloryLike flow-da Job string deyil, JobFamily olmalıdır.
    // SkillsViewModel currentUser.Job.Seniorities və currentUser.Job.JobName oxuyur.
    [ObservableProperty]
    private JobFamily? job;

    // Sign-up / VerifySkills / SkillsPage tərəfindən istifadə olunur.
    public List<UserSkillInfo> SelectedSkills { get; set; } = new();

    // SkillsPage experience kartları üçün.
    public List<UserWorkExperienceInfo> WorkExperiences { get; set; } = new();
}
