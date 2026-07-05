using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Scoring;

public interface IScoreCalculationService
{
    OverallScoreResult CalculateOverallScore(
        IEnumerable<CandidateTargetRole> targetRoles,
        IEnumerable<RoleSkillTemplateItem> roleSkillTemplates,
        IEnumerable<UserSkillScoreInfo> candidateSkills);
}
