using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class HttpOrderService : MicroserviceBase, IOrderService
    {        
        private readonly IRewardItemService _rewardItemService;

        public HttpOrderService(IConfiguration configuration, IRewardItemService rewardItemService,
            HttpClient httpClient, ITokenAcquisition tokenAcquisition) : base(tokenAcquisition, configuration, httpClient)
        {            
            _rewardItemService = rewardItemService;
        }

        public async Task<OrderResponse> PlaceOrder(string productId, string memberId)
        {
            var uri = $"{GetBaseUri()}order";
            var content = new StringContent(JsonConvert.SerializeObject(new { productId, memberId }));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await (await GetHttpClient()).PostAsync(new Uri(uri), content);
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<OrderResponse>(await response.Content.ReadAsStringAsync());

            throw new ApplicationException("Return payload is not valid.");
        }

        public async Task<IEnumerable<Order>> ListOrders(string memberId)
        {
            var uri = $"{GetBaseUri()}order?memberId={memberId}";
            var response = await (await GetHttpClient()).GetAsync(new Uri(uri));
            response.EnsureSuccessStatusCode();

            var orders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());

            foreach (var order in orders)
            {
                order.ProductName = (await _rewardItemService.GetRewardItem(order.ProductId)).Name;
            }

            return orders;
        }

        protected override string GetUriConfigName()
        {
            return "PartnerAPIUri";
        }
    }
}
