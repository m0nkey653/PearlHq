using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PearlHqWeb.Filters;

public class ApiKeyAuthAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedApiKey = configuration["Ingest:ApiKey"];

        var providedApiKey = context.HttpContext.Request.Headers["X-Api-Key"].ToString();

        if (string.IsNullOrEmpty(expectedApiKey) || providedApiKey != expectedApiKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
