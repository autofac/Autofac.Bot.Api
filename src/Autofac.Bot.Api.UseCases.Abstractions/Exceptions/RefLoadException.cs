using System;

namespace Autofac.Bot.Api.UseCases.Abstractions.Exceptions
{
    public class RefLoadException : Exception
    {
        public RefLoadException(string message, string refLoadError) : base(message)
        {
            RefLoadError = refLoadError;
        }

        public string RefLoadError { get; }
    }
}