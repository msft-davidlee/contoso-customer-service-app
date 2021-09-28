using DemoWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class RewardItemService : IRewardItemService
    {
        private readonly IDbServiceFactory _dbService;
        public RewardItemService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }

        public async Task<IEnumerable<RewardItem>> GetRewardItems(int? points)
        {
            if (points.HasValue)
            {
                return await _dbService.GetDbContext().RewardItems.Where(x => x.Points <= points.Value).ToListAsync();
            }
            return await _dbService.GetDbContext().RewardItems.ToListAsync();
        }

        public async Task AddRewardItems(RewardItem RewardItem)
        {
            await _dbService.GetDbContext().RewardItems.AddAsync(RewardItem);
        }

        public async Task<RewardItem> GetRewardItem(string rewardItemId)
        {
            return await _dbService.GetDbContext().RewardItems.SingleOrDefaultAsync(x => x.Id == rewardItemId);
        }
    }
}
