using DemoPartnerCore.Models;

namespace DemoPartnerCore
{
    public class DbServiceFactory : IDbServiceFactory
    {
        private readonly AppDbContext _appDbContext;
        public DbServiceFactory(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public AppDbContext GetDbContext()
        {
            return _appDbContext;
        }
    }
}
