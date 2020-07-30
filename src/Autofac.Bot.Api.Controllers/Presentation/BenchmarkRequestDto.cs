// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Autofac.Bot.Api.Controllers.Presentation
{
    public class BenchmarkRequestDto
    {
        public string Benchmark { get; set; } = null!;

        public RepositoryDto TargetRepository { get; set; } = null!;

        public RepositoryDto SourceRepository { get; set; } = null!;
    }
}