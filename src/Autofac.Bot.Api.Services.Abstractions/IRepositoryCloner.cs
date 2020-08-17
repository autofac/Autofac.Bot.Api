using System;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Abstractions.Models;

namespace Autofac.Bot.Api.Services.Abstractions
{
    public interface IRepositoryCloner
    {
        Task<RepositoryCloneResult> CloneAync(Uri repositoryUri,
            string target, string traceIdentifier);
    }
}