using DemoMemberPortal.Core;
using DemoMemberPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;

namespace DemoMemberPortal.Controllers
{
    [Authorize(AuthenticationSchemes = "B2C")]
    public class HomeController : Controller
    {
        private readonly IPointsService _pointsService;

        public HomeController(IPointsService pointsService)
        {
            _pointsService = pointsService;
        }

        [AuthorizeForScopes(ScopeKeySection = "AzureAd:Scopes")]
        public async Task<IActionResult> Index()
        {
            ViewData["Points"] = await _pointsService.GetPoints(User.MemberId());
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}