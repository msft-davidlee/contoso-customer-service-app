using DemoWebsite.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoWebsite.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;

        public RewardController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpPost]
        public async Task<RewardResult> RedeemReward(RedeemRequest redeemRequest)
        {
            return await _rewardService.Redeem(redeemRequest.MemberId, redeemRequest.RewardItemId);
        }
    }

    public class RedeemRequest
    {
        public string MemberId { get; set; }
        public string RewardItemId { get; set; }
    }
}
