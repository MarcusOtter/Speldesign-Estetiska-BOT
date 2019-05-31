using System;

namespace SpeldesignBotCore
{
    // WARNING: This attribute is currently not being used and will be removed soon.

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CommandInfoAttribute : Attribute
    {
        /// <summary>Explains how to use the command. Omit square brackets.</summary>
        public readonly string Usage;

        /// <summary>Explains what the command does.</summary>
        public readonly string Description;

        /// <param name="usage">Explains how to use the command. Omit square brackets.</param>
        /// <param name="description">Explains what the command does.</param>
        public CommandInfoAttribute(string usage, string description)
        {
            Usage = usage;
            Description = description;
        }
    }
}
