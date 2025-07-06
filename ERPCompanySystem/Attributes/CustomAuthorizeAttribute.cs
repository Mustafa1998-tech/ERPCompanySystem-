using Microsoft.AspNetCore.Authorization;

namespace ERPCompanySystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationRequirement
    {
        public string[] Roles { get; }
        public bool RequireJwt { get; }
        public bool RequireRefreshToken { get; }
        public bool RequireMultiFactor { get; }

        public CustomAuthorizeAttribute(string[] roles, bool requireJwt = false, bool requireRefreshToken = false, bool requireMultiFactor = false)
        {
            Roles = roles;
            RequireJwt = requireJwt;
            RequireRefreshToken = requireRefreshToken;
            RequireMultiFactor = requireMultiFactor;
        }

        public static CustomAuthorizeAttribute RequireJwt(params string[] roles)
        {
            return new CustomAuthorizeAttribute(roles, requireJwt: true);
        }

        public static CustomAuthorizeAttribute RequireRefreshToken(params string[] roles)
        {
            return new CustomAuthorizeAttribute(roles, requireRefreshToken: true);
        }

        public static CustomAuthorizeAttribute RequireMultiFactor(params string[] roles)
        {
            return new CustomAuthorizeAttribute(roles, requireMultiFactor: true);
        }

        public static CustomAuthorizeAttribute RequireAll(bool requireJwt = false, bool requireRefreshToken = false, bool requireMultiFactor = false)
        {
            return new CustomAuthorizeAttribute(new string[] { }, requireJwt, requireRefreshToken, requireMultiFactor);
        }
    }
}
