using MetanetA_MobileApp.Model.Questionnaires;

namespace MetanetA_MobileApp.Services.Abstractions;

public interface ISkillQuestionnaireApiService
{
    Task<SkillQuestionnaireResponse> GenerateQuestionnaireAsync(
        GenerateSkillQuestionnaireRequest request,
        CancellationToken cancellationToken = default);
}