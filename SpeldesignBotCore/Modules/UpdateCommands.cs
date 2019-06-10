using Discord.Commands;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class UpdateCommands : ModuleBase
    {
        [Command("checkupdate")]
        [Summary("Check if the bot has an update available"), Remarks("checkupdate")]
        public async Task CheckUpdate()
        {
            var process = RunCommandLineScript(@"shell/checkupdate", "origin/master");
            string result = process.StandardOutput.ReadLine();
            await ReplyAsync(result);
        }

        /// <summary>Executes a command line script. Exclude file extension in the <paramref name="scriptPath"/>.</summary>
        /// <param name="scriptPath">The path to the script to execute, without file extension. This method appends .sh or .bat depending on the current OS.</param>
        /// <param name="args">Arguments to be passed to the script.</param>
        private static Process RunCommandLineScript(string scriptPath, params string[] args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,

                // TODO: make relative
                WorkingDirectory = @"C:\Users\marcu_000\Documents\Projects\Speldesign Estetiska BOT"
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                scriptPath = scriptPath.Replace("/", @"\");
                processStartInfo.FileName = "cmd.exe";
                processStartInfo.Arguments = $"/C {scriptPath}.bat {string.Join(' ', args)}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                processStartInfo.FileName = "/bin/sh";
                processStartInfo.Arguments = $"{scriptPath}.sh {string.Join(' ', args)}";
            }

            // TODO: Exceptions when on mac

            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            
            return process;
        }
    }
}
