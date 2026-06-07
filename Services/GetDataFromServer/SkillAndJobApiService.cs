using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Services.GetDataFromServer
{

    public class SkillAndJobApiService : ISkillAndJobApiService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SkillAndJobApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JobFamily>> GetJobFamiliesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<JobFamily>>(
                "api/SkillAndJob/job-families",
                JsonOptions);

            return result ?? new List<JobFamily>();
        }
    }
}
