using Autofac.Bot.Api.UseCases.Abstractions.Models;

namespace Autofac.Bot.Api.UseCases.Abstractions.Commands
{
    public class ExecuteBenchmarkCommand
    {
        public ExecuteBenchmarkCommand(string benchmark, Repository targetRepository, Repository sourceRepository)
        {
            Benchmark = benchmark;
            TargetRepository = targetRepository;
            SourceRepository = sourceRepository;
        }

        public string Benchmark { get; }

        public Repository TargetRepository { get; }

        public Repository SourceRepository { get; }
    }
}