using DemoMemberPortal.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace DemoMemberPortal.Core
{
    public class PointsService : IPointsService, IHealthCheck
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PointsService(ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<int> GetPoints(string memberId)
        {
            var client = await GetHttpClient();
            var uri = $"{GetBaseUri()}api/points/member/{memberId}";
            var points = await client.GetFromJsonAsync<MemberPointModel>(uri);

            if (points == null) throw new ApplicationException("Invalid member id.");

            return points.TotalPoints;
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

        private async Task<HttpClient> GetHttpClient()
        {
            var token = await _tokenAcquisition.GetAccessTokenForAppAsync(_configuration["AzureAD:Scopes"]);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        private string GetBaseUri()
        {
            const string configName = "MemberPointsUrl";

            var uri = _configuration[configName];

            if (string.IsNullOrEmpty(uri)) throw new ApplicationException($"{configName} is not configured.");

            if (uri.EndsWith("/")) return uri;

            return $"{uri}/";
        }
    }
}