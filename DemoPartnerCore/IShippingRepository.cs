using System;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public interface IShippingRepository
    {
        Task<int> Add(Guid orderId);
    }
}
