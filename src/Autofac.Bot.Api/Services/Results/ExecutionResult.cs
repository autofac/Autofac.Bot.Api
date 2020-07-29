using System;
using Autofac.Bot.Api.UseCases.Abstractions.Models;

namespace Autofac.Bot.Api.Services.Results
{
    public class ExecutionResult
    {
        public ExecutionResult(Repository repository, string summary, string output)
        {
            Repository = repository;
            Summary = summary;
            Output = output;
        }

        public Repository Repository { get; }
        public string Summary { get; }

        public string Output { get; }
    }
}