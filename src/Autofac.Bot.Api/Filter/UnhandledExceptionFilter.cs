using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Autofac.Bot.Api.Filter
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UnhandledExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled) return;

            var path = context.HttpContext.Request.Path;

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(new
            {
                Message = "Unhandled exception while executing request",
                Path = path.HasValue ? path.Value : null,
                StackTrace = context.Exception.StackTrace?.Trim()
            });
        }
    }
}