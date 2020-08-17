using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Bot.Api.Services;
using Autofac.Bot.Api.Services.Abstractions;
using Autofac.Bot.Api.Services.Abstractions.Models;
using Autofac.Bot.Api.UseCases.Abstractions.Commands;
using Autofac.Bot.Api.UseCases.Abstractions.Enums;
using Autofac.Bot.Api.UseCases.Abstractions.Exceptions;
using Autofac.Bot.Api.UseCases.Abstractions.Models;
using Autofac.Bot.Api.UseCases.Commands;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Autofac.Bot.Api.UseCases.Tests
{
    public class ExecuteBenchmarkCommandHandlerTests
    {
        private const string AutofacUrl = "https://github.com/Autofac/Autofac.git";

        private readonly ExecuteBenchmarkCommand _command = new ExecuteBenchmarkCommand("SomeBenchmarkName",
            Guid.NewGuid().ToString(),
            new Repository("master", AutofacUrl),
            RepositoryTarget.Target);

        [Fact]
        public async Task Handle_CloningRepositoryFails_ThrowsRepositoryCloneException()
        {
            var repositoryClonerMock = new Mock<IRepositoryCloner>();
            const string exceptionMessage = "Error while cloning repository";
            repositoryClonerMock.Setup(cloner =>
                    cloner.CloneAync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RepositoryCloneResult(false, exceptionMessage));
            var handler = new ExecuteBenchmarkCommandHandler(Mock.Of<IRefLoader>(), Mock.Of<IBenchmarkRunner>(),
                repositoryClonerMock.Object, Mock.Of<IProjectPublisher>(), Mock.Of<ISummaryExtractor>(),
                new NullLogger<ExecuteBenchmarkCommandHandler>());

            var act = handler.Handle(_command, default);

            var ex = await Assert.ThrowsAsync<RepositoryCloneException>(() => act);
            ex.CloneError.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task Handle_LoadingRefFails_ThrowsRefLoadException()
        {
            var repositoryClonerMock = new Mock<IRepositoryCloner>();
            const string exceptionMessage = "Error while checking out ref";
            repositoryClonerMock.Setup(cloner =>
                    cloner.CloneAync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() =>
                    new RepositoryCloneResult(true, new Uri(AppContext.BaseDirectory),
                        new Uri(AppContext.BaseDirectory)));
            var refLoaderMock = new Mock<IRefLoader>();
            refLoaderMock.Setup(loader => loader.LoadAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RefLoadResult(false, exceptionMessage));
            var handler = new ExecuteBenchmarkCommandHandler(refLoaderMock.Object, Mock.Of<IBenchmarkRunner>(),
                repositoryClonerMock.Object, Mock.Of<IProjectPublisher>(), Mock.Of<ISummaryExtractor>(),
                new NullLogger<ExecuteBenchmarkCommandHandler>());

            var act = handler.Handle(_command, default);

            var ex = await Assert.ThrowsAsync<RefLoadException>(() => act);
            ex.RefLoadError.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task Handle_PublishingProjectFails_ThrowsPublishException()
        {
            var repositoryClonerMock = new Mock<IRepositoryCloner>();
            const string exceptionMessage = "Error while publishing benchmark project!";
            repositoryClonerMock.Setup(cloner =>
                    cloner.CloneAync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() =>
                    new RepositoryCloneResult(true, new Uri(AppContext.BaseDirectory),
                        new Uri(AppContext.BaseDirectory)));
            var refLoaderMock = new Mock<IRefLoader>();
            refLoaderMock.Setup(loader => loader.LoadAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RefLoadResult(true));
            var publisherMock = new Mock<IProjectPublisher>();
            publisherMock.Setup(publisher => publisher.PublishAsync(It.IsAny<Uri>(), It.IsAny<Uri>()))
                .ReturnsAsync(() => new ProjectPublishResult(false, exceptionMessage));
            var handler = new ExecuteBenchmarkCommandHandler(refLoaderMock.Object, Mock.Of<IBenchmarkRunner>(),
                repositoryClonerMock.Object, publisherMock.Object, Mock.Of<ISummaryExtractor>(),
                new NullLogger<ExecuteBenchmarkCommandHandler>());

            var act = handler.Handle(_command, default);

            var ex = await Assert.ThrowsAsync<PublishException>(() => act);
            ex.PublishError.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task Handle_BenchmarkExecutionFails_ReturnsBenchmarkResultWithErrorOutput()
        {
            var repositoryClonerMock = new Mock<IRepositoryCloner>();
            const string benchmarkExecutionError = "Error while executing benchmark!";
            repositoryClonerMock.Setup(cloner =>
                    cloner.CloneAync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() =>
                    new RepositoryCloneResult(true, new Uri(Path.Combine(AppContext.BaseDirectory, Guid.NewGuid().ToString())),
                        new Uri(AppContext.BaseDirectory)));
            var refLoaderMock = new Mock<IRefLoader>();
            refLoaderMock.Setup(loader => loader.LoadAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RefLoadResult(true));
            var publisherMock = new Mock<IProjectPublisher>();
            publisherMock.Setup(publisher => publisher.PublishAsync(It.IsAny<Uri>(), It.IsAny<Uri>()))
                .ReturnsAsync(() => new ProjectPublishResult(true, new Uri(AppContext.BaseDirectory)));
            var benchmarkRunnerMock = new Mock<IBenchmarkRunner>();
            benchmarkRunnerMock.Setup(
                    runner => runner.RunAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new BenchmarkRunResult(false, benchmarkExecutionError));
            var handler = new ExecuteBenchmarkCommandHandler(refLoaderMock.Object, benchmarkRunnerMock.Object,
                repositoryClonerMock.Object, publisherMock.Object, new SummaryExtractor(),
                new NullLogger<ExecuteBenchmarkCommandHandler>());

            var benchmarkResult = await handler.Handle(_command, default);

            benchmarkResult.Succeeded.Should().BeFalse();
            benchmarkResult.Output.Should().Be(benchmarkExecutionError);
            benchmarkResult.Summary.Should().Be(benchmarkExecutionError);
        }

        [Fact]
        public async Task Handle_BenchmarkExecutionSucceeds_ReturnsSummaryAndFullOutput()
        {
            var repositoryClonerMock = new Mock<IRepositoryCloner>();
            var benchmarkOutput = await
                File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Files", "SampleBenchOutput.txt"));
            repositoryClonerMock.Setup(cloner =>
                    cloner.CloneAync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() =>
                    new RepositoryCloneResult(true, new Uri(Path.Combine(AppContext.BaseDirectory, Guid.NewGuid().ToString())),
                        new Uri(AppContext.BaseDirectory)));
            var refLoaderMock = new Mock<IRefLoader>();
            refLoaderMock.Setup(loader => loader.LoadAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(() => new RefLoadResult(true));
            var publisherMock = new Mock<IProjectPublisher>();
            publisherMock.Setup(publisher => publisher.PublishAsync(It.IsAny<Uri>(), It.IsAny<Uri>()))
                .ReturnsAsync(() => new ProjectPublishResult(true, new Uri(AppContext.BaseDirectory)));
            var benchmarkRunnerMock = new Mock<IBenchmarkRunner>();
            benchmarkRunnerMock.Setup(
                    runner => runner.RunAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => new BenchmarkRunResult(true, benchmarkOutput));
            var handler = new ExecuteBenchmarkCommandHandler(refLoaderMock.Object, benchmarkRunnerMock.Object,
                repositoryClonerMock.Object, publisherMock.Object, new SummaryExtractor(),
                new NullLogger<ExecuteBenchmarkCommandHandler>());

            var benchmarkResult = await handler.Handle(_command, default);

            benchmarkResult.Succeeded.Should().BeTrue();
            benchmarkResult.Output.Should().Be(benchmarkOutput);
            benchmarkResult.Summary.Should().NotBe(benchmarkOutput);
        }
    }
}