using System;
using System.Collections.Generic;

namespace SpeldesignBotCore.Helpers
{
    public static class StringHelper
    {
        public static string UppercaseFirstChar(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException("Can not uppercase a string without characters");
            }
            else if (input[0] == ' ')
            {
                throw new ArgumentException("The first character cannot be whitespace");
            }

            return $"{char.ToUpperInvariant(input[0])}{input.Substring(1)}";
        }

        /// <summary>
        /// Returns the closest matching string(s) using the levenshtein distance method.
        /// WARNING: This function can take a very long time.
        /// </summary>
        /// <returns>
        /// The closest matching string(s) using the levenshtein distance method.
        /// If two strings have the same distance to the examined string, both are returned.
        /// </returns>
        public static string[] FindClosestMatch(this string[] allOutputStrings, string stringToMatch)
        {
            if (allOutputStrings.Length == 0) { throw new ArgumentException("Must have at least one output string"); }
            if (allOutputStrings.Length == 1) { return new string[1] { allOutputStrings[0] }; } // Return the only element if only one exists.

            List<string> closestMatches = new List<string>();
            int lowestDistance = 100;

            for (int i = 0; i < allOutputStrings.Length; i++)
            {
                int levenshteinDistance = allOutputStrings[i].LevenshteinDistance(stringToMatch);

                if (i == 0) { lowestDistance = levenshteinDistance; } // initialize lowestDistance

                if (levenshteinDistance > lowestDistance) { continue; }

                if (levenshteinDistance == lowestDistance)
                {
                    closestMatches.Add(allOutputStrings[i]);
                    continue;
                }

                if (levenshteinDistance < lowestDistance)
                {
                    closestMatches.Clear();
                    lowestDistance = levenshteinDistance;
                    closestMatches.Add(allOutputStrings[i]);
                }
            }

            return closestMatches.ToArray();
        }

        /// <summary>
        /// Calculates the minimum edit distance (levenshtein distance) between two strings.
        /// Time complexity is probably like O(ab), where a and b are the lengths of the inputs.
        /// </summary>
        public static int LevenshteinDistance(this string a, string b)
        {
            int[,] distanceTable = new int[a.Length, b.Length];

            // Set all distances to -1
            for (int x = 0; x < a.Length; x++)
            {
                for (int y = 0; y < b.Length; y++)
                {
                    distanceTable[x, y] = -1;
                }
            }

            return SubstringLevenshteinDistance(a, a.Length, b, b.Length, distanceTable);
        }

        /// <summary>
        /// Returns the levenshtein distance between a.Substring(0, aLength) and b.Substring(0, bLength).
        /// </summary>
        private static int SubstringLevenshteinDistance(string a, int aLength, string b, int bLength, int[,] distanceTable)
        {
            // If the length is <= 0 the substring will be empty, meaning the distance is the length of the other string.
            // e.g, the distance between "" and "hello" == 5 inserts
            if (aLength <= 0) { return bLength; }
            if (bLength <= 0) { return aLength; }

            // Used for 0-based indexing
            int aIndex = aLength - 1;
            int bIndex = bLength - 1;

            // If distance has not been calculated yet, calculate and set it
            if (distanceTable[aIndex, bIndex] == -1)
            {
                // If these characters are equal we can ignore them, since the distance between
                // (aLength, bLength) and (aLength -1, bLength -1) will be the same
                // e.g, "abcd9" and "efgh9" will have the same distance as "abcd" and "efgh"
                if (a[aIndex] == b[bIndex])
                {
                    distanceTable[aIndex, bIndex] = SubstringLevenshteinDistance(a, aLength - 1, b, bLength - 1, distanceTable);
                }
                else
                {
                    // This guy probably explains what's going on here better https://youtu.be/We3YDTzNXEk?t=193
                    int topLeftCell = SubstringLevenshteinDistance(a, aLength - 1, b, bLength - 1, distanceTable);
                    int leftCell    = SubstringLevenshteinDistance(a, aLength - 1, b, bLength,     distanceTable);
                    int topCell     = SubstringLevenshteinDistance(a, aLength,     b, bLength - 1, distanceTable);

                    // Set the distance to the smallest one of these and add 1
                    distanceTable[aIndex, bIndex] = Math.Min(topLeftCell, Math.Min(leftCell, topCell)) + 1;
                }
            }

            return distanceTable[aIndex, bIndex];
        }
    }
}
