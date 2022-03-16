using DemoWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IRewardCustomerPointsService : IHealthCheck
    {
        Task<int> GetPoints(string memberId);
        Task Update(string memberId, int points);
    }

    public class RewardCustomerPointsService : IRewardCustomerPointsService
    {
        private readonly IDbServiceFactory _dbService;

        public RewardCustomerPointsService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }
        public async Task<int> GetPoints(string memberId)
        {
            return (await GetEntity(memberId)).Points;
        }

        private async Task<RewardCustomerPoints> GetEntity(string memberId)
        {
            return await _dbService.GetDbContext().RewardCustomerPoints.SingleAsync(x => x.MemberId == memberId);
        }

        public async Task Update(string memberId, int points)
        {
            var entity = await GetEntity(memberId);
            entity.Points = points;
            await _dbService.GetDbContext().SaveChangesAsync();
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
    }
}
