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
            foreach(var name in studentNames) { name.Trim(); }

            var newClass = new SchoolClass(classRole.Name, classRole.Id, studentNames.ToList());
            _botConfiguration.SchoolClasses.Add(newClass);
            _botConfiguration.Save();
        }
    }
}
