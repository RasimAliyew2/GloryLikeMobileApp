namespace MetanetA_MobileApp.Model;

public class UserSkillInfo
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public int PositionId { get; set; }

    public string PositionName { get; set; } = string.Empty;

    public string SeniorityName { get; set; } = string.Empty;

    public string JobFamilyName { get; set; } = string.Empty;

    public string SkillComplexity { get; set; } = "medium";
}
