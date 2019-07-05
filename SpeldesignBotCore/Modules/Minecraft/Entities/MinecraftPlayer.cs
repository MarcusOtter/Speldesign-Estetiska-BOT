using System;

namespace SpeldesignBotCore.Modules.Minecraft.Entities
{
    public class MinecraftPlayer
    {
        public readonly string Name;
        public readonly string Uuid;
        public readonly DateTimeOffset LastLogin;
        public readonly dynamic Stats;
        public readonly dynamic Advancements;

        public MinecraftPlayer(string name, string uuid, DateTimeOffset lastLogin, dynamic stats = null, dynamic advancements = null)
        {
            Name = name;
            Uuid = uuid;
            LastLogin = lastLogin;
            Stats = stats;
            Advancements = advancements;
        }
    }
}
