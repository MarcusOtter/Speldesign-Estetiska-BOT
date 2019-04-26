using Xunit;
using SpeldesignBotCore.Helpers;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class StringHelperTests
    {
        [Fact]
        public void StringHelperUppercaseFirstLetterTest()
        {
            const string input = "testing input";
            const string expected = "Testing input";

            var actual = input.UppercaseFirstChar();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringHelperUppercaseNoCharacterTest()
        {
            const string emptyInput = "";
            Assert.Throws<System.ArgumentNullException>(() => emptyInput.UppercaseFirstChar());

            const string nullInput = null;
            Assert.Throws<System.ArgumentNullException>(() => nullInput.UppercaseFirstChar());

            const string whitespaceInput = "    ";
            Assert.Throws<System.ArgumentNullException>(() => whitespaceInput.UppercaseFirstChar());

            const string firstCharacterWhitespaceInput = "  test";
            Assert.Throws<System.ArgumentException>(() => firstCharacterWhitespaceInput.UppercaseFirstChar());
        }
    }
}
