namespace SpeldesignBotCore.Modules.Minecraft.Entities
{
    public class MinecraftServerConfig
    {
        public string Host;
        public int Port;
        public string Username;
        public string Password;
        public string WorldName;

        // Todo: remove this ctor
        public MinecraftServerConfig(string host, int port, string username, string password, string worldName)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            WorldName = worldName;
        }
    }
}
