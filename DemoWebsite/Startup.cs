using DemoWebsite.Core;
using DemoWebsite.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.HttpOverrides;

namespace DemoWebsite
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

            if (IsAuth())
            {
                // see: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });

                if (ConfigureForB2C())
                {
                    services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd")
                        .EnableTokenAcquisitionToCallDownstreamApi()
                        .AddInMemoryTokenCaches();
                }
                else
                {
                    services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                        .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                        .EnableTokenAcquisitionToCallDownstreamApi()
                        .AddInMemoryTokenCaches();
                }

                services.AddControllersWithViews(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                });
            }

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
            services.AddTransient<IRewardCustomerService, RewardCustomerService>();
            services.AddTransient<IRewardItemService, RewardItemService>();
            services.AddTransient<IRewardService, RewardService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, HttpOrderService>();
            services.AddTransient<IAlternateIdService, HttpAlternateIdService>();
            var svc = services.AddRazorPages();

            if (IsAuth())
            {
                svc.AddMicrosoftIdentityUI();

                if (ConfigureForB2C())
                {
                    services.AddOptions();
                    services.Configure<OpenIdConnectOptions>(Configuration.GetSection("AzureAd"));
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseForwardedHeaders();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            if (IsAuth()) app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }

        private bool IsAuth()
        {
            bool result;
            bool.TryParse(Configuration["EnableAuth"], out result);
            return result;
        }

        private bool ConfigureForB2C()
        {
            bool result;
            bool.TryParse(Configuration["ConfigureForB2C"], out result);
            return result;
        }
    }
}
