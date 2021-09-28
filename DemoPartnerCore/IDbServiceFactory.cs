using DemoPartnerCore.Models;

namespace DemoPartnerCore
{
    public interface IDbServiceFactory
    {
        AppDbContext GetDbContext();
    }
}
