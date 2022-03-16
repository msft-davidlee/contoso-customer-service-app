using DemoCustomerServiceMember.Core;
using DemoCustomerServiceMember.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DemoCustomerServiceMember.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class MembersController : ControllerBase
    {
        private readonly IRewardCustomerService _rewardCustomerService;

        public MembersController(IRewardCustomerService rewardCustomerService)
        {
            _rewardCustomerService = rewardCustomerService;
        }

        [HttpGet]
        public async Task<IEnumerable<RewardCustomer>> Get(string? memberId, string? firstName, string? lastName)
        {
            if (!string.IsNullOrEmpty(memberId))
            {
                return new[] { await _rewardCustomerService.GetRewardCustomer(memberId) };
            }

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                return await _rewardCustomerService.GetRewardCustomers(firstName, lastName);
            }

            return await _rewardCustomerService.GetRewardCustomers();
        }
    }
}