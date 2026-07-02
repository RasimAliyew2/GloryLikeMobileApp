namespace MetanetA_MobileApp.Model.Questionnaires;

public class GenerateSkillQuestionnaireRequest
{
    public string Skill { get; set; } = string.Empty;

    public string SkillComplexity { get; set; } = "medium";

    public string Seniority { get; set; } = "middle";

    public string Language { get; set; } = "az";
}