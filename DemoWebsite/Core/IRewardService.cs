using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IRewardService
    {
        Task<RewardResult> Redeem(string memberId, string rewardItemId);
    }
}
