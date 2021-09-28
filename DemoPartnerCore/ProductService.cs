using DemoPartnerCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public interface IProductService
    {
        Task<Product> GetProduct(string productId);
    }

    public class ProductService : IProductService
    {
        private readonly IDbServiceFactory _dbService;
        public ProductService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }

        public async Task<Product> GetProduct(string productId)
        {
            return await _dbService.GetDbContext().Products.SingleOrDefaultAsync(x => x.Id == productId);
        }
    }
}
