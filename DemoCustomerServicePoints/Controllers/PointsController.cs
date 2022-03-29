using DemoCustomerServicePoints.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DemoCustomerServicePoints.ApiControllers
{
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    [Route("api/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly IPointsService _pointsService;

        public PointsController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        [HttpPost]
        public async Task<AwardPointsResult> Add(AwardPoints awardPoints)
        {
            var points = await _pointsService.AddPoints(awardPoints.MemberId, awardPoints.Points);
            return new AwardPointsResult { TotalPoints = points };
        }

        [Route("member/{memberId}")]
        [HttpGet]
        public async Task<AwardPointsResult> Get(string memberId)
        {
            var points = await _pointsService.GetPoints(memberId);
            return new AwardPointsResult { TotalPoints = points };
        }
    }

    public class AwardPointsResult
    {
        public int TotalPoints { get; set; }
    }

    public class AwardPoints
    {
        public string MemberId { get; set; }
        public int Points { get; set; }

        /// <summary>
        /// Relates to the transaction that qualifies for this add.
        /// </summary>
        public string TransactionId { get; set; }
    }
}
