using System;

namespace Autofac.Bot.Api.Services.Abstractions.Models
{
    public class RepositoryCloneResult
    {
        public RepositoryCloneResult(bool succeeded, Uri? cloneBasePath = null, Uri? clonePath = null)
        {
            Succeeded = succeeded;
            CloneBasePath = cloneBasePath;
            ClonePath = clonePath;
            Error = null;
        }

        public RepositoryCloneResult(bool succeeded, string? error)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public bool Succeeded { get; }

        public string? Error { get; }

        public Uri? CloneBasePath { get; }

        public Uri? ClonePath { get; }
    }
}