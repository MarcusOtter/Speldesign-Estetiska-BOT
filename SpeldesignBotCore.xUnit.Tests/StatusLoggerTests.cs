using SpeldesignBotCore.Loggers;
using Xunit;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class StatusLoggerTests
    {
        [Fact]
        public void StatusLoggerNotNullTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.NotNull(logger);
        }

        [Fact]
        public void StatusLoggerLogNullTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.Throws<System.ArgumentNullException>(() => logger.LogToConsole(null));
        }

        [Fact]
        public void StatusLoggerLogEmptyStringTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.Throws<System.ArgumentException>(() => logger.LogToConsole(""));
        }

        [Fact]
        public void StatusLoggerLogSpaceStringTest()
        {
            var logger = Unity.Resolve<StatusLogger>();
            Assert.Throws<System.ArgumentException>(() => logger.LogToConsole("    "));
        }
    }
}
