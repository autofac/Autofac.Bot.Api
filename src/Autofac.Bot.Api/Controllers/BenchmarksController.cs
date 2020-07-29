using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Bot.Api.Enums;
using Autofac.Bot.Api.Presentation;
using Autofac.Bot.Api.Services;
using Autofac.Bot.Api.UseCases.Abstractions.Commands;
using Autofac.Bot.Api.UseCases.Abstractions.Models;
using Autofac.Bot.Api.UseCases.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Autofac.Bot.Api.Controllers
{
    [ApiController]
    [Route("api/v1/benchmarks")]
    public class BenchmarksController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ExecuteAsync(
            [FromServices] ExecuteBenchmarkCommandHandler benchmarkCommandHandler,
            [FromBody] BenchmarkRequestDto benchmarkRequest)
        {
            var command = new ExecuteBenchmarkCommand(benchmarkRequest.Benchmark,
                new Repository(benchmarkRequest.TargetRepository.Branch, benchmarkRequest.TargetRepository.Url),
                new Repository(benchmarkRequest.SourceRepository.Branch, benchmarkRequest.SourceRepository.Url));

            var benchmarkOutputContent = await benchmarkCommandHandler.ExecuteAsync(command);

            return File(benchmarkOutputContent, "text/html");
        }
    }
}