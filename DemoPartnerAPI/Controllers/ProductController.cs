using DemoPartnerCore;
using DemoPartnerCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoPartnerAPI.Controllers
{
    [Authorize]
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

        [HttpPost]
        public async Task<ProductUpdateResponseModel> Post(ProductUpdateModel product)
        {
            var qty = await _productService.AddQuantity(product.ProductId, product.Quantity);
            return new ProductUpdateResponseModel { Total = qty };
        }
    }

    public class ProductUpdateResponseModel
    {
        public int Total { get; set; }
    }

    public class ProductUpdateModel
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
