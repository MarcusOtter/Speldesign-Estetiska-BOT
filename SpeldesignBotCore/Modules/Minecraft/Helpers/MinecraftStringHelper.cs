﻿using System;
using System.Text;

namespace SpeldesignBotCore.Modules.Minecraft.Helpers
{
    public static class MinecraftStringHelper
    {
        /// <summary> Turns a string like "Mossy cobblestone Slab" to "MossyCobblestoneSlab". Also removes quotes and parentheses.</summary>
        public static string ToEnumString(this string input)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].UppercaseFirstChar();
            }

            return string.Join("", words)
                .Replace("\"", string.Empty)
                .Replace("(",  string.Empty)
                .Replace(")",  string.Empty);
        }

        public static string ToReadableString<TEnum>(this TEnum minecraftItem) => minecraftItem.ToString().ToReadableString();

        /// <summary> Turns a string like "MossyCobblestoneSlab" to "mossy cobblestone slab" </summary>
        private static string ToReadableString(this string input)
        {
            var builder = new StringBuilder(input);
            int addedSpaces = 0;

            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];

                // We're not interested in already lowercase characters
                if (char.IsLower(character)) { continue; }

                // Make this uppercased character lowercase
                builder.Replace(input[i], char.ToLower(input[i]), i + addedSpaces, 1);

                if (i is 0) { continue; }

                // Insert a space before this character
                builder.Insert(i + addedSpaces, " ");
                addedSpaces++;
            }

            return builder.ToString();
        }

        public static string ToMinecraftJsonString<TEnum>(this TEnum minecraftItem) => minecraftItem.ToString().ToMinecraftJsonString();

        /// <summary> Turns a string like "MossyCobblestoneSlab" to "minecraft:mossy_cobblestone_slab"</summary>
        private static string ToMinecraftJsonString(this string input)
        {
            var builder = new StringBuilder(input);
            int addedUnderscores = 0;

            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];

                // We're not interested in already lowercase characters
                if (char.IsLower(character)) { continue; }

                // Make this uppercased character lowercase
                builder.Replace(input[i], char.ToLower(input[i]), i + addedUnderscores, 1);

                if (i is 0) { continue; }

                // Insert an underscore before this character
                builder.Insert(i + addedUnderscores, "_");
                addedUnderscores++;
            }

            builder.Insert(0, "minecraft:");
            return builder.ToString();
        }
    }
}
