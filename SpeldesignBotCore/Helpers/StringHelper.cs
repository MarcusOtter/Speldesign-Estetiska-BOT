namespace SpeldesignBotCore.Helpers
{
    public static class StringHelper
    {
        public static string UppercaseFirstChar(this string input)
        {
            return string.IsNullOrEmpty(input) 
                ? string.Empty 
                : $"{char.ToUpperInvariant(input[0])}{input.Substring(1)}";
        }
    }
}