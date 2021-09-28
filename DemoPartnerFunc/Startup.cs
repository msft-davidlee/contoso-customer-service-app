using DemoPartnerCore;
using DemoPartnerCore.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(DemoPartnerFunc.Startup))]
namespace DemoPartnerFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
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
        }
    }
}
