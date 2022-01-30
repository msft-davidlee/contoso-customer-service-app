using DemoWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class RewardCustomerService : IRewardCustomerService, IHealthCheck
    {
        private readonly IDbServiceFactory _dbService;
        public RewardCustomerService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }

        public async Task<RewardCustomer> GetRewardCustomer(string memberId)
        {
            return await _dbService.GetDbContext().RewardCustomers.SingleOrDefaultAsync(x => x.MemberId == memberId);
        }

        public async Task<IEnumerable<RewardCustomer>> GetRewardCustomers(string firstName, string lastName)
        {
            return await _dbService.GetDbContext().RewardCustomers.Where(x => x.FirstName == firstName && x.LastName == lastName).ToListAsync();
        }

        public async Task<IEnumerable<RewardCustomer>> GetRewardCustomers()
        {
            return await _dbService.GetDbContext().RewardCustomers.ToListAsync();
        }

        public async Task AddRewardCustomer(RewardCustomer RewardCustomer)
        {
            await _dbService.GetDbContext().RewardCustomers.AddAsync(RewardCustomer);
        }

        public async Task UpdateRewardCustomer()
        {
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
