using DemoPartnerCore;
using DemoPartnerCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace DemoPartnerAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = Configuration["DbConnectionString"];
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
            services.AddTransient<IDbServiceFactory, DbServiceFactory>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IShippingService, ShippingService>();

            string shippingRepositoryType = Configuration["ShippingRepositoryType"];
            switch (shippingRepositoryType)
            {
                case "Storage":
                    services.AddTransient<IShippingRepository, StorageQueueShippingRepository>();
                    break;

                case "ServiceBus":
                    services.AddTransient<IShippingRepository, ServiceBusShippingRepository>();
                    break;

                default:
                    throw new ApplicationException("ShippingRepositoryType is not configured.");
            }

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // TODO: Make this configurable.
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
