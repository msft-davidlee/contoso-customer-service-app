namespace DemoMemberPortal.Core
{
    public interface IPointsService
    {
        Task<int> GetPoints(string memberId);
    }
}