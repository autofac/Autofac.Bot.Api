using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Abstractions;
using Autofac.Bot.Api.UseCases.Abstractions.Commands;
using Autofac.Bot.Api.UseCases.Abstractions.Exceptions;
using Autofac.Bot.Api.UseCases.Abstractions.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Autofac.Bot.Api.UseCases.Commands
{
    public class ExecuteBenchmarkCommandHandler : IRequestHandler<ExecuteBenchmarkCommand, BenchmarkResult>
    {
        private const string BenchmarkAssemblyName = "Autofac.Benchmarks.dll";


        private readonly string[] _benchmarkProjectPathValues =
        {
            "bench",
            "Autofac.Benchmarks",
            "Autofac.Benchmarks.csproj"
        };

        private readonly IBenchmarkRunner _benchmarkRunner;
        private readonly IRepositoryCloner _cloner;
        private readonly ILogger<ExecuteBenchmarkCommandHandler> _logger;
        private readonly IProjectPublisher _projectPublisher;

        private readonly IRefLoader _refLoader;
        private readonly ISummaryExtractor _summaryExtractor;


        public ExecuteBenchmarkCommandHandler(IRefLoader refLoader, IBenchmarkRunner benchmarkRunner,
            IRepositoryCloner cloner, IProjectPublisher projectPublisher, ISummaryExtractor summaryExtractor,
            ILogger<ExecuteBenchmarkCommandHandler> logger)
        {
            _refLoader = refLoader;
            _benchmarkRunner = benchmarkRunner;
            _cloner = cloner;
            _projectPublisher = projectPublisher;
            _summaryExtractor = summaryExtractor;
            _logger = logger;
        }

        public async Task<BenchmarkResult> Handle(ExecuteBenchmarkCommand request,
            CancellationToken cancellationToken)
        {
            var cloneResult = await _cloner.CloneAync(new Uri(request.Repository.Url),
                request.RepositoryTarget.ToString(), Activity.Current.TraceId.ToHexString());

            if (!cloneResult.Succeeded)
                throw new RepositoryCloneException($"Failed to clone repository with  URL: {request.Repository.Url}",
                    cloneResult.Error!);

            var refLoadResult = await _refLoader.LoadAsync(cloneResult.ClonePath!, request.Repository.Ref);

            if (!refLoadResult.Succeeded)
                throw new RefLoadException(
                    $"Failed to checkout ref: {request.Repository.Ref} for URL: {request.Repository.Url}",
                    refLoadResult.Error!);

            var publishPath = Path.Combine(cloneResult.ClonePath!.LocalPath,
                string.Join(Path.DirectorySeparatorChar, _benchmarkProjectPathValues));

            var publishResult =
                await _projectPublisher.PublishAsync(new Uri(publishPath), cloneResult.CloneBasePath!);

            if (!publishResult.Succeeded) throw new PublishException("Failed to publish project", publishResult.Error!);

            var benchmarkRunResult =
                await _benchmarkRunner.RunAsync(publishResult.PublishUri!,
                    BenchmarkAssemblyName,
                    request.Benchmark);

            SafeDeleteDirectory(cloneResult.CloneBasePath!);

            var summary = _summaryExtractor.ExtractSummary(benchmarkRunResult.Output);

            return new BenchmarkResult(request.Repository, request.RepositoryTarget, benchmarkRunResult.Succeeded,
                summary, benchmarkRunResult.Output);
        }

        private void SafeDeleteDirectory(Uri cloneBasePath)
        {
            try
            {
                if (Directory.Exists(cloneBasePath.LocalPath)) Directory.Delete(cloneBasePath.LocalPath, true);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to delete directory");
            }
        }
    }
}