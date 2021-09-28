using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public class RewardService : IRewardService
    {
        private readonly IRewardCustomerService _rewardCustomerService;
        private readonly IRewardItemService _rewardItemService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public RewardService(
            IRewardCustomerService rewardCustomerService,
            IRewardItemService rewardItemService,
            IProductService productService,
            IOrderService orderService)
        {
            _rewardCustomerService = rewardCustomerService;
            _rewardItemService = rewardItemService;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<RewardResult> Redeem(string memberId, string rewardItemId)
        {
            var result = new RewardResult();
            if (!string.IsNullOrEmpty(memberId))
            {
                var customer = await _rewardCustomerService.GetRewardCustomer(memberId);
                if (customer != null)
                {
                    var reward = await _rewardItemService.GetRewardItem(rewardItemId);
                    if (reward != null)
                    {
                        if (reward.Points > customer.Points)
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
                                    customer.Points -= reward.Points;
                                    await _rewardCustomerService.UpdateRewardCustomer();
                                    result.Message = $"Member has {customer.Points} points remaining. Order Id = {order.OrderId.Value}. Estimated Time = {order.EstimatedSeconds} seconds.";
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
