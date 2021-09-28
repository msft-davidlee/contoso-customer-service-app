using DemoPartnerCore;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DemoServiceBusShippingFunc
{
    public class ShippingFunction
    {
        private readonly IShippingService _shippingService;

        public ShippingFunction(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [FunctionName("Shipping")]
        public async Task Run([ServiceBusTrigger("%QueueName%", Connection = "Connection")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"ServiceBus queue trigger function processed message: {myQueueItem}");

            await _shippingService.Process(Guid.Parse(myQueueItem));
        }
    }
}
