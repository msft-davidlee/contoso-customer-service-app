using DemoPartnerCore.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public interface IOrderService
    {
        Task<OrderResponse> PlaceOrder(OrderRequest orderRequest);
        Task<Order> GetOrder(Guid orderId);

        Task<IEnumerable<Order>> ListOrders(string memberId);

        Task SaveChanges();
    }
}
