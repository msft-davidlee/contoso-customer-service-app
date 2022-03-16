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
        private readonly IAlternateIdService _alternateIdService;
        private readonly IMemberService _memberService;

        public RewardCustomerController(            
            IAlternateIdService alternateIdService,
            IMemberService tokenAcquisition)
        {            
            _alternateIdService = alternateIdService;
            _memberService = tokenAcquisition;
        }

        public async Task<List<RewardCustomer>> GetRewardCustomers(string memberId, string firstName, string lastName, bool? useAlternateId)
        {
            if (!string.IsNullOrEmpty(memberId))
            {
                if (useAlternateId == true)
                {
                    memberId = await _alternateIdService.GetMemberIdAsync(memberId);
                }

                return new List<RewardCustomer> { await _memberService.GetRewardCustomer(memberId) };
            }

            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            {
                return (await _memberService.GetRewardCustomers(firstName, lastName)).ToList();
            }

            return (await _memberService.GetRewardCustomers()).ToList();
        }
    }
}
