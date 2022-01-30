using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class HttpAlternateIdService : IAlternateIdService, IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private static readonly HttpClient _client = new HttpClient();

        public HttpAlternateIdService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetMemberIdAsync(string alternateId)
        {
            var alternateIdServiceUri = _configuration["AlternateIdServiceUri"];
            if (!string.IsNullOrEmpty(alternateIdServiceUri))
            {
                var uri = $"{alternateIdServiceUri}/AlternateId/{alternateId}";
                var response = await _client.GetAsync(new Uri(uri));
                response.EnsureSuccessStatusCode();

                var o = JObject.Parse(await response.Content.ReadAsStringAsync());

                return o["memberId"].ToString();
            }

            throw new ApplicationException("AlternateIdServiceUri is not configured.");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var alternateIdServiceUri = _configuration["AlternateIdServiceUri"];
            if (string.IsNullOrEmpty(alternateIdServiceUri))
                throw new ApplicationException("AlternateIdServiceUri is not configured.");

            var uri = $"{alternateIdServiceUri}/health";

            try
            {
                var response = await _client.GetAsync(new Uri(uri));
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
