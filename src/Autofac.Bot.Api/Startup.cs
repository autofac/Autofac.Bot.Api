using Autofac.Bot.Api.Filter;
using Autofac.Bot.Api.Services;
using Autofac.Bot.Api.UseCases.Abstractions.Exceptions;
using Autofac.Bot.Api.UseCases.Commands;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Bot.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                    {
                        options.Filters
                            .Add<UnhandledExceptionFilter>();
                        options.Filters
                            .Add<RepositoryCloneExceptionFilter>();
                        options.Filters
                            .Add<RefLoadExceptionFilter>();
                        options.Filters
                            .Add<PublishExceptionFilter>();
                    }
                )
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<RepositoryCloner>()
                .AsSelf();

            builder.RegisterType<BenchmarkRunner>()
                .AsSelf();

            builder.RegisterType<RefLoader>()
                .AsSelf();

            builder.RegisterType<ProjectPublisher>()
                .AsSelf();

            builder.RegisterType<SummaryExtractor>()
                .AsSelf();

            builder.AddMediatR(typeof(ExecuteBenchmarkCommandHandler).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}