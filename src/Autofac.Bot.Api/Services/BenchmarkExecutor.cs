using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Bot.Api.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.Services
{
    public class BenchmarkExecutor
    {
        private readonly ILogger<BenchmarkExecutor> _logger;

        public BenchmarkExecutor(ILogger<BenchmarkExecutor> logger)
        {
            _logger = logger;
        }

        public async Task<(bool succeeded, string output)> ExecuteAsync(Uri benchmarkBinariesUri, string assemblyName,
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

            return succeeded ? (true, benchmarkOutput) : (false, benchmarkError);
        }
    }
}