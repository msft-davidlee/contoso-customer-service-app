using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Threading.Tasks;

namespace DemoWebsite.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "AzureAD:Scopes")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IConfiguration _configuration;

        public IndexModel(
            ILogger<IndexModel> logger,
            ITokenAcquisition tokenAcquisition,
            IConfiguration configuration)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
            _configuration = configuration;
        }

        public async Task OnGet()
        {
            // This should be cached internally but needs to be invoked here nevertheless.
            await _tokenAcquisition.GetAccessTokenForUserAsync(new string[]
            {
                _configuration["AzureAD:Scopes"]
            });
        }
    }
}
