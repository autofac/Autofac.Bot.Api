namespace Autofac.Bot.Api.UseCases.Abstractions.Models
{
    public class RepositoryModel
    {
        public string Branch { get; }
        
        public string Url { get; }

        public RepositoryModel(string branch, string url)
        {
            Branch = branch;
            Url = url;
        }
    }
}