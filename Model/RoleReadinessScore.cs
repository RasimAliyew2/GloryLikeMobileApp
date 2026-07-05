namespace MetanetA_MobileApp.Model;

public class RoleReadinessScore
{
    public string RoleId { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public int Priority { get; set; }

    // Keep float for sorting.
    public double Score { get; set; }

    // Display as integer, round half up.
    public int DisplayScore => (int)Math.Floor(Score + 0.5d);

    public string ScoreText => $"{DisplayScore}%";

    public string ImprovementHint { get; set; } = string.Empty;
}
