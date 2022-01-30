using DemoCustomerServiceAltId.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace DemoCustomerServiceAltId.Core
{
    public class AlternateIdService : IAlternateIdService, IHealthCheck
    {
        private readonly IDbServiceFactory _dbService;

        public AlternateIdService(IDbServiceFactory dbService)
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

        public Task<AlternateId> Get(string id)
        {
            return _dbService.GetDbContext().AlternateIds.SingleOrDefaultAsync(x => x.Value == id);
        }
    }
}
