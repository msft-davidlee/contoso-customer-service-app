using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DemoPartnerCore;

namespace DemoPartnerFunc
{
    public class ProductFunction
    {
        private readonly IProductService _productService;

        public ProductFunction(IProductService productService)
        {
            _productService = productService;
        }

        [FunctionName("Product")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Product processed a request.");

            string productId = req.Query["productId"];

            return new OkObjectResult(await _productService.GetProduct(productId));
        }
    }
}
