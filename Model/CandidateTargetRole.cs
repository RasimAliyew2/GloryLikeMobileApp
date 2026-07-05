namespace MetanetA_MobileApp.Model;

public class CandidateTargetRole
{
    public string RoleId { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    // Lower number = higher priority. Used as tie-breaker.
    public int Priority { get; set; }
}
