using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ERPCompanySystem.Attributes;
using System.Threading.Tasks;

namespace ERPCompanySystem.Authorization
{
    public class RoleAuthorizationHandler : AuthorizationHandler<CustomAuthorizeAttribute>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizeAttribute requirement)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Fail();
                return;
            }

            // Check JWT requirement
            if (requirement.RequireJwt)
            {
                var tokenClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (tokenClaim == null)
                {
                    context.Fail();
                    return;
                }
            }

            // Check Refresh Token requirement
            if (requirement.RequireRefreshToken)
            {
                var refreshTokenClaim = user.FindFirst("RefreshToken");
                if (refreshTokenClaim == null)
                {
                    context.Fail();
                    return;
                }
            }

            // Check Roles requirement
            if (requirement.Roles != null && requirement.Roles.Length > 0)
            {
                var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == null || !requirement.Roles.Contains(userRole))
                {
                    context.Fail();
                    return;
                }
            }

            context.Succeed(requirement);
        }
    }
}
