using Microsoft.Maui.Graphics;

namespace MetanetA_MobileApp.Model;

public class OpportunityItem
{
    public string LogoLetter { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string PostedAgo { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string WorkType { get; set; } = string.Empty;

    public string Level { get; set; } = string.Empty;

    public string Salary { get; set; } = string.Empty;

    public int Score { get; set; }

    public string ScoreText => Score.ToString() + "%";

    public string ScoreColor { get; set; } = "#10B981";

    public Color ScoreMauiColor => Color.FromArgb(ScoreColor);

    public string AboutRole { get; set; } = string.Empty;

    public string Responsibilities { get; set; } = string.Empty;

    public string MatchedSkills { get; set; } = string.Empty;

    public string MissingSkills { get; set; } = string.Empty;

    public string MatchNote { get; set; } = string.Empty;

    public bool IsExpanded { get; set; }

    public bool IsSaved { get; set; }

    public string SaveIcon => IsSaved ? "♥" : "♡";

    public string SaveColor => IsSaved ? "#EF4444" : "#E5E7EB";

    public string ArrowIcon => IsExpanded ? "⌃" : "⌄";
}
