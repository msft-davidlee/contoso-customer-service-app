using DemoMemberPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace DemoMemberPortal.Controllers
{
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "AzureAd:Scopes")]
    public class HomeController : Controller
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HomeController(ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var memberId = User.MemberId();
            var client = await GetHttpClient();
            var uri = $"{_configuration["MemberPointsUrl"]}api/points/member/{memberId}";
            var points = await client.GetFromJsonAsync<MemberPointModel>(uri);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ViewData["Points"] = points.TotalPoints;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return View();
        }

        protected async Task<HttpClient> GetHttpClient()
        {
            var token = await _tokenAcquisition.GetAccessTokenForAppAsync(_configuration["AzureAD:Scopes"]);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}