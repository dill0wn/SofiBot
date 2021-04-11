using System;
using System.Diagnostics;

namespace SofiBot
{
    public class ShellCommand
    {
        public string Run(string cmd, bool logResult = false)
        {
            Console.WriteLine($"ShellCommand.Run: '{cmd}'");
            var escapedCmd = cmd.Replace("\"", "\\\"");

            string result = null;

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedCmd}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    // WorkingDirectory = "",
                };

                if (!process.Start())
                {
                    throw new System.Exception("wouldn't start");
                }

                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            if (logResult)
            {
                Console.WriteLine($"ShellCommand.Result: '{result}'");
            }
            return result;
        }
    }
}