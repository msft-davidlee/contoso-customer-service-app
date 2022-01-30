using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public class ServiceBusShippingRepository : IShippingRepository, IHealthCheck
    {
        private readonly IConfiguration _configuration;
        public ServiceBusShippingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ServiceBusClient GetServiceBusClient()
        {
            string connectionString = _configuration["QueueConnectionString"];
            if (connectionString.StartsWith("FilePath="))
            {
                string filePath = connectionString.Split('=')[1];
                connectionString = File.ReadAllText(filePath);
            }

            return new ServiceBusClient(connectionString);
        }

        public async Task<int> Add(Guid orderId)
        {
            var queueClient = GetServiceBusClient();

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

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = GetServiceBusClient();
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }
        }
    }
}
