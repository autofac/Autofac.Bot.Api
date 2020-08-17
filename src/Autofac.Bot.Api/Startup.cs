using Autofac.Bot.Api.Filter;
using Autofac.Bot.Api.Services;
using Autofac.Bot.Api.Services.Abstractions;
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
                .As<IRepositoryCloner>();

            builder.RegisterType<BenchmarkRunner>()
                .As<IBenchmarkRunner>();

            builder.RegisterType<RefLoader>()
                .As<IRefLoader>();

            builder.RegisterType<ProjectPublisher>()
                .As<IProjectPublisher>();

            builder.RegisterType<SummaryExtractor>()
                .As<ISummaryExtractor>();

            builder.AddMediatR(typeof(ExecuteBenchmarkCommandHandler).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}