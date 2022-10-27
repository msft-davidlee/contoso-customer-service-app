using DemoPartnerCore;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DemoServiceBusShippingFunc
{
    public class ShippingFunction
    {
        private const string PingTest = "ping";
        private readonly IShippingService _shippingService;

        public ShippingFunction(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [FunctionName("Shipping")]
        public async Task Run([ServiceBusTrigger("%QueueName%", Connection = "Connection")] string myQueueItem, ILogger log)
        {
            if (myQueueItem.StartsWith(PingTest))
            {
                log.LogInformation($"Ping test mesage: {myQueueItem}");
                return;
            }

            await _shippingService.Process(Guid.Parse(myQueueItem));
        }
    }
}
