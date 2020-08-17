using System;
using Autofac.Bot.Api.Services.Abstractions;

namespace Autofac.Bot.Api.Services
{
    public class SummaryExtractor : ISummaryExtractor
    {
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public string ExtractSummary(string benchmarkOutput)
        {
            var indexOfSummary =
                benchmarkOutput.LastIndexOf("// * Summary *", StringComparison.InvariantCultureIgnoreCase);

            if (indexOfSummary == -1) return benchmarkOutput;

            var summary = benchmarkOutput.Substring(indexOfSummary, benchmarkOutput.Length - indexOfSummary);

            var firstIndexOfPipe = summary.IndexOf("|", StringComparison.InvariantCultureIgnoreCase);

            var lastIndexOfPipe = summary.LastIndexOf("|", StringComparison.InvariantCultureIgnoreCase);

            return summary.Substring(firstIndexOfPipe, lastIndexOfPipe + 1 - firstIndexOfPipe);
        }
    }
}