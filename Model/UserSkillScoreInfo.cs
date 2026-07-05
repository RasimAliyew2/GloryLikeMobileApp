namespace MetanetA_MobileApp.Model;

public class UserSkillScoreInfo
{
    public string SkillId { get; set; } = string.Empty;

    public string SkillName { get; set; } = string.Empty;

    // verified | self_declared | absent
    public string Status { get; set; } = "self_declared";

    // 0-100. Produced by verification quiz.
    public double KnowledgeScore { get; set; }

    // 0-100. Produced by Skill Depth Assessment.
    public double ExperienceScore { get; set; }

    // CS = KnowledgeScore * 0.45 + ExperienceScore * 0.55
    public double CredibilityScore => Math.Clamp(KnowledgeScore * 0.45d + ExperienceScore * 0.55d, 0d, 100d);

    // Skill Signal used by readiness formula.
    // verified      => CS
    // self_declared => min(CS, 40)
    // absent        => 0
    public double Signal
    {
        get
        {
            var status = (Status ?? string.Empty).Trim().ToLowerInvariant();

            if (status == "verified")
                return CredibilityScore;

            if (status == "self_declared" || status == "self-declared" || status == "selfdeclared")
                return Math.Min(CredibilityScore, 40d);

            return 0d;
        }
    }
}
