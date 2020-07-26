using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
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

            if (branchName.StartsWith("commit:", StringComparison.InvariantCultureIgnoreCase) || branchName.StartsWith("tag:"))
            {
                var safeBranchName = branchName.ToLowerInvariant().Replace("commit:", "").Replace("tag:", "");
                var commitCheckedOut = await TryCommitHashCheckoutAsync(safeBranchName);
                Directory.SetCurrentDirectory(currentDirectory);
                return commitCheckedOut;
            }

            var branchCheckedOut = await TryRegularBranchCheckoutAsync(branchName);
            Directory.SetCurrentDirectory(currentDirectory);
            return branchCheckedOut;
        }

        private async Task<bool> TryCommitHashCheckoutAsync(string branchName)
        {
            // var fetchProcess = ProcessFactory.Create("git", "fetch --all");
            // await ProcessExecutor.ExecuteAsync(fetchProcess);

            var checkoutProcess = ProcessFactory.Create("git", $"checkout -b {branchName}-branch {branchName}");

            var (succeeded, _, _, commitHashLoadError) = await ProcessExecutor.ExecuteAsync(checkoutProcess);

            if (!succeeded)
                _logger.LogError("Failed to load commit-hash. Error:{newLine}{error}}", Environment.NewLine,
                    commitHashLoadError);

            return succeeded;
        }

        private async Task<bool> TryRegularBranchCheckoutAsync(string branchName)
        {
            var checkoutProcess = ProcessFactory.Create("git", $"checkout -b {branchName}-branch origin/{branchName}");

            var (succeeded, _, _, branchLoadError) = await ProcessExecutor.ExecuteAsync(checkoutProcess);

            if (!succeeded)
                _logger.LogError("Failed to load branch. Error:{newLine}{error}}", Environment.NewLine,
                    branchLoadError);
            return succeeded;
        }
    }
}