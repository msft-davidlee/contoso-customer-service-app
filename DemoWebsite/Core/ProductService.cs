using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IProductService
    {
        Task<bool> IsInStock(string productId);
    }

    public class ProductService : IProductService, IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private static readonly HttpClient _client = new HttpClient();

        public ProductService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> IsInStock(string productId)
        {
            var partnerApiUri = _configuration["PartnerAPIUri"];
            if (!string.IsNullOrEmpty(partnerApiUri))
            {
                var uri = $"{partnerApiUri}/product?productId={productId}";
                var response = await _client.GetAsync(new Uri(uri));
                response.EnsureSuccessStatusCode();

                var o = JObject.Parse(await response.Content.ReadAsStringAsync());
                if (bool.TryParse(o["active"].ToString(), out bool active))
                {
                    if (!active)
                    {
                        return false;
                    }

                    if (int.TryParse(o["quantity"].ToString(), out int quantity))
                    {
                        return quantity > 0;
                    }
                }

                throw new ApplicationException("Return payload is not valid.");
            }

            throw new ApplicationException("PartnerAPIUri is not configured.");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var partnerApiUri = _configuration["PartnerAPIUri"];
            if (string.IsNullOrEmpty(partnerApiUri))
            {
                return HealthCheckResult.Unhealthy();
            }

            var uri = $"{partnerApiUri}/health";

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
