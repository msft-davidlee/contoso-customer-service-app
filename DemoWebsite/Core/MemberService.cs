using DemoWebsite.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class MemberService : MicroserviceBase, IMemberService, IHealthCheck
    {
        private readonly ILogger<MemberService> _logger;
        private readonly string _memberUri;

        public MemberService(ITokenAcquisition tokenAcquisition,
            IConfiguration configuration, HttpClient httpClient,
            ILogger<MemberService> logger) : base(tokenAcquisition, configuration, httpClient)
        {
            _logger = logger;
            _memberUri = GetBaseUri() + "Members";
        }

        private async Task<List<RewardCustomer>> InvokeGetRequest(string uri)
        {

            var response = await (await GetHttpClient()).GetAsync(uri);
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
            var list = await InvokeGetRequest(_memberUri + $"?memberId={memberId}");
            return list.SingleOrDefault();
        }

        public async Task<IEnumerable<RewardCustomer>> GetRewardCustomers(string firstName, string lastName)
        {
            return await InvokeGetRequest(_memberUri + $"?firstName={firstName}&lastName={lastName}");
        }

        public async Task<IEnumerable<RewardCustomer>> GetRewardCustomers()
        {
            return await InvokeGetRequest(_memberUri);
        }

        protected override string GetUriConfigName()
        {
            return "MemberServiceUri";
        }
    }
}
