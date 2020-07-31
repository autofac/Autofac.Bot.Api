using System;

namespace Autofac.Bot.Api.UseCases.Abstractions.Exceptions
{
    public class PublishException : Exception
    {
        public PublishException(string? message, string publishError) : base(message)
        {
            PublishError = publishError;
        }

        public string PublishError { get; }
    }
}