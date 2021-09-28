using DemoPartnerCore;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DemoStorageShippingFunc
{
    public class ShippingFunction
    {
        private readonly IShippingService _shippingService;

        public ShippingFunction(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [FunctionName("Shipping")]
        public async Task Run([QueueTrigger("%QueueName%", Connection = "Connection")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"Queue trigger function processed: {myQueueItem}");

            await _shippingService.Process(Guid.Parse(myQueueItem));
        }
    }
}
