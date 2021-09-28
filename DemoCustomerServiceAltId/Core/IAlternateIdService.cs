using DemoCustomerServiceAltId.Models;
using System.Threading.Tasks;

namespace DemoCustomerServiceAltId.Core
{
    public interface IAlternateIdService
    {
        Task<AlternateId> Get(string id);
    }
}
