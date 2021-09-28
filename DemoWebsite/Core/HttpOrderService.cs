using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class HttpOrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IRewardItemService _rewardItemService;
        private static readonly HttpClient _client = new HttpClient();

        public HttpOrderService(IConfiguration configuration, IRewardItemService rewardItemService)
        {
            _configuration = configuration;
            _rewardItemService = rewardItemService;
        }

        public async Task<OrderResponse> PlaceOrder(string productId, string memberId)
        {
            var partnerApiUri = _configuration["PartnerAPIUri"];
            if (!string.IsNullOrEmpty(partnerApiUri))
            {
                var uri = $"{partnerApiUri}/order";
                var content = new StringContent(JsonConvert.SerializeObject(new { productId, memberId }));

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _client.PostAsync(new Uri(uri), content);
                response.EnsureSuccessStatusCode();

                return JsonConvert.DeserializeObject<OrderResponse>(await response.Content.ReadAsStringAsync());

                throw new ApplicationException("Return payload is not valid.");
            }

            throw new ApplicationException("PartnerAPIUri is not configured.");
        }

        public async Task<IEnumerable<Order>> ListOrders(string memberId)
        {
            var partnerApiUri = _configuration["PartnerAPIUri"];
            if (!string.IsNullOrEmpty(partnerApiUri))
            {
                var uri = $"{partnerApiUri}/order?memberId={memberId}";
                var response = await _client.GetAsync(new Uri(uri));
                response.EnsureSuccessStatusCode();

                var orders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());

                foreach (var order in orders)
                {
                    order.ProductName = (await _rewardItemService.GetRewardItem(order.ProductId)).Name;
                }

                return orders;
            }

            throw new ApplicationException("PartnerAPIUri is not configured.");
        }
    }
}
