using Autofac.Bot.Api.UseCases.Abstractions.Enums;

namespace Autofac.Bot.Api.UseCases.Abstractions.Models
{
    public class BenchmarkResult
    {
        public BenchmarkResult(Repository repository, RepositoryTarget repositoryTarget, bool succeeded, string summary,
            string output)
        {
            Repository = repository;
            RepositoryTarget = repositoryTarget;
            Succeeded = succeeded;
            Summary = summary;
            Output = output;
        }

        public Repository Repository { get; }

        public RepositoryTarget RepositoryTarget { get; }
        public bool Succeeded { get; }

        public string Summary { get; }

        public string Output { get; }
    }
}