using Discord;
using Discord.Commands;
using SpeldesignBotCore.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class BotConfigurationCommand : ModuleBase<SocketCommandContext>
    {
        private readonly BotConfiguration _botConfiguration;

        public BotConfigurationCommand()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("addclass")]
        [Summary("Adds a new class to the list of school classes."), Remarks("addclass [@classrole] [comma separated names]")]
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
    }
}
