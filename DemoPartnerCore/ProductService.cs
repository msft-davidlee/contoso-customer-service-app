using DemoPartnerCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public interface IProductService
    {
        Task<Product> GetProduct(string productId);
    }

    public class ProductService : IProductService, IHealthCheck
    {
        private readonly IDbServiceFactory _dbService;
        public ProductService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbService.GetDbContext().Database.ExecuteSqlRawAsync("SELECT 1");
                return HealthCheckResult.Healthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        public async Task<Product> GetProduct(string productId)
        {
            return await _dbService.GetDbContext().Products.SingleOrDefaultAsync(x => x.Id == productId);
        }
    }
}
