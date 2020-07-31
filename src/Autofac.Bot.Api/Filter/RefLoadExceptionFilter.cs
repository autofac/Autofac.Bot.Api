using System;
using Autofac.Bot.Api.UseCases.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Autofac.Bot.Api.Filter
{
    public class RefLoadExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!(context.Exception is RefLoadException refLoadException)) return;

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Result = new ObjectResult(new
            {
                refLoadException.Message,
                Error = refLoadException.RefLoadError.Replace(Environment.NewLine, "")
            });
        }
    }
}