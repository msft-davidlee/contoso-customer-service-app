using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public class StorageQueueShippingRepository : IShippingRepository, IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public StorageQueueShippingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private QueueClient GetQueueClient()
        {
            string connectionString = _configuration["QueueConnectionString"];
            if (connectionString.StartsWith("FilePath="))
            {
                string filePath = connectionString.Split('=')[1];
                connectionString = File.ReadAllText(filePath);
            }

            return new QueueClient(connectionString, _configuration["QueueName"]);
        }

        public async Task<int> Add(Guid orderId)
        {
            var queueClient = GetQueueClient();

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

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var queueClient = GetQueueClient();
            try
            {
                await queueClient.GetPropertiesAsync();
                return HealthCheckResult.Healthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
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
