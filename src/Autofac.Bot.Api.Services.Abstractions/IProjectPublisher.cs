using System;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Abstractions.Models;

namespace Autofac.Bot.Api.Services.Abstractions
{
    public interface IProjectPublisher
    {
        Task<ProjectPublishResult> PublishAsync(Uri projectUri, Uri cloneBasePath);
    }
}