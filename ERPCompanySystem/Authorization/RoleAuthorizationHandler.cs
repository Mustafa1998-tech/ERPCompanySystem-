using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ERPCompanySystem.Attributes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace ERPCompanySystem.Authorization
{
    public class RoleAuthorizationHandler : AuthorizationHandler<CustomAuthorizeAttribute>
    {
        private readonly ILogger<RoleAuthorizationHandler> _logger;

        public RoleAuthorizationHandler(ILogger<RoleAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomAuthorizeAttribute requirement)
        {
            try
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("User is not authenticated");
                    context.Fail();
                    return Task.CompletedTask;
                }

                // Check JWT token if required
                if (requirement.RequireJwt)
                {
                    var jwtClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                    if (jwtClaim == null)
                    {
                        _logger.LogWarning("JWT token is required but not found");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                // Check refresh token if required
                if (requirement.RequireRefreshToken)
                {
                    var refreshTokenClaim = context.User.FindFirst("refresh_token");
                    if (refreshTokenClaim == null)
                    {
                        _logger.LogWarning("Refresh token is required but not found");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                // Check multi-factor authentication if required
                if (requirement.RequireMultiFactor)
                {
                    var mfaClaim = context.User.FindFirst("mfa_enabled");
                    if (mfaClaim == null || mfaClaim.Value != "true")
                    {
                        _logger.LogWarning("Multi-factor authentication is required but not enabled");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                // Check roles if specified
                if (requirement.Roles != null && requirement.Roles.Length > 0)
                {
                    var hasRequiredRole = requirement.Roles.Any(role => context.User.IsInRole(role));
                    if (!hasRequiredRole)
                    {
                        _logger.LogWarning($"User does not have required roles: {string.Join(", ", requirement.Roles)}");
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in authorization handler");
                context.Fail();
                return Task.CompletedTask;
            }
        }
    }
}
