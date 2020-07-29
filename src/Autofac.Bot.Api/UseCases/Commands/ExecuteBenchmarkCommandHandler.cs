using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac.Bot.Api.Enums;
using Autofac.Bot.Api.Services;
using Autofac.Bot.Api.Services.Results;
using Autofac.Bot.Api.UseCases.Abstractions.Commands;
using Autofac.Bot.Api.UseCases.Abstractions.Models;

namespace Autofac.Bot.Api.UseCases.Commands
{
    public class ExecuteBenchmarkCommandHandler
    {
        private const string BenchmarkAssemblyName = "Autofac.Benchmarks.dll";


        private readonly string[] _benchmarkProjectPathValues =
        {
            "bench",
            "Autofac.Benchmarks",
            "Autofac.Benchmarks.csproj"
        };

        private readonly RefLoader _refLoader;
        private readonly BenchmarkRunner _benchmarkRunner;
        private readonly RepositoryCloner _cloner;
        private readonly ProjectPublisher _projectPublisher;
        private readonly SummaryExtractor _summaryExtractor;
        private readonly MarkdownGenerator _markdownGenerator;

        public ExecuteBenchmarkCommandHandler(RefLoader refLoader, BenchmarkRunner benchmarkRunner,
            RepositoryCloner cloner, ProjectPublisher projectPublisher, SummaryExtractor summaryExtractor,
            MarkdownGenerator markdownGenerator)
        {
            _refLoader = refLoader;
            _benchmarkRunner = benchmarkRunner;
            _cloner = cloner;
            _projectPublisher = projectPublisher;
            _summaryExtractor = summaryExtractor;
            _markdownGenerator = markdownGenerator;
        }

        public async Task<byte[]> ExecuteAsync(ExecuteBenchmarkCommand command)
        {
            var targetExecutionResult =
                await ExecuteForTargetBranch(command.TargetRepository, command.Benchmark);

            var sourceExecutionResult =
                await ExecuteForSourceBranch(command.SourceRepository, command.Benchmark);


            return Encoding.UTF8.GetBytes(_markdownGenerator.Generate(targetExecutionResult, sourceExecutionResult,
                command.Benchmark));
        }

        private async Task<ExecutionResult> ExecuteForSourceBranch(
            Repository repository, string benchmark)
        {
            var cloneResult = await _cloner.CloneAync(new Uri(repository.Url, UriKind.Absolute),
                RepositoryTarget.Source, Activity.Current.TraceId.ToHexString());

            await _refLoader.LoadAsync(cloneResult.ClonePath!, repository.Ref);

            var buildPath = Path.Combine(cloneResult.ClonePath!.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            var publishResult =
                await _projectPublisher.PublishAsync(new Uri(buildPath), cloneResult.CloneBasePath!);

            var benchmarkRunnerResult =
                await _benchmarkRunner.RunAsync(publishResult.PublishUri!,
                    BenchmarkAssemblyName,
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(benchmarkRunnerResult.Output);

            return new ExecutionResult(repository, summary, benchmarkRunnerResult.Output);
        }

        private async Task<ExecutionResult> ExecuteForTargetBranch(
            Repository repository, string benchmark)
        {
            var cloneResult = await _cloner.CloneAync(new Uri(repository.Url, UriKind.Absolute),
                RepositoryTarget.Target, Activity.Current.TraceId.ToHexString());

            await _refLoader.LoadAsync(cloneResult.ClonePath!, repository.Ref);

            var buildPath = Path.Combine(cloneResult.ClonePath!.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            var publishResult =
                await _projectPublisher.PublishAsync(new Uri(buildPath), cloneResult.CloneBasePath!);

            var benchmarkRunnerResult =
                await _benchmarkRunner.RunAsync(publishResult.PublishUri!,
                    BenchmarkAssemblyName,
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(benchmarkRunnerResult.Output);

            return new ExecutionResult(repository, summary, benchmarkRunnerResult.Output);
        }
    }
}