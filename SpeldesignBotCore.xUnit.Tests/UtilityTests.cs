using Xunit;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class UtilityTests
    {
        [Fact]
        public void DebugTest()
        {
            const int expected = 3;

            var actual = Utilities.MyUtility(expected);
            Assert.Equal(expected, actual + 1);
        }
    }
}
