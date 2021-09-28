using DemoWebsite.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IRewardItemService
    {
        Task<IEnumerable<RewardItem>> GetRewardItems(int? points);
        Task AddRewardItems(RewardItem RewardItem);
        Task<RewardItem> GetRewardItem(string rewardItemId);
    }
}
