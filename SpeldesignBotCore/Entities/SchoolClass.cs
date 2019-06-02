using System.Collections.Generic;

namespace SpeldesignBotCore.Entities
{
    public class SchoolClass
    {
        public readonly string Name;
        public readonly ulong RoleId;
        public readonly List<string> StudentNames;
        // teacher? to make DM announcements or something idk

        public SchoolClass(string name, ulong roleId, List<string> studentNames)
        {
            Name = name;
            RoleId = roleId;
            StudentNames = studentNames;
        }
    }
}
