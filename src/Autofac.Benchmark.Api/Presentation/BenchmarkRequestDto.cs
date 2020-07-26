// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Autofac.Benchmark.Api.Presentation
{
    public class BenchmarkRequestDto
    {
        public string Benchmark { get; set; }
        
        public RepositoryDto TargetRepository { get; set; }
        
        public RepositoryDto SourceRepository { get; set; }
    }
}