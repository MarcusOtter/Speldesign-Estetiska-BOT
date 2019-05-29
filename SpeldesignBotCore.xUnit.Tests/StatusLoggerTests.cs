using SpeldesignBotCore.Loggers;
using Xunit;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class StatusLoggerTests
    {
        [Fact]
        public void StatusLogger_NotNullTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.NotNull(logger);
        }

        [Fact]
        public void StatusLogger_LogToConsole_LogNullTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.Throws<System.ArgumentNullException>(() => logger.LogToConsole(null));
        }

        [Fact]
        public void StatusLogger_LogToConsole_LogEmptyStringTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.Throws<System.ArgumentException>(() => logger.LogToConsole(""));
        }

        [Fact]
        public void StatusLogger_LogToConsole_LogWhitespaceTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.Throws<System.ArgumentException>(() => logger.LogToConsole("    "));
        }
    }
}
