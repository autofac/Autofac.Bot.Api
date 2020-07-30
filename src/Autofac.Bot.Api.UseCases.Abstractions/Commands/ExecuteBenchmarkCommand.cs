using Autofac.Bot.Api.UseCases.Abstractions.Enums;
using Autofac.Bot.Api.UseCases.Abstractions.Models;
using MediatR;

namespace Autofac.Bot.Api.UseCases.Abstractions.Commands
{
    public class ExecuteBenchmarkCommand : IRequest<BenchmarkResult>
    {
        public ExecuteBenchmarkCommand(string benchmark, Repository repository, RepositoryTarget repositoryTarget)
        {
            Benchmark = benchmark;
            Repository = repository;
            RepositoryTarget = repositoryTarget;
        }

        public string Benchmark { get; }

        public Repository Repository { get; }

        public RepositoryTarget RepositoryTarget { get; }
    }
}