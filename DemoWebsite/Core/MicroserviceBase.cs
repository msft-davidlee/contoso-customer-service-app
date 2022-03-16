using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public abstract class MicroserviceBase
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        protected MicroserviceBase(ITokenAcquisition tokenAcquisition,
            IConfiguration configuration, HttpClient httpClient)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        protected abstract string GetUriConfigName();

        protected string GetBaseUri()
        {
            var configName = GetUriConfigName();

            var uri = _configuration[configName];

            if (string.IsNullOrEmpty(uri)) throw new ApplicationException($"{configName} is not configured.");

            if (uri.EndsWith("/")) return uri;

            return $"{uri}/";
        }

        protected async Task<HttpClient> GetHttpClient()
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { _configuration["AzureAD:Scopes"] });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
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
