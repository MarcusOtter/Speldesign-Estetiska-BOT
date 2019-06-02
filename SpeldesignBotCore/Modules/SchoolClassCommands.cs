using Discord;
using Discord.Commands;
using SpeldesignBotCore.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class SchoolClassCommands : ModuleBase<SocketCommandContext>
    {
        private readonly BotConfiguration _botConfiguration;

        public SchoolClassCommands()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("addclass")]
        [Summary("Adds a new class to the list of school classes."), Remarks("addclass [@class] [comma separated names]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task AddSchoolClass(IRole classRole, [Remainder] string names)
        {
            if (_botConfiguration.SchoolClassesRoleIds.Contains(classRole.Id) ||
                _botConfiguration.AlumniRoleId == classRole.Id)
            {
                await ReplyAsync("That class already exists.");
                return;
            }

            // Create a string[] of the students' names, correctly formatted.
            var studentNames = names.Split(',');
            for (int i = 0; i < studentNames.Length; i++)
            {
                studentNames[i] = studentNames[i].Trim();
            }

            var newClass = new SchoolClass(classRole.Name, classRole.Id, studentNames.ToList());
            _botConfiguration.SchoolClasses.Add(newClass);
            _botConfiguration.Save();

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Success!")
                .WithDescription($"Added class {classRole.Mention} with {newClass.StudentNames.Count} students.")
                .WithColor(118, 196, 177)
                .WithFooter($"Something wrong? {_botConfiguration.Prefix}removeclass and {_botConfiguration.Prefix}classes to the rescue!");

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("removeclass")]
        [Summary("Permanently removes an existing class from the list of school classes."), Remarks("removeclass [@class]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task RemoveSchoolClass(IRole classRole)
        {
            if (!_botConfiguration.SchoolClassesRoleIds.Contains(classRole.Id))
            {
                await ReplyAsync("That class does not exist.");
                return;
            }

            var classToRemove = _botConfiguration.SchoolClasses.FirstOrDefault(x => x.RoleId == classRole.Id);
            _botConfiguration.SchoolClasses.Remove(classToRemove);
            _botConfiguration.Save();

            // Would be very nice with a confirmation here. Make sure they didn't mean to graduate the class.

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Success!")
                .WithDescription($"Removed class {classRole.Mention}.")
                .WithColor(118, 196, 177)
                .WithFooter($"{_botConfiguration.Prefix}classes to see all remaining classes.");

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("classes")]
        [Summary("Lists all the classes."), Remarks("classes")]
        public async Task ListClasses()
        {
            var embedBuilder = new EmbedBuilder()
            {
                Title = "All classes",
                Color = new Color(118, 196, 177)
            };

            foreach (var schoolClass in _botConfiguration.SchoolClasses)
            {
                var classRole = Context.Guild.GetRole(schoolClass.RoleId);
                int studentsInServer = Context.Guild.Users.Where(x => x.Roles.Contains(classRole)).Count();
                int studentsInClass = schoolClass.StudentNames.Count;

                float studentsInServerProportion = (studentsInServer / (float) studentsInClass) * 100f;

                embedBuilder.AddField(schoolClass.Name, 
                    $"{classRole.Mention} students joined: {studentsInServer}/{studentsInClass} ({studentsInServerProportion.ToString("0.##")}%)");
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("remainingStudents")]
        [Summary("Lists the students that haven't joined from a class"), Remarks("remainingStudents [@class]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task RemainingStudents(IRole classRole)
        {
            if (!_botConfiguration.SchoolClassesRoleIds.Contains(classRole.Id))
            {
                await ReplyAsync("That class does not exist.");
                return;
            }

            // Get the names of all the students in the class
            var allNames = _botConfiguration.SchoolClasses
                .First(x => x.RoleId == classRole.Id)
                .StudentNames;

            // Get the names of all the students in this class on the server
            var studentsInServer = Context.Guild.Users
                .Where(x => x.Roles.Contains(classRole))
                .Select(x => x.Nickname)
                .ToArray();
            
            // Find the names that are not in the server
            var remainingStudents = allNames
                .Where(x => !studentsInServer.Contains(x))
                .ToArray();

            if (!remainingStudents.Any())
            {
                await ReplyAsync($"There are no remaining students from {classRole.Name}!");
                return;
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Students remaining from {classRole.Name}")
                .WithDescription(string.Join(", ", remainingStudents))
                .WithColor(118, 196, 177);

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            try
            {
                await dmChannel.SendMessageAsync("", embed: embedBuilder.Build());
            }
            // If the user does not allow direct messages from this server, deny access
            catch (Discord.Net.HttpException)
            {
                await ReplyAsync(
                    "You need to allow DMs from this server so the names can be sent to you in private.\n\n" +
                    "To enable:\n" +
                    $"Click the `{Context.Guild.Name}` dropdown (top left corner)\n" +
                    "Click `Privacy Settings`\n" +
                    "Enable `Allow direct messages from server members`\n");

                return;
            }

            if (!Context.IsPrivate)
            {
                await ReplyAsync("Sent a list of the names to your DMs");
            }
        }
    }
}
