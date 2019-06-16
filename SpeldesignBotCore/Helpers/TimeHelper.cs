using System;
using System.Text;

namespace SpeldesignBotCore
{
    public static class TimeHelper
    {
        /// <summary> Formats the timespan as "A days B hours C minutes D seconds" with proper pluralization. </summary>
        public static string ToPrettyString(this TimeSpan span)
        {
            if (span == TimeSpan.Zero) { return "0 seconds"; }

            var builder = new StringBuilder();

            if (span.Days > 0)    { builder.Append($"{span.Days} day{      (span.Days > 1    ? "s" : string.Empty)} "); }
            if (span.Hours > 0)   { builder.Append($"{span.Hours} hour{    (span.Hours > 1   ? "s" : string.Empty)} "); }
            if (span.Minutes > 0) { builder.Append($"{span.Minutes} minute{(span.Minutes > 1 ? "s" : string.Empty)} "); }
            if (span.Seconds > 0) { builder.Append($"{span.Seconds} second{(span.Seconds > 1 ? "s" : string.Empty)} "); }

            return builder.ToString().Trim();
        }
    }
}
