namespace Autofac.Bot.Api.Services.Abstractions.Models
{
    public class BenchmarkRunResult
    {
        public BenchmarkRunResult(bool succeeded, string output)
        {
            Succeeded = succeeded;
            Output = output;
        }
        
        public bool Succeeded { get; }

        public string Output { get; }
    }
}