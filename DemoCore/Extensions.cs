using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DemoCore
{
    public static class Extensions
    {
        private const string VersionFileName = "version.txt";

        public static void AddManagedConfiguration<T>(this IServiceCollection services)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string version = Assembly.GetAssembly(typeof(T)).GetName().Version.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            services.AddSingleton<IManagedConfiguration>(new ManagedConfiguration(version, Environment.MachineName));
        }

        public static string GetConnectionString(this IConfiguration configuration)
        {
            var dbDataSource = configuration["DbSource"];
            if (!string.IsNullOrEmpty(dbDataSource))
            {
                var dbName = configuration["DbName"];
                var userId = configuration["DbUserId"];
                var dbPassword = configuration["DbPassword"];
                return $"Data Source={dbDataSource};Initial Catalog={dbName}; User Id={userId};Password={dbPassword}";
            }

            string connectionString = configuration["DbConnectionString"];
            if (connectionString.StartsWith("FilePath="))
            {
                string filePath = connectionString.Split('=')[1];
                return File.ReadAllText(filePath);
            }

            return connectionString;
        }

        public static void EnableApplicationInsights(this IServiceCollection services, IConfiguration configuration)
        {
            // See: https://github.com/microsoft/ApplicationInsights-Kubernetes
            var appInsightsConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(appInsightsConnectionString))
            {
                services.AddApplicationInsightsTelemetry();

                var isAzureFunction = !string.IsNullOrEmpty(configuration["FUNCTIONS_WORKER_RUNTIME"]);

                // Currently, this will not work for Azure Functions
                if (!isAzureFunction) services.AddApplicationInsightsKubernetesEnricher();
            }
        }
    }
}