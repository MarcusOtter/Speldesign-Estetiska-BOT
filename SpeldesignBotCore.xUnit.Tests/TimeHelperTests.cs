using System;
using Xunit;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class TimeHelperTests
    {
        [Fact]
        public void TimeHelper_ToPrettyString_TimeSpan1()
        {
            var now = DateTime.Now;
            var soon = now.AddDays(43).AddHours(1).AddMinutes(0).AddSeconds(12);

            var input = soon - now;
            const string expected = "43 days 1 hour 12 seconds";
            var actual = input.ToPrettyString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TimeHelper_ToPrettyString_TimeSpan2()
        {
            var now = DateTime.Now;
            var soon = now.AddDays(0).AddHours(0).AddMinutes(21).AddSeconds(1);

            var input = soon - now;
            const string expected = "21 minutes 1 second";
            var actual = input.ToPrettyString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TimeHelper_ToPrettyString_TimeSpan3()
        {
            var input = new TimeSpan(days: 214, hours: 1, minutes: 1, seconds: 0);
            const string expected = "214 days 1 hour 1 minute";
            var actual = input.ToPrettyString();

            Assert.Equal(expected, actual);
        }
    }
}
