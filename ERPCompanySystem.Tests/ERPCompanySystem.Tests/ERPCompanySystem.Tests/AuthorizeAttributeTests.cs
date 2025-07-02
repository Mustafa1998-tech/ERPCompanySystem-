using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPCompanySystem.Tests
{
    [TestClass]
    public class AuthorizeAttributeTests
    {
        [TestMethod]
        public async Task OnAuthorization_WhenUserIsAuthenticated_ShouldNotThrow()
        {
            // Arrange
            var attribute = new AuthorizeAttribute();
            var httpContext = new Mock<HttpContext>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "Admin")
            }));
            httpContext.Setup(x => x.User).Returns(user);

            var actionContext = new ActionContext(httpContext.Object, 
                new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, 
                new FilterMetadata[] { });

            // Act & Assert
            await attribute.OnAuthorizationAsync(context);
            Assert.IsFalse(context.Result is UnauthorizedResult);
        }

        [TestMethod]
        public async Task OnAuthorization_WhenUserIsNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            var attribute = new AuthorizeAttribute();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(new ClaimsPrincipal());

            var actionContext = new ActionContext(httpContext.Object, 
                new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, 
                new FilterMetadata[] { });

            // Act
            await attribute.OnAuthorizationAsync(context);

            // Assert
            Assert.IsTrue(context.Result is UnauthorizedResult);
        }
    }
}
