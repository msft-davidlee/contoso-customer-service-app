using DemoCustomerServiceMember.Core;
using DemoCustomerServiceMember.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DemoCustomerServiceMember.Controllers
{
    [Authorize]
    [Route("legacy/members")]
    [ApiController]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class LegacyMembersController : ControllerBase
    {
        private readonly IRewardCustomerService _rewardCustomerService;
        private readonly IAlternateMemberIdService _alternateMemberIdService;

        public LegacyMembersController(IRewardCustomerService rewardCustomerService,
            IAlternateMemberIdService alternateMemberIdService)
        {
            _rewardCustomerService = rewardCustomerService;
            _alternateMemberIdService = alternateMemberIdService;
        }

        [HttpGet("{alternateMemberId}")]
        public async Task<IEnumerable<RewardCustomer>> Get([FromRoute] string alternateMemberId)
        {
            var authorization = Request.Headers.Authorization.Single();
            var tokenParts = authorization.Split(' ');
            var memberId = await _alternateMemberIdService.GetMemberId(alternateMemberId, tokenParts[1]);
            return new[] { await _rewardCustomerService.GetRewardCustomer(memberId) };
        }
    }
}
