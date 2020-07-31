using System;

namespace Autofac.Bot.Api.UseCases.Abstractions.Exceptions
{
    public class RepositoryCloneException : Exception
    {
        public RepositoryCloneException(string message, string cloneError) : base (message)
        {
            CloneError = cloneError;
        }

        public string CloneError { get; }
    }
}