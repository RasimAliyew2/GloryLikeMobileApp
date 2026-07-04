using System.Net.Http.Json;
using System.Text.Json;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Services.GetDataFromServer;

public class JobOffersApiService : IJobOffersApiService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public JobOffersApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<JobOfferApiItem>> GetJobOffersAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync("api/JobOffers", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"JobOffers API error. StatusCode: {(int)response.StatusCode}. Body: {body}");
        }

        var result = JsonSerializer.Deserialize<List<JobOfferApiItem>>(body, JsonOptions);
        return result ?? new List<JobOfferApiItem>();
    }
}
