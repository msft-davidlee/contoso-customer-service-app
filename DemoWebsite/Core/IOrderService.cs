using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IOrderService
    {
        Task<OrderResponse> PlaceOrder(string productId, string memberId);
        Task<IEnumerable<Order>> ListOrders(string memberId);
    }
}
