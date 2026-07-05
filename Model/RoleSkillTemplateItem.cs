namespace MetanetA_MobileApp.Model;

public class RoleSkillTemplateItem
{
    public string RoleId { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;

    public string SkillId { get; set; } = string.Empty;

    public string SkillName { get; set; } = string.Empty;

    // Spec: core = 2, secondary = 1.
    public int Weight { get; set; } = 1;
}
