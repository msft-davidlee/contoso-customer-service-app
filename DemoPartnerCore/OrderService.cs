using DemoPartnerCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public class OrderService : IOrderService
    {
        private readonly IDbServiceFactory _dbService;
        private readonly IProductService _productService;
        private readonly IShippingRepository _shippingRepository;

        public OrderService(IDbServiceFactory dbService, IProductService productService, IShippingRepository shippingRepository)
        {
            _dbService = dbService;
            _productService = productService;
            _shippingRepository = shippingRepository;
        }

        public async Task<Order> GetOrder(Guid orderId)
        {
            return await _dbService.GetDbContext().Orders.SingleOrDefaultAsync(x => x.Id == orderId);
        }

        public async Task SaveChanges()
        {
            await _dbService.GetDbContext().SaveChangesAsync();
        }

        public async Task<OrderResponse> PlaceOrder(OrderRequest orderRequest)
        {
            var orderResponse = new OrderResponse();
            var product = await _productService.GetProduct(orderRequest.ProductId);
            if (product.Active && product.Quantity > 0)
            {
                product.Quantity -= 1;

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    MemberId = orderRequest.MemberId,
                    ProductId = orderRequest.ProductId
                };
                await _dbService.GetDbContext().Orders.AddAsync(order);

                await _dbService.GetDbContext().SaveChangesAsync();

                orderResponse.EstimatedSeconds = await _shippingRepository.Add(order.Id);
                orderResponse.OrderId = order.Id;
                orderResponse.Success = true;
            }
            else
            {
                orderResponse.Message = "Product is either inactive or we do not have sufficient quantity.";
            }

            return orderResponse;
        }

        public async Task<IEnumerable<Order>> ListOrders(string memberId)
        {
            return await _dbService.GetDbContext().Orders.Where(x => x.MemberId == memberId)
                .OrderByDescending(x => x.Created)
                .Take(10)
                .ToListAsync();
        }
    }
}
