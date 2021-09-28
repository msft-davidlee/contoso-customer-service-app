using DemoPartnerCore;
using DemoPartnerCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoPartnerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<Product> Get(string productId)
        {
            return await _productService.GetProduct(productId);
        }
    }
}
