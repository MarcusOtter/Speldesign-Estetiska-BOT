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

            var needsUpdate = await BotNeedsUpdate(upstream);

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
            var socketClient = Context.Client as DiscordSocketClient;

            if (!await BotNeedsUpdate(upstream))
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
            await socketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await socketClient.SetGameAsync("updating...", type: ActivityType.Playing);

            var process = await RunShellScriptAsync("shell/update", upstream);
            Unity.Resolve<StatusLogger>().LogToConsole(process.StandardOutput.ReadToEnd());

            // From this point on, we need to check if the bot is still connected before doing
            // any actions from the client. There's a chance the update script takes too long
            // (especially on Raspberry Pi) which leads to the bot disconnecting. 

            if (socketClient.ConnectionState == ConnectionState.Connected)
            {
                await ReplyAsync("Update downloaded! Restarting...");
                await socketClient.SetGameAsync("new features being installed...", type: ActivityType.Watching);
            }

            ProcessStartInfo newBuildStartInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"\"{Assembly.GetEntryAssembly().Location}\""
            };

            Process.Start(newBuildStartInfo);

            if (socketClient.ConnectionState == ConnectionState.Connected)
            {
                await socketClient.SetStatusAsync(UserStatus.Invisible);
                await socketClient.LogoutAsync();
            }

            Environment.Exit(0);
        }

        private async Task<bool> BotNeedsUpdate(string upstream)
        {
            var process = await RunShellScriptAsync("shell/checkupdate", upstream);
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
        private static Task<Process> RunShellScriptAsync(string scriptPath, params string[] args)
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                processStartInfo.FileName = "/bin/sh";
                processStartInfo.Arguments = $"{scriptPath}.sh {string.Join(' ', args)}";
            }

            var taskCompletionSource = new TaskCompletionSource<Process>();

            var process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.Exited += (sender, eventArgs) => { taskCompletionSource.SetResult(process); };
            process.Start();

            return taskCompletionSource.Task;
        }
    }
}
