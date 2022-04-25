using DemoPartnerCore;
using DemoPartnerCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoPartnerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<OrderResponse> Post(OrderRequest orderRequest)
        {
            return await _orderService.PlaceOrder(orderRequest);
        }

        [HttpGet]
        public async Task<List<Order>> Get(string memberId)
        {
            return (await _orderService.ListOrders(memberId)).ToList();
        }

        [Route("orderId/{orderId}")]
        [HttpGet]
        public async Task<Order> GetByOrderId(Guid orderId)
        {
            return await _orderService.GetOrder(orderId);
        }
    }
}
