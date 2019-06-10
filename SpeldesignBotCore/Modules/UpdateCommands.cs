using Discord.Commands;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using System;
using SpeldesignBotCore.Loggers;
using System.Reflection;
using Discord.WebSocket;
using System.IO;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore.Modules
{
    public class UpdateCommands : ModuleBase
    {
        private readonly BotConfiguration _botConfiguration;

        public UpdateCommands()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("checkupdate")]
        [Summary("Check if the bot has an update available from the specified upstream. Defaults to origin/master."), Remarks("checkupdate [upstream]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task CheckUpdate([Remainder] string upstream = null)
        {
            upstream = upstream ?? "origin/master";

            var needsUpdate = BotNeedsUpdate(upstream);

            var title = needsUpdate 
                ? "Outdated" 
                : "Up to date";

            var description = needsUpdate
                ? $"There are updates on the server.\nRun `{_botConfiguration.Prefix}update` to download the latest version."
                : "This bot is up to date with the server.";

            var color = needsUpdate
                ? new Color(255, 79, 79)
                : new Color(118, 196, 177);

            var embedBuilder = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithFooter($"Checking against the '{upstream}' branch");

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("update")]
        [Summary("Update the bot to the latest version from the specified upstream. Defaults to origin/master."), Remarks("update [upstream]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Update([Remainder] string upstream = null)
        {
            upstream = upstream ?? "origin/master";

            if (!BotNeedsUpdate(upstream))
            {
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("Already up to date")
                    .WithDescription("The bot is already up to date with the server.")
                    .WithColor(118, 196, 177)
                    .WithFooter($"Checking against the '{upstream}' branch");

                await ReplyAsync("", embed: embedBuilder.Build());
                return;
            }

            await ReplyAsync("Downloading the latest update, please wait...");

            var process = RunShellScript("shell/update", upstream);
            Unity.Resolve<StatusLogger>().LogToConsole(process.StandardOutput.ReadToEnd());

            await ReplyAsync("Update downloaded! Restarting...");


            ProcessStartInfo newBuildStartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"\"{Assembly.GetEntryAssembly().Location}\""
            };

            Process.Start(newBuildStartInfo);

            await (Context.Client as DiscordSocketClient).LogoutAsync();
            Environment.Exit(0);
        }

        private bool BotNeedsUpdate(string upstream)
        {
            var process = RunShellScript("shell/checkupdate", upstream);
            string result = process.StandardOutput.ReadLine();

            switch (result)
            {
                case "Up to date": return false;
                case "Outdated": return true;

                default: throw new Exception($"The shell script 'checkupdate' does not work correctly for {Environment.OSVersion}");
            }
        }

        /// <summary>Executes a shell script. Exclude file extension in the <paramref name="scriptPath"/>.</summary>
        /// <param name="scriptPath">The path to the script to execute, without file extension. This method appends .sh or .bat depending on the current OS.</param>
        /// <param name="args">Arguments to be passed to the script.</param>
        private static Process RunShellScript(string scriptPath, params string[] args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Directory.GetCurrentDirectory()
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                processStartInfo.FileName = "/Applications/Utilities/Terminal.app";
                processStartInfo.Arguments = $"sh {scriptPath}.sh {string.Join(' ', args)}";
            }

            // TODO: test on mac and linux

            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            
            return process;
        }
    }
}
