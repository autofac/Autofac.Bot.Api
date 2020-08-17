namespace Autofac.Bot.Api.Services.Abstractions.Models
{
    public class RefLoadResult
    {
        public RefLoadResult(bool succeeded, string? error = null)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public bool Succeeded { get; }

        public string? Error { get; }
    }
}