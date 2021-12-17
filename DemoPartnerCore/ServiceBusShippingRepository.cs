using Azure.Messaging.ServiceBus;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public class ServiceBusShippingRepository : IShippingRepository
    {
        private readonly IConfiguration _configuration;
        public ServiceBusShippingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> Add(Guid orderId)
        {
            string connectionString = _configuration["QueueConnectionString"];
            if (connectionString.StartsWith("FilePath="))
            {
                string filePath = connectionString.Split('=')[1];
                connectionString = File.ReadAllText(filePath);
            }

            var queueClient = new ServiceBusClient(connectionString);
            var sender = queueClient.CreateSender(_configuration["QueueName"]);
            if (IsQueueDeplayDisabled())
            {
                await sender.SendMessageAsync(new ServiceBusMessage(orderId.ToString()));

                return 0;
            }

            // We want to simulate backend shipping processing takes some time, where we have robots trying to fulfill a shipment. 
            // This can take anywhere from 20 seconds to 1 minute.
            var rand = new Random();
            int result = rand.Next(20, 60);
            await sender.SendMessageAsync(new ServiceBusMessage(orderId.ToString()) { ScheduledEnqueueTime = DateTime.UtcNow.AddSeconds(TimeSpan.FromSeconds(result).TotalSeconds) });

            return result;
        }

        private bool IsQueueDeplayDisabled()
        {
            var disableQueueDelayStr = _configuration["DisableQueueDelay"];

            if (!string.IsNullOrEmpty(disableQueueDelayStr))
            {
                if (bool.TryParse(disableQueueDelayStr, out bool result))
                {
                    return result;
                }
            }
            return false;
        }
    }
}
