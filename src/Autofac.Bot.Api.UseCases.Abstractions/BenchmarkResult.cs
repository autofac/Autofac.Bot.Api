using Autofac.Bot.Api.UseCases.Abstractions.Enums;
using Autofac.Bot.Api.UseCases.Abstractions.Models;

namespace Autofac.Bot.Api.UseCases.Abstractions
{
    public class BenchmarkResult
    {
        public BenchmarkResult(Repository repository, RepositoryTarget repositoryTarget, string summary, string output)
        {
            Repository = repository;
            RepositoryTarget = repositoryTarget;
            Summary = summary;
            Output = output;
        }

        public Repository Repository { get; }

        public RepositoryTarget RepositoryTarget { get; }

        public string Summary { get; }

        public string Output { get; }
    }
}