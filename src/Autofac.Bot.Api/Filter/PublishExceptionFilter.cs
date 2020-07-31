using System;
using System.Net;
using Autofac.Bot.Api.UseCases.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Autofac.Bot.Api.Filter
{
    public class PublishExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!(context.Exception is PublishException publishException)) return;

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Result = new ObjectResult(new
            {
                publishException.Message,
                Error = publishException.PublishError.Replace(Environment.NewLine, "")
            });
        }
    }
}