namespace Autofac.Bot.Api.Services.Abstractions
{
    public interface ISummaryExtractor
    {
        string ExtractSummary(string benchmarkOutput);
    }
}