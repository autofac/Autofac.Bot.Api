using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
// ReSharper disable ClassNeverInstantiated.Global

namespace Autofac.Bot.Api.Controllers.Tests.Fixtures
{
    public class WebApplicationFactoryFixture : WebApplicationFactory<Startup>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            return base.CreateHost(builder);
        }
    }
}