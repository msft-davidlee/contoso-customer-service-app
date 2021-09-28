using System;
using System.Threading.Tasks;

namespace DemoPartnerCore
{
    public interface IShippingService
    {
        Task Process(Guid orderId);
    }
}
