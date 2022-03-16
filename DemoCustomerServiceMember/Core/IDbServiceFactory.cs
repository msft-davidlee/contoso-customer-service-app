namespace DemoCustomerServiceMember.Core
{
    public interface IDbServiceFactory
    {
        AppDbContext GetDbContext();
    }
}
