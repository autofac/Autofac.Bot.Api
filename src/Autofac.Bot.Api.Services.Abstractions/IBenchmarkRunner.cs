using System;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Abstractions.Models;

namespace Autofac.Bot.Api.Services.Abstractions
{
    public interface IBenchmarkRunner
    {
        Task<BenchmarkRunResult> RunAsync(Uri benchmarkBinariesUri, string assemblyName,
            string benchmarkName);
    }
}