namespace SpeldesignBotCore.Helpers
{
    public static class StringHelper
    {
        public static string UppercaseFirstChar(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new System.ArgumentNullException("Can not uppercase a string without characters");
            }
            else if (input[0] == ' ')
            {
                throw new System.ArgumentException("The first character cannot be whitespace");
            }

            return $"{char.ToUpperInvariant(input[0])}{input.Substring(1)}";
        }
    }
}
