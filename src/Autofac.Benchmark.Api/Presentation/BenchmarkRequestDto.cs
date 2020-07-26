namespace Autofac.Benchmark.Api.Presentation
{
    public class BenchmarkRequestDto
    {
        public RepositoryDto TargetRepository { get; set; }
        
        public RepositoryDto SourceRepository { get; set; }
    }
}