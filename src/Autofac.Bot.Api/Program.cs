using System;
using System.Diagnostics;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Autofac.Bot.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseContentRoot(AppContext.BaseDirectory)
                .UseSerilog(ConfigureLogger)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        private static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        }
    }
}