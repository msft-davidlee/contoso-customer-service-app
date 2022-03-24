namespace DemoCustomerServiceMember.Core
{
    public interface IAlternateMemberIdService
    {
        Task<string> GetMemberId(string alternateMemberId, string authorization);
    }
}
