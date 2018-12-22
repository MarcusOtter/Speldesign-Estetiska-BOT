using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Loggers;

namespace SpeldesignBotCore
{
    public class DiscordMessageHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly BotConfiguration _botConfig;
        private readonly DiscordMessageLogger _messageLogger;
        private readonly DiscordCommandHandler _commandHandler;

        public DiscordMessageHandler(DiscordSocketClient client, BotConfiguration config, DiscordMessageLogger logger, DiscordCommandHandler commandHandler)
        {
            _client = client;
            _botConfig = config;
            _messageLogger = logger;
            _commandHandler = commandHandler;
        }

        public async Task HandleMessageAsync(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);

            if (context.User.IsBot) { return; }
            if (msg is null) { return; }

            await _messageLogger.LogToLoggingChannel(msg);

            if (context.Channel.Id == _botConfig.RegistrationChannelId)
            {
                await TryRegisterNewUser(context);
                return;
            }

            await _commandHandler.HandleCommand(msg, context);
        }
        
        private async Task TryRegisterNewUser(SocketCommandContext context)
        {
            string msg = context.Message.Content;
            var socketUser = context.User as SocketGuildUser;

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
                458192128980549633,
                489738222054670336
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

            try
            {
                await socketUser.ModifyAsync(user => user.Nickname = fullName);
            }
            catch (Exception e)
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
            Console.WriteLine($"Unsuccessful registration by {context.User.Username}. Message: '{context.Message}'");
        }
    }
}