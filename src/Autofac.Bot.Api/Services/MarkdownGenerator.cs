using System;
using System.Text;
using Autofac.Bot.Api.Services.Results;
using Autofac.Bot.Api.UseCases.Abstractions.Models;

namespace Autofac.Bot.Api.Services
{
    public class MarkdownGenerator
    {
        private static readonly string FullOutputSummaryTemplateLeft =
            $"{Environment.NewLine}{Environment.NewLine}<details>{Environment.NewLine}<summary>Complete benchmark output</summary>{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}```text";

        private static readonly string FullOutputSummaryTemplateRight =
            $"```{Environment.NewLine}{Environment.NewLine}</details>{Environment.NewLine}{Environment.NewLine}";

        public string Generate(ExecutionResult targetExecutionResult, ExecutionResult sourceExecutionResult,
            string benchmark)
        {
            var content = new StringBuilder()
                .AppendLine($"### {benchmark}{Environment.NewLine}{Environment.NewLine}")
                .AppendLine(
                    $"#### Target: {GenerateMarkdownUrl(targetExecutionResult.Repository)}{Environment.NewLine}{targetExecutionResult.Summary}{Environment.NewLine}{FullOutputSummaryTemplateLeft}{Environment.NewLine}{targetExecutionResult.Output}{Environment.NewLine}{FullOutputSummaryTemplateRight}")
                .AppendLine($"{Environment.NewLine}{Environment.NewLine}")
                .AppendLine(
                    $"#### Source: {GenerateMarkdownUrl(sourceExecutionResult.Repository)} {Environment.NewLine}{sourceExecutionResult.Summary}{Environment.NewLine}{FullOutputSummaryTemplateLeft}{Environment.NewLine}{sourceExecutionResult.Output}{Environment.NewLine}{FullOutputSummaryTemplateRight}")
                .ToString();

            return content;
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