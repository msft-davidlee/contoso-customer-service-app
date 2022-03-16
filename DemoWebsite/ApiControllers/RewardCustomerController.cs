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
        private readonly IRewardCustomerPointsService _rewardCustomerPointsService;
        private readonly IMemberService _memberService;

        public RewardCustomerController(
            IAlternateIdService alternateIdService,
           IRewardCustomerPointsService rewardCustomerPointsService,
            IMemberService tokenAcquisition)
        {
            _alternateIdService = alternateIdService;
            _rewardCustomerPointsService = rewardCustomerPointsService;
            _memberService = tokenAcquisition;
        }

        public async Task<List<RewardCustomerResponse>> GetRewardCustomers(string memberId, string firstName, string lastName, bool? useAlternateId)
        {
            List<RewardCustomer> customers;
            if (!string.IsNullOrEmpty(memberId))
            {
                if (useAlternateId == true)
                {
                    memberId = await _alternateIdService.GetMemberIdAsync(memberId);
                }

                customers = new List<RewardCustomer> { await _memberService.GetRewardCustomer(memberId) };
            }
            else
            {
                if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                {
                    customers = (await _memberService.GetRewardCustomers(firstName, lastName)).ToList();
                }
                else
                {
                    customers = (await _memberService.GetRewardCustomers()).ToList();
                }
            }

            var responses = new List<RewardCustomerResponse>();

            // Populate points
            foreach (var customer in customers)
            {
                var response = new RewardCustomerResponse
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    MemberId = customer.MemberId
                };

                response.Points = await _rewardCustomerPointsService.GetPoints(response.MemberId);
                responses.Add(response);
            }

            return responses;
        }
    }
}
