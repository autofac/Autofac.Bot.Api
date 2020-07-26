using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Benchmark.Api.Tools;

namespace Autofac.Benchmark.Api.Services
{
    public class RepositoryCloner
    {
        public async Task<(bool succeeded, Uri outputUri)> CloneAsync(Uri repositoryUri)
        {
            var clonePath = Path.Combine(AppContext.BaseDirectory, "Autofac");
            if (Directory.Exists(clonePath)) Directory.Delete(clonePath, true);

            var cloneProcess =
                ProcessFactory.Create("git", $"clone https://github.com/alsami/Autofac.git {clonePath}");
            var output = await ProcessExecutor.RunAsync(cloneProcess);

            return output.IsSuccess ? (true, new Uri(clonePath, UriKind.Absolute)) : (false, null);
        }
    }
}