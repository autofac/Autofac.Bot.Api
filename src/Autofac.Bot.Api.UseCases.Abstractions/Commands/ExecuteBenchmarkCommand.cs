using Autofac.Bot.Api.UseCases.Abstractions.Enums;
using Autofac.Bot.Api.UseCases.Abstractions.Models;
using MediatR;

namespace Autofac.Bot.Api.UseCases.Abstractions.Commands
{
    public class ExecuteBenchmarkCommand : IRequest<BenchmarkResult>
    {
        public ExecuteBenchmarkCommand(string benchmark, string traceIdentifier, Repository repository, RepositoryTarget repositoryTarget)
        {
            Benchmark = benchmark;
            TraceIdentifier = traceIdentifier;
            Repository = repository;
            RepositoryTarget = repositoryTarget;
        }

        public string Benchmark { get; }

        public string TraceIdentifier { get; }

        public Repository Repository { get; }

        public RepositoryTarget RepositoryTarget { get; }
    }
}