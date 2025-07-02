using Microsoft.AspNetCore.Authorization;

namespace ERPCompanySystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationRequirement
    {
        public string[] Roles { get; }
        public bool RequireJwt { get; }
        public bool RequireRefreshToken { get; }

        public CustomAuthorizeAttribute(params string[] roles)
        {
            if (roles == null || roles.Length == 0)
                throw new ArgumentException("At least one role must be specified");
            
            Roles = roles;
            RequireJwt = true;
            RequireRefreshToken = false;
        }

        public CustomAuthorizeAttribute(bool requireJwt, bool requireRefreshToken = false)
        {
            Roles = Array.Empty<string>();
            RequireJwt = requireJwt;
            RequireRefreshToken = requireRefreshToken;
        }

        public static CustomAuthorizeAttribute RequireJwt() => 
            new CustomAuthorizeAttribute(true);

        public static CustomAuthorizeAttribute RequireRefreshToken() => 
            new CustomAuthorizeAttribute(false, true);

        public static CustomAuthorizeAttribute RequireRole(params string[] roles) =>
            new CustomAuthorizeAttribute(roles);
    }
}
