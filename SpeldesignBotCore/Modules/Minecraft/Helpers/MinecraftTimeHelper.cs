using System;

namespace SpeldesignBotCore.Modules.Minecraft.Helpers
{
    public static class MinecraftTimeHelper
    {
        /// <summary> Turns a minecraft date string (example: "2019-07-23 22:22:38 +0200") to a DateTimeOffset. </summary> 
        public static DateTimeOffset MinecraftDateToDateTimeOffset(this string minecraftDateString)
        {
            string[] minecraftDateSplit = minecraftDateString.Split(' ');

            // Removing the timezone, making the example "2019-07-23 22:22:38" (because DateTime cannot parse it)
            string minecraftDateWithoutTimezone = string.Join(' ', minecraftDateSplit[0], minecraftDateSplit[1]);

            var dateTime = DateTime.ParseExact(minecraftDateWithoutTimezone, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            return new DateTimeOffset(dateTime, GetMinecraftDateUtcOffset(minecraftDateString));
        }

        /// <summary>
        /// Returns the UTC offset from a minecraft date string.
        /// For example, it returns a timespan with +2 hours from "2019-07-23 22:22:38 +0200".
        /// </summary>
        private static TimeSpan GetMinecraftDateUtcOffset(string minecraftDateString)
        {
            string[] minecraftDateSplit = minecraftDateString.Split(' ');

            // The utc offset string is "+0200" in the example case
            string minecraftDateUtcOffset = minecraftDateSplit[2];

            // Grabbing the 2nd and 3rd characters and parses it into an int. In the example case "02" -> 2.
            int utcOffset = int.Parse(minecraftDateUtcOffset.Substring(1, 2));
            utcOffset *= minecraftDateUtcOffset[0] == '-' ? -1 : 1; // Make the offset negative if necessary.

            return new TimeSpan(hours: utcOffset, minutes: 0, seconds: 0);
        }
    }
}
