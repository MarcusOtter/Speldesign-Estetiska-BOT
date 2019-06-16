using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class MiscellaneousCommands : ModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        [Summary("Shuts down the bot."), Remarks("shutdown")]
        [RequireOwner]
        public async Task Shutdown()
        {
            await ReplyAsync("Shutting down...");
            await (Context.Client as DiscordSocketClient).LogoutAsync();
            Environment.Exit(0);
        }
    }
}
