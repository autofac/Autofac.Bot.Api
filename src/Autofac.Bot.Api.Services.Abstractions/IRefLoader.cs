using System;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services.Abstractions.Models;

namespace Autofac.Bot.Api.Services.Abstractions
{
    public interface IRefLoader
    {
        Task<RefLoadResult> LoadAsync(Uri repositoryPath, string branchName);
    }
}