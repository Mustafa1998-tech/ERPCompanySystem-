using Microsoft.AspNetCore.Authorization;

namespace ERPCompanySystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationRequirement
    {
        public string[] Roles { get; }
        public bool RequireJwt { get; }
        public bool RequireRefreshToken { get; }

        public AuthorizeAttribute(params string[] roles)
        {
            Roles = roles;
            RequireJwt = true;
            RequireRefreshToken = false;
        }

        public AuthorizeAttribute(bool requireJwt, bool requireRefreshToken = false)
        {
            Roles = Array.Empty<string>();
            RequireJwt = requireJwt;
            RequireRefreshToken = requireRefreshToken;
        }
    }
}
