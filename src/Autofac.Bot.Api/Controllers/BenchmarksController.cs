using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
        // private static readonly string BenchmarkProjectPath = Path.Combine(AppContext.BaseDirectory, "Autofac", "bench",
        //     "Autofac.Benchmarks",
        //     "Autofac.Benchmarks.csproj");

        private readonly string[] _benchmarkProjectPathValues = {
            "bench",
            "Autofac.Benchmarks",
            "Autofac.Benchmarks.csproj"
        };

        // private static readonly string BenchmarkBuildOutputBasePath = Path.Combine(AppContext.BaseDirectory, "Autofac",
        //     "bench", "Autofac.Benchmarks",
        //     "bin", "Release");

        private readonly string[] _benchmarkBuildOutputBasePathValues =
        {
            "bench", 
            "Autofac.Benchmarks",
            "bin", 
            "Release"
        };

        private const string BenchmarkAssemblyName = "Autofac.Benchmarks.dll";

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
            var summaryTarget =
                await ExecuteForTargetBranch(benchmarkRequest.TargetRepository, benchmarkRequest.Benchmark);

            var summarySource =
                await ExecuteForSourceBranch(benchmarkRequest.SourceRepository, benchmarkRequest.Benchmark);

            var header = $"## {benchmarkRequest.Benchmark}{Environment.NewLine}{Environment.NewLine}";
            var resultPartOne =
                $"#### {benchmarkRequest.TargetRepository.Branch} (Target){Environment.NewLine}{summaryTarget}";
            var resultPartTwo =
                $"#### {benchmarkRequest.SourceRepository.Branch} (Source){Environment.NewLine}{summarySource}";
            var sep = $"{Environment.NewLine}{Environment.NewLine}";

            return File(Encoding.UTF8.GetBytes($"{header}{resultPartOne}{sep}{resultPartTwo}"), "text/html");
        }

        private async Task<string> ExecuteForSourceBranch(RepositoryDto repository, string benchmark)
        {
            var (_, clonePath) = await _cloner.CloneAync(new Uri(repository.Url, UriKind.Absolute), RepositoryTarget.Source, Activity.Current.Id);

            await _branchLoader.LoadAsync(clonePath, repository.Branch);

            var buildPath = Path.Combine(clonePath.LocalPath, string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            await _projectBuilder.BuildAsync(new Uri(buildPath, UriKind.Absolute));

            var benchmarkBinaryPath = Path.Combine(clonePath.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkBuildOutputBasePathValues));

            var (_, output) =
                await _benchmarkExecutor.ExecuteAsync(new Uri(benchmarkBinaryPath, UriKind.Absolute),
                    BenchmarkAssemblyName,
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(output);

            return summary;
        }

        private async Task<string> ExecuteForTargetBranch(RepositoryDto repository, string benchmark)
        {
            var (_, clonePath) = await _cloner.CloneAync(new Uri(repository.Url, UriKind.Absolute), RepositoryTarget.Target, Activity.Current.Id);

            await _branchLoader.LoadAsync(clonePath, repository.Branch);

            var buildPath = Path.Combine(clonePath.LocalPath, string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            await _projectBuilder.BuildAsync(new Uri(buildPath, UriKind.Absolute));

            var benchmarkBinaryPath = Path.Combine(clonePath.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkBuildOutputBasePathValues));

            var (_, output) =
                await _benchmarkExecutor.ExecuteAsync(new Uri(benchmarkBinaryPath, UriKind.Absolute),
                    BenchmarkAssemblyName,
                    benchmark);

            var summary = _summaryExtractor.ExtractSummary(output);

            return summary;
        }
    }
}