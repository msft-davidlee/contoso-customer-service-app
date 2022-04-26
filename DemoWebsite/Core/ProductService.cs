using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IProductService
    {
        Task<bool> IsInStock(string productId);
    }

    public class ProductService : MicroserviceBase, IProductService, IHealthCheck
    {
        public ProductService(IConfiguration configuration, HttpClient httpClient, ITokenAcquisition tokenAcquisition)
            : base(tokenAcquisition, configuration, httpClient)
        {

        }

        public async Task<bool> IsInStock(string productId)
        {
            var uri = $"{GetBaseUri()}product?productId={productId}";
            var response = await (await GetHttpClient()).GetAsync(new Uri(uri));
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

        protected override string GetUriConfigName()
        {
            return "PartnerAPIUri";
        }
    }
}
