using DemoCustomerServiceAltId.Core;
using DemoCustomerServiceAltId.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DemoCustomerServiceAltId.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlternateIdController : ControllerBase
    {
        private readonly IAlternateIdService _alternateService;

        public AlternateIdController(IAlternateIdService alternateService)
        {
            _alternateService = alternateService;
        }

        [HttpGet("{id}")]
        public async Task<AlternateId> Get([FromRoute] string id)
        {
            return await _alternateService.Get(id);
        }
    }
}
