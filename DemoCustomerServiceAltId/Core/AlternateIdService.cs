using DemoCustomerServiceAltId.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DemoCustomerServiceAltId.Core
{
    public class AlternateIdService : IAlternateIdService
    {
        private readonly IDbServiceFactory _dbService;

        public AlternateIdService(IDbServiceFactory dbService)
        {
            _dbService = dbService;
        }

        public Task<AlternateId> Get(string id)
        {
            return _dbService.GetDbContext().AlternateIds.SingleOrDefaultAsync(x => x.Value == id);
        }
    }
}
