using System;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;

namespace Autofac.Benchmark.Api.Services
{
    public class ProjectBuilder
    {
        public async Task<bool> BuildAsync(Uri projectUri)
        {
            var process = ProcessFactory.Create("dotnet", $"build -c Release {projectUri.LocalPath}");

            var buildResult = await ProcessExecutor.RunAsync(process);

            return buildResult.IsSuccess;
        }


    }
}