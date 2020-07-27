using System;

namespace Autofac.Bot.Api.Services
{
    public class SummaryExtractor
    {
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public string ExtractSummary(string benchmarkResult)
        {
            var indexOfSummary =
                benchmarkResult.LastIndexOf("// * Summary *", StringComparison.InvariantCultureIgnoreCase);

            if (indexOfSummary == -1) return benchmarkResult;

            var summary = benchmarkResult.Substring(indexOfSummary, benchmarkResult.Length - indexOfSummary);

            var firstIndexOfPipe = summary.IndexOf("|", StringComparison.InvariantCultureIgnoreCase);

            var lastIndexOfPipe = summary.LastIndexOf("|", StringComparison.InvariantCultureIgnoreCase);

            return summary.Substring(firstIndexOfPipe, lastIndexOfPipe + 1 - firstIndexOfPipe);
        }
    }
}