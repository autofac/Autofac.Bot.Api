// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Autofac.Bot.Api.Controllers.Presentation
{
    public class RepositoryDto
    {
        public RepositoryDto(string @ref, string url)
        {
            Ref = @ref;
            Url = url;
        }

        // ReSharper disable once UnusedMember.Local
        private RepositoryDto()
        {
            
        }
        
        public string Ref { get; set; } = null!;

        public string Url { get; set; } = null!;
    }
}