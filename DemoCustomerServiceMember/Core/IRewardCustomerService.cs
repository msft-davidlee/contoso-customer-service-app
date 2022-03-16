using DemoCustomerServiceMember.Models;

namespace DemoCustomerServiceMember.Core
{
    public interface IRewardCustomerService
    {
        Task<RewardCustomer> GetRewardCustomer(string memberId);
        Task<IEnumerable<RewardCustomer>> GetRewardCustomers(string firstName, string lastName);
        Task<IEnumerable<RewardCustomer>> GetRewardCustomers();
        Task AddRewardCustomer(RewardCustomer RewardCustomer);
        Task UpdateRewardCustomer();
    }
}
