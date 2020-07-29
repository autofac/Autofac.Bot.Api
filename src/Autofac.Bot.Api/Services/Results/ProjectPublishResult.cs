using System;

namespace Autofac.Bot.Api.Services.Results
{
    public class ProjectPublishResult
    {
        public ProjectPublishResult(bool succeeded, Uri? publishUri)
        {
            Succeeded = succeeded;
            PublishUri = publishUri;
        }

        public ProjectPublishResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public bool Succeeded { get; }

        public string? Error { get; }

        public Uri? PublishUri { get; }
    }
}