using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DemoCustomerServiceMember.Core
{
    public class AlternateMemberIdService : IAlternateMemberIdService, IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AlternateMemberIdService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GetMemberId(string alternateMemberId, string authorization)
        {
            var uri = $"{GetBaseUri()}AlternateId/{alternateMemberId}";

            var response = await GetHttpClient(authorization).GetAsync(new Uri(uri));

            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var o = JsonSerializer.Deserialize<AlternateMemberIdModel>(content);

            if (o == null) throw new ApplicationException("Unable to convert response from alternate member Id api to model.");

            return o.MemberId;
        }

        protected HttpClient GetHttpClient(string authorization)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        protected string GetUriConfigName()
        {
            return "AlternateIdServiceUri";
        }

        private string GetBaseUri()
        {
            var configName = GetUriConfigName();

            var uri = _configuration[configName];

            if (string.IsNullOrEmpty(uri)) throw new ApplicationException($"{configName} is not configured.");

            if (uri.EndsWith("/")) return uri;

            return $"{uri}/";
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var uri = $"{GetBaseUri()}health";

            try
            {
                var response = await _httpClient.GetAsync(new Uri(uri));
                response.EnsureSuccessStatusCode();

                return HealthCheckResult.Healthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
