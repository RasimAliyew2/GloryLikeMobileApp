using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Abstractions;

public interface IJobOffersApiService
{
    Task<List<JobOfferApiItem>> GetJobOffersAsync(CancellationToken cancellationToken = default);
}
