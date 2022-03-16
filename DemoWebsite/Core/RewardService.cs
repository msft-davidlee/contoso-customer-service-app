using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class RewardService : IRewardService
    {
        private readonly IRewardCustomerPointsService _rewardCustomerPointsService;
        private readonly IMemberService _memberService;
        private readonly IRewardItemService _rewardItemService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public RewardService(
            IRewardCustomerPointsService rewardCustomerPointsService,
            IMemberService memberService,
            IRewardItemService rewardItemService,
            IProductService productService,
            IOrderService orderService)
        {
            _rewardCustomerPointsService = rewardCustomerPointsService;
            _memberService = memberService;
            _rewardItemService = rewardItemService;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<RewardResult> Redeem(string memberId, string rewardItemId)
        {
            var result = new RewardResult();
            if (!string.IsNullOrEmpty(memberId))
            {
                var customer = await _memberService.GetRewardCustomer(memberId);
                if (customer != null)
                {
                    var customerPoints = await _rewardCustomerPointsService.GetPoints(memberId);
                    var reward = await _rewardItemService.GetRewardItem(rewardItemId);
                    if (reward != null)
                    {
                        if (reward.Points > customerPoints)
                        {
                            result.Message = "Member does not have enough points for this reward item.";
                        }
                        else
                        {
                            // Check if Product is in stock
                            if (await _productService.IsInStock(rewardItemId))
                            {
                                // Place Order
                                var order = await _orderService.PlaceOrder(rewardItemId, memberId);
                                if (order.Success)
                                {
                                    // Redeem
                                    customerPoints -= reward.Points;
                                    await _rewardCustomerPointsService.Update(memberId, customerPoints);
                                    result.Message = $"Member has {customerPoints} points remaining. Order Id = {order.OrderId.Value}. Estimated Time = {order.EstimatedSeconds} seconds.";
                                    result.Success = true;
                                }
                                else
                                {
                                    result.Message = order.Message;
                                }

                            }
                            else
                            {
                                result.Message = "Sorry, product is not in stock, please choose a different reward.";
                            }
                        }
                    }
                    else
                    {
                        result.Message = "Reward item is not found.";
                    }
                }
                else
                {
                    result.Message = $"Member Id {memberId} is not found.";
                }
            }
            else
            {
                result.Message = "Member Id is missing.";
            }

            return result;
        }
    }
}
