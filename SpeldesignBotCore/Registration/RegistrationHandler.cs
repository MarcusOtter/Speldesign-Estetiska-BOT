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
        private readonly StatusLogger _statusLogger;


        public RegistrationHandler()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
            _statusLogger = Unity.Resolve<StatusLogger>();
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

            // If this role is not a school class role or the alumni role, don't give it to the user.
            if (!_botConfiguration.SchoolClassesRoleIds.Contains(roleToAdd.Id) && 
                _botConfiguration.AlumniRoleId != roleToAdd.Id)
            {
                await SendRegistrationErrorMessage(context, $"\"@{roleToAdd.Name}\" is not a valid class role.");
                return;
            }

            // Remove the role mention from the string
            List<string> names = splitMsg.ToList();
            names.RemoveAt(0);

            string fullName = string.Join(' ', names);

            string[] namesInClass = _botConfiguration.SchoolClasses
                                        .Where(x => x.RoleId == roleToAdd.Id)
                                        .FirstOrDefault()
                                        .StudentNames.ToArray();

            // If the name doesn't exist in this class, check for close matches in the class
            if (!namesInClass.Contains(fullName))
            {
                string[] closeMatches = namesInClass.FindClosestMatch(fullName, maxDistance: 5);

                if (closeMatches.Length == 0)
                {
                    await SendRegistrationErrorMessage(context, $"Could not find that name in {roleToAdd.Name}");
                }
                else
                {
                    await SendCloseMatchMessage(context, closeMatches);
                }

                return;
            }

            try
            {
                await socketUser.AddRoleAsync(roleToAdd);
                await socketUser.ModifyAsync(user => user.Nickname = fullName);
            }
            catch (Exception e)
            {
                // This usually means that the user is higher in the hierarchy than the bot.
                _statusLogger.LogToConsole($"[EXCEPTION] Could not modify user: {e.Message}");
                return;
            }

            _statusLogger.LogToConsole($"Successful registration by {context.User.Username}.");
        }

        private async Task SendCloseMatchMessage(SocketCommandContext context, string[] closeMatches)
        {
            // Filter out users that have already joined the guild
            closeMatches = closeMatches.Where(x => !context.Guild.Users.Any(y => y.Nickname == x)).ToArray();
            if (closeMatches.Length == 0)
            {
                await SendRegistrationErrorMessage(context, "That user has already joined.");
                return;
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Could not find that name.")
                .WithColor(255, 79, 79)
                .WithFooter("Think this an error? Send a message to @CalmEyE#8246 or @LeMorrow#8192")
                .WithDescription(closeMatches.Length == 1
                    ? $"Did you mean `{closeMatches[0]}`?"
                    : $"Did you mean one of these?\n`{string.Join("`\n`", closeMatches)}`");

            _statusLogger.LogToConsole($"Unsuccessful registration by {context.User.Username}. Message: '{context.Message}'");
            await context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
        }

        private async Task SendRegistrationErrorMessage(SocketCommandContext context, string exceptionMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(exceptionMessage)
                .WithDescription("Make sure you're following the template and spelling your name correctly.\n**@SPE-- Firstname Lastname**")
                .WithColor(255, 79, 79)
                .WithFooter("Think this an error? Send a message to @CalmEyE#8246 or @LeMorrow#8192");

            _statusLogger.LogToConsole($"Unsuccessful registration by {context.User.Username}. Message: '{context.Message}'");
            await context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
        }
    }
}
