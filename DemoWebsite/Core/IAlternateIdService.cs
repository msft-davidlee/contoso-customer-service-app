using System.Threading.Tasks;

namespace DemoWebsite.Core
{
    public interface IAlternateIdService
    {
        Task<string> GetMemberIdAsync(string alternateId);
    }
}
