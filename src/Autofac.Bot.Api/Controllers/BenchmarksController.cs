using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Bot.Api.Enums;
using Autofac.Bot.Api.Presentation;
using Autofac.Bot.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Autofac.Bot.Api.Controllers
{
    [ApiController]
    [Route("api/v1/benchmarks")]
    public class BenchmarksController : ControllerBase
    {
        private const string BenchmarkAssemblyName = "Autofac.Benchmarks.dll";

        private readonly BenchmarkExecutor _benchmarkExecutor;

        private readonly string[] _benchmarkProjectPathValues =
        {
            "bench",
            "Autofac.Benchmarks",
            "Autofac.Benchmarks.csproj"
        };

        private static readonly string FullOutputSummaryTemplateLeft =
            $"{Environment.NewLine}{Environment.NewLine}<details>{Environment.NewLine}<summary>Complete benchmark output</summary>{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}```text";

        private static readonly string FullOutputSummaryTemplateRight =
            $"```{Environment.NewLine}{Environment.NewLine}</details>{Environment.NewLine}{Environment.NewLine}";


        private readonly BranchLoader _branchLoader;

        private readonly RepositoryCloner _cloner;

        private readonly ProjectBuilder _projectBuilder;
        private readonly SummaryExtractor _summaryExtractor;

        public BenchmarksController(BranchLoader branchLoader, RepositoryCloner cloner, ProjectBuilder projectBuilder,
            BenchmarkExecutor benchmarkExecutor, SummaryExtractor summaryExtractor)
        {
            _branchLoader = branchLoader;
            _cloner = cloner;
            _projectBuilder = projectBuilder;
            _benchmarkExecutor = benchmarkExecutor;
            _summaryExtractor = summaryExtractor;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteAsync([FromBody] BenchmarkRequestDto benchmarkRequest)
        {
            var (summaryTarget, outputTarget) =
                await ExecuteForTargetBranch(benchmarkRequest.TargetRepository, benchmarkRequest.Benchmark);

            var (summarySource, outputSource) =
                await ExecuteForSourceBranch(benchmarkRequest.SourceRepository, benchmarkRequest.Benchmark);

            var header = $"### {benchmarkRequest.Benchmark}{Environment.NewLine}{Environment.NewLine}";
            var targetResult =
                $"#### {benchmarkRequest.TargetRepository.Branch} (Target){Environment.NewLine}{summaryTarget}{Environment.NewLine}{FullOutputSummaryTemplateLeft}{Environment.NewLine}{outputTarget}{Environment.NewLine}{FullOutputSummaryTemplateRight}";
            var sourceResult =
                $"#### {benchmarkRequest.SourceRepository.Branch} (Source){Environment.NewLine}{summarySource}{Environment.NewLine}{FullOutputSummaryTemplateLeft}{Environment.NewLine}{outputSource}{Environment.NewLine}{FullOutputSummaryTemplateRight}";
            var sep = $"{Environment.NewLine}{Environment.NewLine}";

            var content = $"{header}{targetResult}{sep}{sourceResult}";

            return File(Encoding.UTF8.GetBytes(content), "text/html");
        }

        private async Task<(string summarySource, string completeOutput)> ExecuteForSourceBranch(
            RepositoryDto repository, string benchmark)
        {
            var (_, cloneBasePath, clonePath) = await _cloner.CloneAync(new Uri(repository.Url, UriKind.Absolute),
                RepositoryTarget.Source, Activity.Current.TraceId.ToHexString());

            await _branchLoader.LoadAsync(clonePath, repository.Branch);

            var buildPath = Path.Combine(clonePath.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            var benchmarkBinariesUri =
                await _projectBuilder.BuildAsync(new Uri(buildPath, UriKind.Absolute), cloneBasePath);

            var (_, output) =
                await _benchmarkExecutor.ExecuteAsync(benchmarkBinariesUri,
                    BenchmarkAssemblyName,
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(output);

            return (summary, output);
        }

        private async Task<(string summaryTarget, string completeOutput)> ExecuteForTargetBranch(
            RepositoryDto repository, string benchmark)
        {
            var (_, cloneBasePath, clonePath) = await _cloner.CloneAync(new Uri(repository.Url, UriKind.Absolute),
                RepositoryTarget.Target, Activity.Current.TraceId.ToHexString());

            await _branchLoader.LoadAsync(clonePath, repository.Branch);

            var buildPath = Path.Combine(clonePath.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            var benchmarkBinariesUri =
                await _projectBuilder.BuildAsync(new Uri(buildPath, UriKind.Absolute), cloneBasePath);

            var (_, output) =
                await _benchmarkExecutor.ExecuteAsync(benchmarkBinariesUri,
                    BenchmarkAssemblyName,
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(output);

            return (summary, output);
        }
    }
}