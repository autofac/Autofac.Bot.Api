using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Benchmark.Api.Services
{
    public class BranchLoader
    {
        private readonly ILogger<BranchLoader> _logger;

        public BranchLoader(ILogger<BranchLoader> logger)
        {
            _logger = logger;
        }

        public async Task<bool> LoadAsync(Uri repositoryPath, string branchName)
        {
            if (branchName == "develop") return true;
            
            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(repositoryPath.LocalPath);
            var checkoutProcess = ProcessFactory.Create("git", $"checkout -b {branchName} origin/{branchName}");
            var (succeeded, _, _, branchLoadError) = await ProcessExecutor.RunAsync(checkoutProcess);
            Directory.SetCurrentDirectory(currentDirectory);
            
            if (!succeeded) _logger.LogError("Failed to load branch. Error:{newLine}{error}}", Environment.NewLine, branchLoadError);

            return succeeded;
        }
    }
}