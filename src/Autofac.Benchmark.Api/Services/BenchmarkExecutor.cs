using System;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;
using CSharpFunctionalExtensions;

namespace Autofac.Benchmark.Api.Services
{
    public class BenchmarkExecutor
    {
        public async Task<(bool succeeded, string output)> ExecuteAsync(Uri benchmarkAssemblyUri, string benchmarkName)
        {
            var benchmarkProcess = ProcessFactory.Create("dotnet", $"{benchmarkAssemblyUri.LocalPath} --filter {benchmarkName}");

            var (succeeded, _, benchmarkOutput, benchmarkError) = await ProcessExecutor.RunAsync(benchmarkProcess);

            return succeeded ? (true, benchmarkOutput) : (false, benchmarkError);
        }
    }
}