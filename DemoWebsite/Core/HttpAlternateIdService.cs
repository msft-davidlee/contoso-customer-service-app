using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class HttpAlternateIdService : MicroserviceBase, IAlternateIdService, IHealthCheck
    {
        public HttpAlternateIdService(IConfiguration configuration, HttpClient httpClient, ITokenAcquisition tokenAcquisition)
            : base(tokenAcquisition, configuration, httpClient)
        {

        }

        protected override string GetUriConfigName()
        {
            return "AlternateIdServiceUri";
        }

        public async Task<string> GetMemberIdAsync(string alternateId)
        {
            var uri = $"{GetBaseUri()}AlternateId/{alternateId}";

            var response = await (await GetHttpClient()).GetAsync(new Uri(uri));
            response.EnsureSuccessStatusCode();

            var o = JObject.Parse(await response.Content.ReadAsStringAsync());

            return o["memberId"].ToString();
        }
    }
}
