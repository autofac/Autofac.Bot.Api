using System;
using Autofac.Bot.Api.UseCases.Abstractions.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Autofac.Bot.Api.Filter
{
    public class RepositoryCloneExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!(context.Exception is RepositoryCloneException repositoryCloneException)) return;

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Result = new ObjectResult(new
            {
                repositoryCloneException.Message,
                Error = repositoryCloneException.CloneError.Replace(Environment.NewLine, "")
            });
        }
    }
}