using System;
using System.Text;
using Autofac.Bot.Api.UseCases.Abstractions;
using Autofac.Bot.Api.UseCases.Abstractions.Models;

namespace Autofac.Bot.Api.Controllers.Services
{
    public static class MarkdownGenerator
    {
        private static readonly string FullOutputSummaryTemplateLeft =
            $"{Environment.NewLine}{Environment.NewLine}<details>{Environment.NewLine}<summary>Complete benchmark output</summary>{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}```text";

        private static readonly string FullOutputSummaryTemplateRight =
            $"```{Environment.NewLine}{Environment.NewLine}</details>{Environment.NewLine}{Environment.NewLine}";

        public static string Generate(string benchmark, params BenchmarkResult[] benchmarkResults)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"### {benchmark}{Environment.NewLine}{Environment.NewLine}");

            for (var i = 0; i < benchmarkResults.Length; i++)
            {
                var benchmarkResult = benchmarkResults[i];

                builder.AppendLine(
                    $"#### {benchmarkResult.RepositoryTarget.ToString()}: {GenerateMarkdownUrl(benchmarkResult.Repository)}{Environment.NewLine}{benchmarkResult.Summary}{Environment.NewLine}{FullOutputSummaryTemplateLeft}{Environment.NewLine}{benchmarkResult.Output}{Environment.NewLine}{FullOutputSummaryTemplateRight}");

                if (i == benchmarkResults.Length - 1) break;

                builder.AppendLine($"{Environment.NewLine}{Environment.NewLine}");
            }

            return builder.ToString();
        }

        private static string GenerateMarkdownUrl(Repository repository)
        {
            if (repository.Ref.StartsWith("commit:", StringComparison.InvariantCultureIgnoreCase))
                return CleanRefPrefix(repository.Ref);

            var cleanedRef = CleanRefPrefix(repository.Ref);

            var markdownUrl =
                $"[{cleanedRef}]({repository.Url.Replace(".git", "", StringComparison.InvariantCultureIgnoreCase)}/tree/{cleanedRef})";

            return markdownUrl;
        }

        private static string CleanRefPrefix(string @ref)
            => @ref.Replace("commit:", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("tag:", "", StringComparison.InvariantCultureIgnoreCase);
    }
}