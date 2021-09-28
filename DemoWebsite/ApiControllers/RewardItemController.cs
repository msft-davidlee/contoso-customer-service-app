using DemoWebsite.Core;
using DemoWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebsite.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardItemController : ControllerBase
    {
        private readonly IRewardItemService _rewardItemService;

        public RewardItemController(IRewardItemService rewardItemService)
        {
            _rewardItemService = rewardItemService;
        }

        public async Task<List<RewardItem>> GetRewardItems([FromQuery] RewardItemQuery query)
        {
            if (!string.IsNullOrEmpty(query.Id))
                return new List<RewardItem> { await _rewardItemService.GetRewardItem(query.Id) };

            return (await _rewardItemService.GetRewardItems(query.Points)).ToList();
        }
    }
}
