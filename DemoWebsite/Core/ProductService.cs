using Microsoft.Extensions.Configuration;
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

    public class ProductService : IProductService
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
    }
}
