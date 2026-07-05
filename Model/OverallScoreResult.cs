namespace MetanetA_MobileApp.Model;

public class OverallScoreResult
{
    public bool HasScore { get; set; }

    public double Score { get; set; }

    public int DisplayScore => (int)Math.Floor(Score + 0.5d);

    public string ScoreText => HasScore ? $"{DisplayScore}%" : "Choose target roles";

    public string StrongestRoleName { get; set; } = string.Empty;

    public string ImprovementHint { get; set; } = string.Empty;

    public List<RoleReadinessScore> Roles { get; set; } = new();
}
