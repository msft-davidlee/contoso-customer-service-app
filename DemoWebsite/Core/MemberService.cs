using DemoWebsite.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class MemberService : IMemberService, IHealthCheck
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MemberService> _logger;
        private readonly string _uri;

        public MemberService(ITokenAcquisition tokenAcquisition,
            IConfiguration configuration, HttpClient httpClient,
            ILogger<MemberService> logger)
        {
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
            _uri = GetBaseUri() + "Members";
        }

        private string GetBaseUri()
        {
            return _configuration["MemberServiceUri"];
        }

        private async Task<List<RewardCustomer>> InvokeGetRequest(string uri)
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { _configuration["AzureAD:Scopes"] });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                if (!string.IsNullOrEmpty(body))
                {
                    _logger.LogTrace(body);
                }

                throw;
            }

            return JsonSerializer.Deserialize<List<RewardCustomer>>(body);
        }

        public async Task<RewardCustomer> GetRewardCustomer(string memberId)
        {
            var list = await InvokeGetRequest(_uri + $"?memberId={memberId}");
            return list.SingleOrDefault();
        }

        public async Task<IEnumerable<RewardCustomer>> GetRewardCustomers(string firstName, string lastName)
        {
            return await InvokeGetRequest(_uri + $"?firstName={firstName}&lastName={lastName}");
        }

        public async Task<IEnumerable<RewardCustomer>> GetRewardCustomers()
        {
            return await InvokeGetRequest(_uri);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(GetBaseUri()))
                throw new ApplicationException("MemberServiceUri is not configured.");

            var uri = $"{GetBaseUri()}/health";

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
