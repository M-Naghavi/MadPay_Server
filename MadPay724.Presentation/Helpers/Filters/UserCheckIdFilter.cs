using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;

namespace MadPay724.Presentation.Helpers.Filters
{
    public class UserCheckIdFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserCheckIdFilter(ILoggerFactory loggerFactory , IHttpContextAccessor httpContextAccessor)
        {
            _logger = loggerFactory.CreateLogger("UserCheckIdFilter");
            _httpContextAccessor = httpContextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values["id"] != null && context.RouteData.Values["userId"] == null)
            {
                string id = context.RouteData.Values["id"].ToString();
                string userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userID == id)
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    _logger.LogError($"{id} : your not allowed to edit this user");
                    //context.Result = new JsonResult(new { HttpStatusCode.Unauthorized });
                    context.Result = new UnauthorizedResult();
                }
            }
            else
            {
                string id = context.RouteData.Values["userId"].ToString();
                string userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userID == id)
                {
                    base.OnActionExecuting(context);
                }
                else
                {
                    _logger.LogError($"{id} : your not allowed to edit this user");
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
