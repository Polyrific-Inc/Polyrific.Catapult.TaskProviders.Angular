using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Polyrific.Catapult.Plugins.Angular.Helpers
{
    public static class CommandHelper
    {
        public static async Task<string> ExecuteShellCommand(string command, string workingDirectory, ILogger logger = null)
        {
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var error = "";

            string fileName;

            // we cannot start the node module in ProcessStartInfo, so we will use powershell
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            {
                fileName = "powershell";
            }
            else 
            {
                fileName = "pwsh";
                command = $"-c \"{command}\"";
            }

            var info = new ProcessStartInfo(fileName)
            {
                UseShellExecute = false,
                Arguments = command,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            using (var process = Process.Start(info))
            {
                if (process != null)
                {
                    var reader = process.StandardOutput;
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();

                        logger?.LogDebug(line);

                        outputBuilder.AppendLine(line);
                    }

                    var errorReader = process.StandardError;
                    while (!errorReader.EndOfStream)
                    {
                        var line = await errorReader.ReadLineAsync();

                        if (line.StartsWith("npm WARN"))
                        {
                            logger?.LogWarning(line);
                        }
                        else if (!string.IsNullOrEmpty(line))
                        {
                            errorBuilder.AppendLine(line);
                        }
                    }

                    error = errorBuilder.ToString();
                }
            }

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return outputBuilder.ToString();
        }
    }
}