using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.UIState;

public class SkillVerificationState
{
    public List<SelectedSkillForVerification> SelectedSkills { get; } = new();

    public void SetSelectedSkills(
        IEnumerable<Skill> skills,
        string? seniority,
        string language = "az")
    {
        SelectedSkills.Clear();

        var normalizedSeniority = NormalizeSeniority(seniority);

        foreach (var skill in skills)
        {
            var skillComplexity = string.IsNullOrWhiteSpace(skill.SkillComplexity)
                ? "medium"
                : skill.SkillComplexity.Trim().ToLowerInvariant();

            SelectedSkills.Add(new SelectedSkillForVerification
            {
                SkillId = skill.Id,
                SkillName = skill.SkillName,
                Seniority = normalizedSeniority,
                SkillComplexity = NormalizeComplexity(skillComplexity),
                Language = language
            });
        }
    }

    public void Clear()
    {
        SelectedSkills.Clear();
    }

    private static string NormalizeSeniority(string? seniority)
    {
        if (string.IsNullOrWhiteSpace(seniority))
            return "middle";

        return seniority.Trim().ToLowerInvariant() switch
        {
            "junior" => "junior",
            "middle" => "middle",
            "senior" => "senior",
            "lead" => "lead",
            "head" => "lead",
            _ => "middle"
        };
    }

    private static string NormalizeComplexity(string? complexity)
    {
        if (string.IsNullOrWhiteSpace(complexity))
            return "medium";

        return complexity.Trim().ToLowerInvariant() switch
        {
            "low" => "low",
            "medium" => "medium",
            "high" => "high",
            _ => "medium"
        };
    }
}

public class SelectedSkillForVerification
{
    public int SkillId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public string Seniority { get; set; } = "middle";

    public string SkillComplexity { get; set; } = "medium";

    public string Language { get; set; } = "az";
}