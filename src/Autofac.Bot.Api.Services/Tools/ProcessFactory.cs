using System.Diagnostics;
using System.Text;

namespace Autofac.Bot.Api.Services.Tools
{
    internal static class ProcessFactory
    {
        public static ProcessStartInfo Create(string executableName, string args)
        {
            return new ProcessStartInfo(executableName, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8
            };
        }
    }
}