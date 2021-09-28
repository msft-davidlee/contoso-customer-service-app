using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public class StorageQueueShippingRepository : IShippingRepository
    {
        private readonly IConfiguration _configuration;

        public StorageQueueShippingRepository(IConfiguration configuration)
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

            var queueClient = new QueueClient(connectionString, _configuration["QueueName"]);
            var base64message = Convert.ToBase64String(Encoding.UTF8.GetBytes(orderId.ToString()));

            if (IsQueueDeplayDisabled())
            {
                await queueClient.SendMessageAsync(base64message);

                return 0;
            }

            // We want to simulate backend shipping processing takes some time, where we have robots trying to fulfill a shipment. 
            // This can take anywhere from 20 seconds to 1 minute.
            var rand = new Random();
            int result = rand.Next(20, 60);
            await queueClient.SendMessageAsync(base64message, TimeSpan.FromSeconds(result));

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
