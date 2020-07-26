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

        private RepositoryCloner cloner;
        private BranchLoader branchLoader;

        private ProjectBuilder projectBuilder;
        private BenchmarkExecutor benchmarkExecutor;
        private SummaryExtractor summaryExtractor;

        public BenchmarksController(BranchLoader branchLoader, RepositoryCloner cloner, ProjectBuilder projectBuilder,
            BenchmarkExecutor benchmarkExecutor, SummaryExtractor summaryExtractor)
        {
            this.branchLoader = branchLoader;
            this.cloner = cloner;
            this.projectBuilder = projectBuilder;
            this.benchmarkExecutor = benchmarkExecutor;
            this.summaryExtractor = summaryExtractor;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteAsync([FromBody] BenchmarkRequestDto benchmarkRequest)
        {
            var summarySource = await ExecuteForSourceBranch(benchmarkRequest.SourceRepository, benchmarkRequest.Benchmark);
            var summaryTarget = await ExecuteForTargetBranch(benchmarkRequest.TargetRepository, benchmarkRequest.Benchmark);

            return File(Encoding.UTF8.GetBytes($"{summarySource}\n\n{summaryTarget}"), "text/html");
        }

        private async Task<string> ExecuteForSourceBranch(RepositoryDto repository, string benchmark)
        {
            var (_, clonePath) =
                await cloner.CloneAsync(new Uri(repository.Url, UriKind.Absolute));

            await branchLoader.LoadAsync(clonePath, repository.Branch);

            await projectBuilder.BuildAsync(new Uri(BenchmarkProjectPath, UriKind.Absolute));

            var (_, output) =
                await benchmarkExecutor.ExecuteAsync(new Uri(BenchmarkExecuteablePath, UriKind.Absolute),
                    benchmark);

            var summary = summaryExtractor.ExtractSummary(output);

            return summary;
        }

        private async Task<string> ExecuteForTargetBranch(RepositoryDto repository, string benchmark)
        {
            var (_, clonePath) =
                await cloner.CloneAsync(new Uri(repository.Url, UriKind.Absolute));

            await branchLoader.LoadAsync(clonePath, repository.Branch);

            await projectBuilder.BuildAsync(new Uri(BenchmarkProjectPath, UriKind.Absolute));

            var (_, output) =
                await benchmarkExecutor.ExecuteAsync(new Uri(BenchmarkExecuteablePath, UriKind.Absolute),
                    benchmark);

            var summary = summaryExtractor.ExtractSummary(output);

            return summary;
        }
    }
}