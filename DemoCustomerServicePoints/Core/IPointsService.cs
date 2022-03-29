using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DemoCustomerServicePoints.Core
{
    public interface IPointsService
    {
        Task<int> AddPoints(string memberId, int points);
        Task<int> GetPoints(string memberId);
    }

    public class PointsService : IPointsService, IHealthCheck
    {
        private readonly IDbServiceFactory _dbService;

        public PointsService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }

        public async Task<int> GetPoints(string memberId)
        {
            var ctx = _dbService.GetDbContext();

            var mem = await ctx.RewardCustomerPoints.SingleAsync(x => x.MemberId == memberId);
            return mem.Points;
        }

        public async Task<int> AddPoints(string memberId, int points)
        {
            var ctx = _dbService.GetDbContext();

            var memberPoints = await ctx.RewardCustomerPoints.SingleAsync(x => x.MemberId == memberId);
            memberPoints.Points += points;

            var total = memberPoints.Points;
            ctx.Update(memberPoints);

            await ctx.SaveChangesAsync();

            return total;
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
