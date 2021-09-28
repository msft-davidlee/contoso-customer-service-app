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
    public class RewardCustomerController : ControllerBase
    {
        private readonly IRewardCustomerService _rewardCustomerService;
        private readonly IAlternateIdService _alternateIdService;

        public RewardCustomerController(IRewardCustomerService rewardCustomerService, IAlternateIdService alternateIdService)
        {
            _rewardCustomerService = rewardCustomerService;
            _alternateIdService = alternateIdService;
        }

        public async Task<List<RewardCustomer>> GetRewardCustomers(string memberId, string firstName, string lastName, bool? useAlternateId)
        {
            if (!string.IsNullOrEmpty(memberId))
            {
                if (useAlternateId == true)
                {
                    memberId = await _alternateIdService.GetMemberIdAsync(memberId);
                }

                return new List<RewardCustomer> { await _rewardCustomerService.GetRewardCustomer(memberId) };
            }

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                return (await _rewardCustomerService.GetRewardCustomers(firstName, lastName)).ToList();
            }

            return (await _rewardCustomerService.GetRewardCustomers()).ToList();
        }
    }
}
