using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Scoring;

public class ScoreCalculationService : IScoreCalculationService
{
    private const double TargetSignal = 70d;

    public OverallScoreResult CalculateOverallScore(
        IEnumerable<CandidateTargetRole> targetRoles,
        IEnumerable<RoleSkillTemplateItem> roleSkillTemplates,
        IEnumerable<UserSkillScoreInfo> candidateSkills)
    {
        var roles = (targetRoles ?? Enumerable.Empty<CandidateTargetRole>())
            .Where(r => !string.IsNullOrWhiteSpace(r.RoleId) || !string.IsNullOrWhiteSpace(r.RoleName))
            .Take(5)
            .ToList();

        if (roles.Count == 0)
        {
            return new OverallScoreResult
            {
                HasScore = false,
                StrongestRoleName = string.Empty,
                ImprovementHint = "Choose target roles"
            };
        }

        var templates = (roleSkillTemplates ?? Enumerable.Empty<RoleSkillTemplateItem>()).ToList();

        var skillMap = (candidateSkills ?? Enumerable.Empty<UserSkillScoreInfo>())
            .Where(s => !string.IsNullOrWhiteSpace(s.SkillId) || !string.IsNullOrWhiteSpace(s.SkillName))
            .GroupBy(MakeSkillKey)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var readinessScores = new List<RoleReadinessScore>();

        foreach (var role in roles)
        {
            var roleTemplates = templates
                .Where(t => SameRole(t, role))
                .Where(t => !string.IsNullOrWhiteSpace(t.SkillId) || !string.IsNullOrWhiteSpace(t.SkillName))
                .ToList();

            var totalWeight = roleTemplates.Sum(t => NormalizeWeight(t.Weight));
            if (totalWeight <= 0)
                continue;

            var weightedSum = roleTemplates.Sum(t =>
            {
                skillMap.TryGetValue(MakeSkillKey(t), out var skill);
                var signal = skill?.Signal ?? 0d;
                return NormalizeWeight(t.Weight) * signal;
            });

            var score = weightedSum / totalWeight;

            readinessScores.Add(new RoleReadinessScore
            {
                RoleId = role.RoleId,
                RoleName = ResolveRoleName(role, roleTemplates),
                Priority = role.Priority,
                Score = Math.Clamp(score, 0d, 100d),
                ImprovementHint = BuildImprovementHint(roleTemplates, skillMap, totalWeight)
            });
        }

        var ordered = readinessScores
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Priority)
            .ThenBy(r => r.RoleName)
            .ToList();

        var strongest = ordered.FirstOrDefault();
        if (strongest is null)
        {
            return new OverallScoreResult
            {
                HasScore = false,
                StrongestRoleName = string.Empty,
                ImprovementHint = "Choose target roles"
            };
        }

        return new OverallScoreResult
        {
            HasScore = true,
            Score = strongest.Score,
            StrongestRoleName = strongest.RoleName,
            ImprovementHint = strongest.ImprovementHint,
            Roles = ordered
        };
    }

    private static string ResolveRoleName(CandidateTargetRole role, List<RoleSkillTemplateItem> roleTemplates)
    {
        if (!string.IsNullOrWhiteSpace(role.RoleName))
            return role.RoleName.Trim();

        return roleTemplates.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.RoleName))?.RoleName?.Trim()
               ?? role.RoleId.Trim();
    }

    private static string BuildImprovementHint(
        List<RoleSkillTemplateItem> template,
        IReadOnlyDictionary<string, UserSkillScoreInfo> skillMap,
        int totalWeight)
    {
        var candidates = template
            .Select(t =>
            {
                skillMap.TryGetValue(MakeSkillKey(t), out var skill);
                var status = (skill?.Status ?? "absent").Trim().ToLowerInvariant();
                var isVerified = status == "verified";
                var signal = skill?.Signal ?? 0d;
                var weight = NormalizeWeight(t.Weight);
                var gain = weight * Math.Max(0d, TargetSignal - signal) / totalWeight;

                return new
                {
                    Template = t,
                    Skill = skill,
                    Status = status,
                    IsVerified = isVerified,
                    Weight = weight,
                    Gain = gain
                };
            })
            .Where(x => !x.IsVerified)
            .OrderByDescending(x => x.Gain)
            .ThenByDescending(x => x.Weight)
            .ThenBy(x => ResolveSkillName(x.Template, x.Skill), StringComparer.OrdinalIgnoreCase)
            .ToList();

        var best = candidates.FirstOrDefault();
        if (best is null)
            return "All key skills verified";

        var skillName = ResolveSkillName(best.Template, best.Skill);
        var gainText = ((int)Math.Floor(best.Gain + 0.5d)).ToString();

        var isSelfDeclared = best.Status == "self_declared" || best.Status == "self-declared" || best.Status == "selfdeclared";
        return isSelfDeclared
            ? $"Verify {skillName} → ≈ +{gainText}"
            : $"Add & verify {skillName} → ≈ +{gainText}";
    }

    private static string ResolveSkillName(RoleSkillTemplateItem template, UserSkillScoreInfo? skill)
    {
        if (!string.IsNullOrWhiteSpace(template.SkillName))
            return template.SkillName.Trim();

        if (!string.IsNullOrWhiteSpace(skill?.SkillName))
            return skill.SkillName.Trim();

        return template.SkillId.Trim();
    }

    private static int NormalizeWeight(int weight)
    {
        return weight <= 1 ? 1 : 2;
    }

    private static bool SameRole(RoleSkillTemplateItem template, CandidateTargetRole role)
    {
        if (!string.IsNullOrWhiteSpace(role.RoleId) && !string.IsNullOrWhiteSpace(template.RoleId))
            return string.Equals(template.RoleId.Trim(), role.RoleId.Trim(), StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(role.RoleName) && !string.IsNullOrWhiteSpace(template.RoleName))
            return string.Equals(template.RoleName.Trim(), role.RoleName.Trim(), StringComparison.OrdinalIgnoreCase);

        return false;
    }

    private static string MakeSkillKey(RoleSkillTemplateItem item)
    {
        return !string.IsNullOrWhiteSpace(item.SkillId)
            ? $"id:{item.SkillId.Trim()}"
            : $"name:{item.SkillName.Trim()}";
    }

    private static string MakeSkillKey(UserSkillScoreInfo item)
    {
        return !string.IsNullOrWhiteSpace(item.SkillId)
            ? $"id:{item.SkillId.Trim()}"
            : $"name:{item.SkillName.Trim()}";
    }
}
