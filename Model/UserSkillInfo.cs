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

    public string SkillComplexity { get; set; } = string.Empty;

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

    // Formula üzrə skill signal
    public double Signal
    {
        get
        {
            if (Status == "verified" || IsVerified)
                return CredibilityScore;

            if (Status == "self_declared")
                return Math.Min(CredibilityScore, 40);

            return 0;
        }
    }

    // Backward-compatible aliases
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