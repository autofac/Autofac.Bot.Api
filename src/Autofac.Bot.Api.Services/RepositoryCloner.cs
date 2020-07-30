using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Models;
using Autofac.Bot.Api.Services.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.Services
{
    public class RepositoryCloner
    {
        private readonly ILogger<RepositoryCloner> _logger;

        public RepositoryCloner(ILogger<RepositoryCloner> logger)
        {
            _logger = logger;
        }

        public async Task<RepositoryCloneResult> CloneAync(Uri repositoryUri,
            string target, string traceIdentifier)
        {
            var cloneBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ".autofacbot",
                traceIdentifier, target.ToString());

            var clonePath = Path.Combine(cloneBasePath, "src");

            var cloneProcess =
                ProcessFactory.Create("git", $"clone {repositoryUri} {clonePath}");

            var (succeeded, _, _, cloneError) = await ProcessExecutor.ExecuteAsync(cloneProcess);

            if (!succeeded)
                _logger.LogError("Failed to clone repository. Error:{newLine}{error}}", Environment.NewLine,
                    cloneError);

            return succeeded
                ? new RepositoryCloneResult(true, new Uri(cloneBasePath), new Uri(clonePath))
                : new RepositoryCloneResult(false, cloneError);
        }
    }
}