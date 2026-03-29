using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Store_API.MiddleWares
{
    public class ApiKeyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Secret-Key", out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key is missing!");
                return;
            }

            if (extractedApiKey != "YourSecretPassword123")
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key!");
                return;
            }
        }
    }
}
