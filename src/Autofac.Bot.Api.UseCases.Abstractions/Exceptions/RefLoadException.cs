using System;

namespace Autofac.Bot.Api.UseCases.Abstractions.Exceptions
{
    public class RefLoadException : Exception
    {
        public RefLoadException(string message, string refCheckoutError) : base(message)
        {
            RefLoadError = refCheckoutError;
        }

        public string RefLoadError { get; }
    }
}