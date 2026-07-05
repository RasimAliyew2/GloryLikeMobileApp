namespace MetanetA_MobileApp.Model;

public class UserSkillInfo
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public int PositionId { get; set; }

    public string PositionName { get; set; } = string.Empty;

    public int SeniorityId { get; set; }

    public string SeniorityName { get; set; } = string.Empty;

    public int JobFamilyId { get; set; }

    public string JobFamilyName { get; set; } = string.Empty;

    public string SkillComplexity { get; set; } = "medium";

    // verified | self_declared | absent
    public string Status { get; set; } = "self_declared";

    public bool IsVerified { get; set; }

    public double KnowledgeScore { get; set; }

    public double ExperienceScore { get; set; }

    public double DepthScore { get; set; }

    public double CredibilityScore { get; set; }

    public string TaskComplexity { get; set; } = string.Empty;

    public string OwnershipLevel { get; set; } = string.Empty;

    public string DepthTier { get; set; } = string.Empty;

    public double ContextScore { get; set; }

    public double ComplexityScore { get; set; }

    public double OwnershipScore { get; set; }

    public double ResultScore { get; set; }

    // Word faylındakı CS: KnowledgeScore * 0.45 + ExperienceScore * 0.55.
    public double CalculatedCredibilityScore
    {
        get
        {
            if (CredibilityScore > 0)
                return Math.Clamp(CredibilityScore, 0, 100);

            return Math.Clamp((KnowledgeScore * 0.45d) + (ExperienceScore * 0.55d), 0, 100);
        }
    }

    // Skill Signal:
    // verified      => CS
    // self_declared => min(CS, 40)
    // absent        => 0
    public double Signal
    {
        get
        {
            var status = (Status ?? string.Empty).Trim().ToLowerInvariant();
            var cs = CalculatedCredibilityScore;

            if (IsVerified || status == "verified")
                return cs;

            if (status == "self_declared" || string.IsNullOrWhiteSpace(status))
                return Math.Min(cs, 40);

            return 0;
        }
    }

    // Backward-compatible aliases. Köhnə ViewModel-lər bu adları oxuyur.
    public double Knowledge
    {
        get => KnowledgeScore;
        set => KnowledgeScore = value;
    }

    public double Experience
    {
        get => ExperienceScore;
        set => ExperienceScore = value;
    }

    public double Depth
    {
        get => DepthScore;
        set => DepthScore = value;
    }

    public double Credibility
    {
        get => CredibilityScore;
        set => CredibilityScore = value;
    }

    public double Score
    {
        get => CredibilityScore;
        set => CredibilityScore = value;
    }

    public string Position
    {
        get => PositionName;
        set => PositionName = value;
    }

    public string Seniority
    {
        get => SeniorityName;
        set => SeniorityName = value;
    }

    public string JobFamily
    {
        get => JobFamilyName;
        set => JobFamilyName = value;
    }
}
