using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Enums;
using Autofac.Bot.Api.Tools;
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
        
        public async Task<(bool succeeded, Uri outputUri)> CloneAync(Uri repositoryUri, RepositoryTarget target, string traceIdentifier)
        {
            var traceIdentifierPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), traceIdentifier);

            if (!Directory.Exists(traceIdentifierPath)) Directory.CreateDirectory(traceIdentifierPath); 
            
            var clonePath = Path.Combine(traceIdentifierPath, target.ToString(), "Autofac");
            
            if (Directory.Exists(clonePath)) Directory.Delete(clonePath, true);

            Directory.CreateDirectory(clonePath);

            var cloneProcess =
                ProcessFactory.Create("git", $"clone {repositoryUri} {clonePath}");
            
            var (succeeded, _, _, cloneError) = await ProcessExecutor.ExecuteAsync(cloneProcess);
            
            if (!succeeded)
                _logger.LogError("Failed to clone repository. Error:{newLine}{error}}", Environment.NewLine, cloneError);

            return succeeded ? (true, new Uri(clonePath, UriKind.Absolute)) : (false, null);
        }
    }
}