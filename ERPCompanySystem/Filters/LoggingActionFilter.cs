using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ERPCompanySystem.Filters
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation($"Starting action: {actionName} in {controllerName} by user: {userId}");

            var resultContext = await next();

            if (resultContext.Exception != null)
            {
                _logger.LogError(resultContext.Exception, 
                    $"Error in action: {actionName} in {controllerName} by user: {userId}");
            }
            else
            {
                _logger.LogInformation($"Completed action: {actionName} in {controllerName} by user: {userId}");
            }
        }
    }
}
