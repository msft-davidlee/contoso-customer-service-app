using System;
using System.Security.Claims;

namespace DemoWebsite.Core
{
    public enum UserRoles
    {
        Any,
        Agent,
        Supervisor
    }
    public static class Extensions
    {
        public static bool IsB2C(this ClaimsPrincipal user)
        {
            return user.HasClaim(x => x.Type == "tfp" && x.Value == "B2C_1_CS_Rewards");
        }

        public static bool ShouldDisplayForRole(this ClaimsPrincipal user, UserRoles role)
        {
            if (bool.TryParse(Environment.GetEnvironmentVariable("EnableAuth"), out bool result) && result)
            {
                return user.IsInRole($"CS.{role}");
            }
            return true;
        }

        public static string DisplayName(this ClaimsPrincipal user)
        {
            return user.FindFirst(x => x.Type == "name").Value;
        }
    }
}
