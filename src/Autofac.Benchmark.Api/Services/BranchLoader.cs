using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;

namespace Autofac.Benchmark.Api.Services
{
    public class BranchLoader
    {
        public async Task<bool> LoadAsync(Uri repositoryPath, string branchName)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(repositoryPath.LocalPath);
            var checkoutProcess = ProcessFactory.Create("git", $"checkout -b {branchName} origin/{branchName}");
            var branchLoadResult = await ProcessExecutor.RunAsync(checkoutProcess);
            Directory.SetCurrentDirectory(currentDirectory);

            return branchLoadResult.IsSuccess;
        }
    }
}