using System;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Benchmark.Api.Services
{
    public class BenchmarkExecutor
    {
        private readonly ILogger<BenchmarkExecutor> _logger;

        public BenchmarkExecutor(ILogger<BenchmarkExecutor> logger)
        {
            _logger = logger;
        }

        public async Task<(bool succeeded, string output)> ExecuteAsync(Uri benchmarkAssemblyUri, string benchmarkName)
        {
            var benchmarkProcess = ProcessFactory.Create("dotnet", $"{benchmarkAssemblyUri.LocalPath} --filter *{benchmarkName}*");

            var (succeeded, _, benchmarkOutput, benchmarkError) = await ProcessExecutor.RunAsync(benchmarkProcess);
            
            if (!succeeded) _logger.LogError("Failed to execute Benchmark. Error:{newLine}{error}}", Environment.NewLine, benchmarkError);

            return succeeded ? (true, benchmarkOutput) : (false, benchmarkError);
        }
    }
}