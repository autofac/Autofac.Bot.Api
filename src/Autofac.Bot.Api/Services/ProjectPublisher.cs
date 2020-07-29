using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Tools;
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

        public async Task<Uri> BuildAsync(Uri projectUri, Uri cloneBasePath)
        {
            var publishPath = Path.Combine(cloneBasePath.LocalPath, "publish");
            var process = ProcessFactory.Create("dotnet", $"publish -c Release {projectUri.LocalPath} -o {publishPath}");

            var (succeeded, _, _, buildError) = await ProcessExecutor.ExecuteAsync(process);

            if (!succeeded)
                _logger.LogError("Failed to load branch. Error:{newLine}{error}}", Environment.NewLine, buildError);

            return new Uri(publishPath);
        }
    }
}