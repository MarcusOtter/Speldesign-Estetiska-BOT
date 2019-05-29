using Xunit;
using SpeldesignBotCore.Helpers;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class StringHelperTests
    {
        [Fact]
        public void StringHelper_UppercaseFirstChar_CorrectOutputTest()
        {
            const string input = "testing input";
            const string expected = "Testing input";

            var actual = input.UppercaseFirstChar();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringHelper_UppercaseFirstChar_NoCharactersTest()
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

        [Fact]
        public void StringHelper_FindClosestMatch_EmptyArrayTest()
        {
            string[] emptyArray = new string[0];
            Assert.Throws<System.ArgumentException>(() => emptyArray.FindClosestMatch("any input"));
        }

        [Fact]
        public void StringHelper_LevenshteinDistance_CorrectDistanceTest1()
        {
            const string inputA = "912345";
            const string inputB = "1234";

            const int expected = 2;
            var actual = inputA.LevenshteinDistance(inputB);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringHelper_LevenshteinDistance_CorrectDistanceTest2()
        {
            const string inputA = "abcdef";
            const string inputB = "azced";

            const int expected = 3;
            var actual = inputA.LevenshteinDistance(inputB);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringHelper_FindClosestMatch_CorrectMatchTest1()
        {
            string[] possibleOutputs = new string[]
            {
                "Hello world",
                "Hello",
                "world",
                "1234"
            };

            const string input = "912345";
            string[] expected = new string[] { "1234" };

            var actual = possibleOutputs.FindClosestMatch(input);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringHelper_FindClosestMatch_CorrectMatchTest2()
        {
            string[] possibleOutputs = new string[]
            {
                "Samuel Svensson",
                "Samuel Lundberg",
                "Samuel Manell"
            };

            const string input = "Sameull lndbreg";
            string[] expected = new string[] { "Samuel Lundberg" };
            var actual = expected.FindClosestMatch(input);

            Assert.Equal(expected, actual);
        }
    }
}
