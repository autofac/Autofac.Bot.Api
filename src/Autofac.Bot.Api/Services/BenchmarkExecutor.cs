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

        public async Task<(bool succeeded, string output)> ExecuteAsync(Uri benchmarkBuildOutputUri, string assemblyName, string benchmarkName)
        {
            var localBasePath = benchmarkBuildOutputUri.LocalPath;

            var directories = Directory.EnumerateDirectories(localBasePath);

            var targetFrameworkMonikerPath = directories.First()!;

            var executeablePath = Path.Combine(targetFrameworkMonikerPath, assemblyName);
            
            var benchmarkProcess = ProcessFactory.Create("dotnet", $"{executeablePath} --filter *{benchmarkName}*");

            var (succeeded, _, benchmarkOutput, benchmarkError) = await ProcessExecutor.ExecuteAsync(benchmarkProcess);
            
            if (!succeeded) _logger.LogError("Failed to execute Benchmark. Error:{newLine}{error}}", Environment.NewLine, benchmarkError);

            return succeeded ? (true, benchmarkOutput) : (false, benchmarkError);
        }
    }
}