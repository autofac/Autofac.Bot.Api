namespace Autofac.Bot.Api.UseCases.Abstractions.Models
{
    public class Repository
    {
        public string Ref { get; }

        public string Url { get; }

        public Repository(string @ref, string url)
        {
            Ref = @ref;
            Url = url;
        }
    }
}