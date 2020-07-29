using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Results;
using Autofac.Bot.Api.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.Services
{
    public class RefLoader
    {
        private readonly ILogger<RefLoader> _logger;

        public RefLoader(ILogger<RefLoader> logger)
        {
            _logger = logger;
        }

        public async Task<RefLoadResult> LoadAsync(Uri repositoryPath, string branchName)
        {
            if (branchName == "develop") return new RefLoadResult(true);

            var currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(repositoryPath.LocalPath);

            if (branchName.StartsWith("commit:", StringComparison.InvariantCultureIgnoreCase) ||
                branchName.StartsWith("tag:"))
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

        private async Task<RefLoadResult> TryCommitHashCheckoutAsync(string branchName)
        {
            var checkoutProcess = ProcessFactory.Create("git", $"checkout -b {branchName}-branch {branchName}");

            var (succeeded, _, _, commitHashLoadError) = await ProcessExecutor.ExecuteAsync(checkoutProcess);

            if (!succeeded)
                _logger.LogError("Failed to load commit-hash. Error:{newLine}{error}}", Environment.NewLine,
                    commitHashLoadError);

            return new RefLoadResult(succeeded, succeeded ? null : commitHashLoadError);
        }

        private async Task<RefLoadResult> TryRegularBranchCheckoutAsync(string branchName)
        {
            var checkoutProcess = ProcessFactory.Create("git", $"checkout -b {branchName}-branch origin/{branchName}");

            var (succeeded, _, _, branchLoadError) = await ProcessExecutor.ExecuteAsync(checkoutProcess);

            if (!succeeded)
                _logger.LogError("Failed to load branch. Error:{newLine}{error}}", Environment.NewLine,
                    branchLoadError);

            return new RefLoadResult(succeeded, succeeded ? null : branchLoadError);
        }
    }
}