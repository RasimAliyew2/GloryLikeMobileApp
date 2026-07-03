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

    public int Knowledge { get; set; }

    public int Experience { get; set; }

    public int Depth { get; set; }

    public int Credibility { get; set; }

    public int DepthScore { get; set; }

    public string TaskComplexity { get; set; } = string.Empty;

    public string OwnershipLevel { get; set; } = string.Empty;

    public string DepthTier { get; set; } = string.Empty;
}
