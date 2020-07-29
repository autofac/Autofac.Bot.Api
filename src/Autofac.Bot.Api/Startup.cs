using Autofac.Bot.Api.Services;
using Autofac.Bot.Api.UseCases.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Bot.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<ExecuteBenchmarkCommandHandler>();
            services.AddScoped<RepositoryCloner>();
            services.AddScoped<BenchmarkRunner>();
            services.AddScoped<RefLoader>();
            services.AddScoped<ProjectPublisher>();
            services.AddScoped<SummaryExtractor>();
            services.AddScoped<MarkdownGenerator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}