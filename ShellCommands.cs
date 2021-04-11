using System.Diagnostics;

namespace SofiBot
{
    public class ShellCommand
    {
        public string Run(string cmd)
        {
            var escapedCmd = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedCmd}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    // WorkingDirectory = "",
                }
            };

            if (!process.Start())
            {
                throw new System.Exception("wouldn't start");
            }
            
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}