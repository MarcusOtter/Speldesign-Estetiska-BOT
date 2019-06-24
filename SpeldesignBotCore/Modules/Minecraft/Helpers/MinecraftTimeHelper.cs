using System;

namespace SpeldesignBotCore.Modules.Minecraft.Helpers
{
    public static class MinecraftTimeHelper
    {
        // Minecraft date strings comes in the following format: "2019-07-23 22:22:38 +0200"
        public static DateTime ToDateTime(this string minecraftDateString)
        {
            string[] minecraftDateSplit = minecraftDateString.Split(' ');

            // Removing the timezone, making the example "2019-07-23 22:22:38" (because DateTime cannot parse it)
            string minecraftDateWithoutTimezone = string.Join(' ', minecraftDateSplit[0], minecraftDateSplit[1]);

            return DateTime.ParseExact(minecraftDateWithoutTimezone, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Minecraft date strings comes in the following format: "2019-07-23 22:22:38 +0200"
        public static int GetUtcOffset(this string minecraftDateString)
        {
            string[] minecraftDateSplit = minecraftDateString.Split(' ');

            // The utc offset string is "+0200" in this case
            string minecraftDateUtcOffset = minecraftDateSplit[2];

            // Grabbing the 2nd and 3rd characters and parses it into an int. In this case "02" -> 2.
            int utcOffset = int.Parse(minecraftDateUtcOffset.Substring(1, 2));
            utcOffset *= minecraftDateUtcOffset[0] == '-' ? -1 : 1; // Make the offset negative if necessary.

            return utcOffset;
        }
    }
}
