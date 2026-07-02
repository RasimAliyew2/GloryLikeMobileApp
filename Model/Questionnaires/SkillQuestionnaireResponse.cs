namespace MetanetA_MobileApp.Model.Questionnaires;

public class SkillQuestionnaireResponse
{
    public string Skill { get; set; } = string.Empty;

    public string Seniority { get; set; } = string.Empty;

    public string SkillComplexity { get; set; } = string.Empty;

    public List<QuestionnaireQuestionDto> Questions { get; set; } = new();

    public QuestionnaireScoringDto Scoring { get; set; } = new();
}

public class QuestionnaireQuestionDto
{
    public string Id { get; set; } = string.Empty;

    public int Order { get; set; }

    public string Dimension { get; set; } = string.Empty;

    public bool HiddenByDefault { get; set; }

    public string Text { get; set; } = string.Empty;

    public string Type { get; set; } = "single";

    public List<QuestionnaireOptionDto> Options { get; set; } = new();

    public List<QuestionnaireBranchingRuleDto> Branching { get; set; } = new();
}

public class QuestionnaireOptionDto
{
    public string Id { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public QuestionnaireOptionWeightsDto Weights { get; set; } = new();
}

public class QuestionnaireOptionWeightsDto
{
    public int Complexity { get; set; }

    public int Ownership { get; set; }

    public int Depth { get; set; }
}

public class QuestionnaireBranchingRuleDto
{
    public string IfOption { get; set; } = string.Empty;

    public string RevealQuestionId { get; set; } = string.Empty;
}

public class QuestionnaireScoringDto
{
    public int MaxComplexity { get; set; }

    public int MaxOwnership { get; set; }

    public int MaxDepth { get; set; }
}