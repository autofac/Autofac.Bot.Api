using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Autofac.Bot.Api.Tools
{
    public static class ProcessExecutor
    {
        public static async Task<Result<string>> ExecuteAsync(ProcessStartInfo processStartInfo)
        {
            using var process = Process.Start(processStartInfo);
            if (process == null) return Result.Failure<string>("Couldn't start process to create library!");

            var output = await process.StandardOutput.ReadToEndAsync();

            while (!process.HasExited)
            {
                await Task.Delay(10);
            }

            try
            {
                return process.ExitCode == 0
                    ? Result.Success(output)
                    : Result.Failure<string>(await process.StandardError.ReadToEndAsync());
            }
            catch (Exception)
            {
                return Result.Failure<string>(output);
            }
        }
    }
}