using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public class ShippingService : IShippingService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public ShippingService(IOrderService orderService, ILogger<ShippingService> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task Process(Guid orderId)
        {
            var order = await _orderService.GetOrder(orderId);
            if (order == null)
            {
                _logger.LogInformation($"Order {orderId} is not found.");
                return;
            }

            order.Shipped = DateTime.UtcNow;

            TimeSpan t = DateTime.Now - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            order.TrackingNumber = secondsSinceEpoch.ToString();

            await _orderService.SaveChanges();
        }
    }
}
