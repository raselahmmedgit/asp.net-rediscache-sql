using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace GCSideLoading.Core.Util
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetLoggedInUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return loggedInUserId;
        }

        public static string GetLoggedInUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            return principal.FindFirstValue(ClaimTypes.Email);
        }
        public static string GetLoggedInUserDisplayName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            return principal.FindFirstValue(ClaimTypes.GivenName);
        }
    }
}
