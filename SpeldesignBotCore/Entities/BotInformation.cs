using System;

namespace SpeldesignBotCore.Entities
{
    public class BotInformation
    {
        public DateTime TimeOfPreviousStartup;
        public string OsName;
        public string DeviceName;

        public BotInformation(DateTime startupTime, string osName = null, string deviceName = null)
        {
            TimeOfPreviousStartup = startupTime;
            OsName = osName;
            DeviceName = deviceName;
        }
    }
}
