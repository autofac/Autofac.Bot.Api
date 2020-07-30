using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Models;
using Autofac.Bot.Api.Services.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.Services
{
    public class ProjectPublisher
    {
        private readonly ILogger<ProjectPublisher> _logger;

        public ProjectPublisher(ILogger<ProjectPublisher> logger)
        {
            _logger = logger;
        }

        public async Task<ProjectPublishResult> PublishAsync(Uri projectUri, Uri cloneBasePath)
        {
            var publishPath = Path.Combine(cloneBasePath.LocalPath, "publish");

            var process =
                ProcessFactory.Create("dotnet", $"publish -c Release {projectUri.LocalPath} -o {publishPath}");

            var (succeeded, _, _, publishError) = await ProcessExecutor.ExecuteAsync(process);

            if (!succeeded)
                _logger.LogError("Failed to load branch. Error:{newLine}{error}}", Environment.NewLine, publishError);

            return succeeded
                ? new ProjectPublishResult(true, new Uri(publishPath))
                : new ProjectPublishResult(false, publishError);
        }
    }
}