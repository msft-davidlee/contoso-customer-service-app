using DemoPartnerCore;
using DemoPartnerCore.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(DemoStorageShippingFunc.Startup))]
namespace DemoStorageShippingFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string value = Environment.GetEnvironmentVariable("Connection");
            if (!string.IsNullOrEmpty(value) && value.StartsWith("FilePath="))
            {
                string filePath = value.Split('=')[1];
                value = File.ReadAllText(filePath);
                Environment.SetEnvironmentVariable("Connection", value);
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
                if (connectionString.StartsWith("FilePath="))
                {
                    string filePath = connectionString.Split('=')[1];
                    connectionString = File.ReadAllText(filePath);
                }
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                    });
            });

            builder.Services.AddTransient<IDbServiceFactory, DbServiceFactory>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<IShippingService, ShippingService>();
            builder.Services.AddTransient<IShippingRepository, StorageQueueShippingRepository>();
        }
    }
}
