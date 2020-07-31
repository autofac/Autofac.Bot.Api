// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Autofac.Bot.Api.Controllers.Presentation
{
    public class BenchmarkRequestDto
    {
        public BenchmarkRequestDto(string benchmark, RepositoryDto targetRepository, RepositoryDto sourceRepository)
        {
            Benchmark = benchmark;
            TargetRepository = targetRepository;
            SourceRepository = sourceRepository;
        }

        // ReSharper disable once UnusedMember.Local
        private BenchmarkRequestDto()
        {
            
        }
        
        public string Benchmark { get; set; } = null!;

        public RepositoryDto TargetRepository { get; set; } = null!;

        public RepositoryDto SourceRepository { get; set; } = null!;
    }
}