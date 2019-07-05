using System;
using System.Linq;

namespace SpeldesignBotCore
{
    public static class EnumHelper
    {
        public static bool FindSimilarAndTryParse<TEnum>(string value, out TEnum result, int similarWordThreshold = 3, bool ignoreCase = true) where TEnum : struct
        {
            // First try parsing using the built-in TryParse
            var succesfulTryParse = Enum.TryParse(value, ignoreCase, out result);
            if (succesfulTryParse) { return true; }

            // If the first TryParse fails, find the closest matching enum name that are within the similarWordThreshold
            var enumNames = Enum.GetNames(typeof(TEnum));
            var closeMatch = enumNames.FindClosestMatch(value, similarWordThreshold).FirstOrDefault();

            if (closeMatch is null) { return false; }

            // If we find a match, we know it can be parsed.
            result = (TEnum) Enum.Parse(typeof(TEnum), closeMatch);
            return true;
        }
    }
}
