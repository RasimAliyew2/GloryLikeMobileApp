using System.Net.Http.Json;
using System.Text.Json;
using MetanetA_MobileApp.Model.Questionnaires;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Services.GetDataFromServer;

public class SkillQuestionnaireApiService : ISkillQuestionnaireApiService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SkillQuestionnaireApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SkillQuestionnaireResponse> GenerateQuestionnaireAsync(
        GenerateSkillQuestionnaireRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/SkillQuestionnaires/generate",
            request,
            JsonOptions,
            cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Questionnaire API error. StatusCode: {(int)response.StatusCode}. Body: {body}");
        }

        var result = JsonSerializer.Deserialize<SkillQuestionnaireResponse>(body, JsonOptions);

        return result ?? throw new InvalidOperationException("Questionnaire response deserialize edilə bilmədi.");
    }
}