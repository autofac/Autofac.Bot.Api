using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Presentation;
using Autofac.Benchmark.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Autofac.Benchmark.Api.Controllers
{
    [ApiController]
    [Route("api/v1/benchmarks")]
    public class BenchmarksController : ControllerBase
    {
        private static readonly string BenchmarkProjectPath = Path.Combine(AppContext.BaseDirectory, "Autofac", "bench",
            "Autofac.Benchmarks",
            "Autofac.Benchmarks.csproj");

        private static readonly string BenchmarkExecuteablePath = Path.Combine(AppContext.BaseDirectory, "Autofac",
            "bench", "Autofac.Benchmarks",
            "bin", "Release", "netcoreapp3.1", "Autofac.Benchmarks.dll");

        private readonly RepositoryCloner _cloner;
        private readonly BranchLoader _branchLoader;

        private readonly ProjectBuilder _projectBuilder;
        private readonly BenchmarkExecutor _benchmarkExecutor;
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
            var summarySource =
                await ExecuteForSourceBranch(benchmarkRequest.SourceRepository, benchmarkRequest.Benchmark);
            var summaryTarget =
                await ExecuteForTargetBranch(benchmarkRequest.TargetRepository, benchmarkRequest.Benchmark);

            var resultPartOne = $"#### {benchmarkRequest.SourceRepository.Branch}{Environment.NewLine}{summarySource}";
            var sep = $"{Environment.NewLine}{Environment.NewLine}";
            var resultPartTwo = $"#### {benchmarkRequest.TargetRepository.Branch}{Environment.NewLine}{summaryTarget}";

            return File(Encoding.UTF8.GetBytes($"{resultPartOne}{sep}{resultPartTwo}"), "text/html");
        }

        private async Task<string> ExecuteForSourceBranch(RepositoryDto repository, string benchmark)
        {
            var (_, clonePath) =
                await _cloner.CloneAsync(new Uri(repository.Url, UriKind.Absolute));

            await _branchLoader.LoadAsync(clonePath, repository.Branch);

            await _projectBuilder.BuildAsync(new Uri(BenchmarkProjectPath, UriKind.Absolute));

            var (_, output) =
                await _benchmarkExecutor.ExecuteAsync(new Uri(BenchmarkExecuteablePath, UriKind.Absolute),
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(output);

            return summary;
        }

        private async Task<string> ExecuteForTargetBranch(RepositoryDto repository, string benchmark)
        {
            var (_, clonePath) =
                await _cloner.CloneAsync(new Uri(repository.Url, UriKind.Absolute));

            await _branchLoader.LoadAsync(clonePath, repository.Branch);

            await _projectBuilder.BuildAsync(new Uri(BenchmarkProjectPath, UriKind.Absolute));

            var (_, output) =
                await _benchmarkExecutor.ExecuteAsync(new Uri(BenchmarkExecuteablePath, UriKind.Absolute),
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(output);

            return summary;
        }
    }
}