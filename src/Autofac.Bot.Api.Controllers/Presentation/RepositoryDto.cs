// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Autofac.Bot.Api.Controllers.Presentation
{
    public class RepositoryDto
    {
        public string Ref { get; set; } = null!;

        public string Url { get; set; } = null!;
    }
}