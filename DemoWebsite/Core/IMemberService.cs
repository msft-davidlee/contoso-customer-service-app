using DemoWebsite.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IMemberService
    {
        Task<RewardCustomer> GetRewardCustomer(string memberId);
        Task<IEnumerable<RewardCustomer>> GetRewardCustomers(string firstName, string lastName);
        Task<IEnumerable<RewardCustomer>> GetRewardCustomers();
    }
}
