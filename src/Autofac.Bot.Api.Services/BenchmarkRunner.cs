using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Models;
using Autofac.Bot.Api.Services.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.Services
{
    public class BenchmarkRunner
    {
        private readonly ILogger<BenchmarkRunner> _logger;

        public BenchmarkRunner(ILogger<BenchmarkRunner> logger)
        {
            _logger = logger;
        }

        public async Task<BenchmarkRunnerResult> RunAsync(Uri benchmarkBinariesUri, string assemblyName,
            string benchmarkName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(benchmarkBinariesUri.LocalPath);

            var benchmarkProcess = ProcessFactory.Create("dotnet", $"{assemblyName} -i --filter *{benchmarkName}*");

            var (succeeded, _, benchmarkOutput, benchmarkError) = await ProcessExecutor.ExecuteAsync(benchmarkProcess);

            Directory.SetCurrentDirectory(currentDirectory);

            if (!succeeded)
                _logger.LogError("Failed to execute Benchmark. Error:{newLine}{error}}", Environment.NewLine,
                    benchmarkError);

            return succeeded
                ? new BenchmarkRunnerResult(true, benchmarkOutput)
                : new BenchmarkRunnerResult(false, benchmarkError);
        }
    }
}