using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Registration
{
    public class RegistrationHandler
    {
        private readonly BotConfiguration _botConfiguration;

        public RegistrationHandler()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        public async Task TryRegisterNewUser(SocketCommandContext context)
        {
            string msg = context.Message.Content;
            var socketUser = context.User as SocketGuildUser;

            string[] splitMsg = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (splitMsg.Length < 3)
            {
                await SendRegistrationErrorMessage(context, "I need more information than that.");
                return;
            }

            SocketRole roleToAdd;

            try
            {
                var requestedRoleId = Convert.ToUInt64(splitMsg[0]
                    .Replace("<", string.Empty).Replace("@", string.Empty)
                    .Replace("&", string.Empty).Replace(">", string.Empty));

                roleToAdd = context.Guild.GetRole(requestedRoleId);

                if (roleToAdd == null) { throw new Exception(); }
            }
            catch
            {
                await SendRegistrationErrorMessage(context, $"\"{splitMsg[0]}\" is not a valid class role.");
                return;
            }

            if (!_botConfiguration.SchoolClassesRoleIds.Contains(roleToAdd.Id))
            {
                await SendRegistrationErrorMessage(context, $"\"@{roleToAdd.Name}\" is not a valid class role.");
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
                Unity.Resolve<StatusLogger>().LogToConsole($"[EXCEPTION] Could not change nickname of user: {e.Message}");
            }
        }

        private async Task SendRegistrationErrorMessage(SocketCommandContext context, string exceptionMessage)
        {
            Unity.Resolve<StatusLogger>().LogToConsole($"Unsuccessful registration by {context.User.Username}. Message: '{context.Message}'");

            var embedBuilder = new EmbedBuilder()
                .WithTitle(exceptionMessage)
                .WithDescription("Follow the template:\n**@SPE-- Firstname Lastname**")
                .WithColor(255, 79, 79)
                .WithFooter("Think this an error? Send a message to @CalmEyE#8246 or @LeMorrow#8192");

            await context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
        }
    }
}
