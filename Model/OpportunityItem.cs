namespace MetanetA_MobileApp.Model
{
    public class OpportunityItem
    {
        public string LogoLetter { get; set; }
        public string Company { get; set; }
        public string PostedAgo { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string WorkType { get; set; }
        public string Level { get; set; }
        public string Salary { get; set; }
        public int Score { get; set; }
        public string ScoreColor { get; set; }

        public string AboutRole { get; set; }
        public string Responsibilities { get; set; }
        public string MatchedSkills { get; set; }
        public string MissingSkills { get; set; }
        public string MatchNote { get; set; }

        public bool IsExpanded { get; set; }
        public bool IsSaved { get; set; }

        public string SaveIcon => IsSaved ? "♥" : "♡";
        public string SaveColor => IsSaved ? "#EF4444" : "#071331";
        public string ArrowIcon => IsExpanded ? "⌃" : "⌄";
    }
}