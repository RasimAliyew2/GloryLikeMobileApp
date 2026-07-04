using System.Collections.ObjectModel;

namespace MetanetA_MobileApp.Model;

public partial class UserInfo
{
    public ObservableCollection<UserWorkExperienceInfo> WorkExperiences { get; } = new();
}

public class UserWorkExperienceInfo
{
    public string CompanyName { get; set; } = string.Empty;

    public string PositionName { get; set; } = string.Empty;

    public string StartYear { get; set; } = string.Empty;

    public string EndYear { get; set; } = string.Empty;
}
