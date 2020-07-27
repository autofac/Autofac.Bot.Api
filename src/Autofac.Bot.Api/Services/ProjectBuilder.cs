using System;
using System.Threading.Tasks;
using Autofac.Bot.Api.Tools;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.Services
{
    public class ProjectBuilder
    {
        private readonly ILogger<ProjectBuilder> _logger;

        public ProjectBuilder(ILogger<ProjectBuilder> logger)
        {
            _logger = logger;
        }

        public async Task<bool> BuildAsync(Uri projectUri)
        {
            var process = ProcessFactory.Create("dotnet", $"build -c Release {projectUri.LocalPath}");

            var (succeeded, _, _, buildError) = await ProcessExecutor.ExecuteAsync(process);

            if (!succeeded)
                _logger.LogError("Failed to load branch. Error:{newLine}{error}}", Environment.NewLine, buildError);

            return succeeded;
        }
    }
}