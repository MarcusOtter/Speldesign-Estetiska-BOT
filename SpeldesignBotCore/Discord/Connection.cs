using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Discord.Entities;

namespace SpeldesignBotCore.Discord
{
    public class Connection
    {
        private readonly DiscordSocketClient _client;
        private readonly DiscordStatusLogger _statusLogger;
        private readonly DiscordMessageLogger _messageLogger;

        public Connection(DiscordSocketClient client, DiscordStatusLogger statusLogger, DiscordMessageLogger messageLogger)
        {
            _statusLogger = statusLogger;
            _client = client;
            _messageLogger = messageLogger;
        }

        internal async Task ConnectAsync(BotConfiguration config)
        {
            _client.Log += _statusLogger.Log;
            await _client.LoginAsync(TokenType.Bot, config.Token);
            await _client.StartAsync();

            _client.MessageReceived += TempDiscordMessageHandler;
            await _client.SetGameAsync("cute animals on imgur", type: ActivityType.Watching);

            await Task.Delay(-1);
        }


        private async Task TempDiscordMessageHandler(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);

            if (context.User.IsBot) return;

            _messageLogger.LogMsgSent(msg);

            // TODO: Move this channel ID to json instead of hardcoded
            //await LogMessageContents(context, 458197224489353239);

            if (context.Channel.Id == 458193387237933056)
            {
                await RegisterNewUser(context);
            }
        }

        private async Task RegisterNewUser(SocketCommandContext context)
        {
            string msg = context.Message.Content;
            SocketGuildUser socketUser = (SocketGuildUser) context.User;

            string[] splitMsg = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (splitMsg.Length < 3)
            {
                await SendRegistrationErrorMessage(context, "Too little information provided. Please follow the template:\n@SPE-- Firstname Lastname");
                return;
            }

            SocketRole roleToAdd;

            try
            {
                var requestedRoleId = Convert.ToUInt64(splitMsg[0]
                    .Replace("<", string.Empty).Replace("@", string.Empty)
                    .Replace("&", string.Empty).Replace(">", string.Empty));

                roleToAdd = context.Guild.GetRole(requestedRoleId);

                if (roleToAdd == null)
                {
                    throw new Exception();
                }
            }
            catch
            {
                await SendRegistrationErrorMessage(context, $"Your first word, '{splitMsg[0]}', is not a valid class role.\nMake sure to write '@' and then your class name. (3 capital letters followed by 2 numbers. Example: @SPE16)");
                return;
            }

            // TODO: Get this list from a json
            ulong[] validRoleIds =
            {
                458191879884898325,
                458192131874488330,
                458192128980549633
            };

            if (!validRoleIds.Contains(roleToAdd.Id))
            {
                await SendRegistrationErrorMessage(context, $"@{roleToAdd.Name} is not a valid class role.\nValid class role example: @SPE16");
                return;
            }

            await socketUser.AddRoleAsync(roleToAdd);

            List<string> names = splitMsg.ToList();
            names.RemoveAt(0);

            string fullName = string.Join(' ', names);

            // TODO: Add check to see if the user is in the class
            try
            {
                await socketUser.ModifyAsync(user => user.Nickname = fullName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task SendRegistrationErrorMessage(SocketCommandContext context, string exceptionMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(":warning: Whoops! :warning:\n");
            stringBuilder.Append($"It looks like you ({context.User.Mention}) did not follow the registration template correctly.\n");
            stringBuilder.Append($"\nError message:\n```fix\n{exceptionMessage}```");
            stringBuilder.Append("\nIf you believe that this is an error, send a message to `CalmEyE#8246 (Alexander Eriksson)` or `LeMorrow#8192 (Marcus Otterström).`");

            await context.Channel.SendMessageAsync(stringBuilder.ToString());
            Console.WriteLine($"WARNING: Unsuccessful registration by {context.User.Username}. Message: '{context.Message}'");
        }

        private async Task LogMessageContents(SocketCommandContext context, ulong channelId)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(context.User);
            embed.WithDescription(context.Message.Content);
            embed.WithFooter($"Skickat i #{context.Channel} av {((SocketGuildUser)context.User).Nickname}");

            var channel = (ISocketMessageChannel)_client.GetChannel(channelId);
            await channel.SendMessageAsync("", embed: embed.Build());
        }
    }
}
