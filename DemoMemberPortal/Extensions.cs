using System.Security.Claims;

namespace DemoMemberPortal
{
    public static class Extensions
    {
        public static string DisplayName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
        }

        public static string MemberId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("extension_MemberId");
        }

        public static string Email(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("emails");
        }
    }
}
