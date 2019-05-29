using SpeldesignBotCore.Storage;
using Xunit;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class UnityTests
    {
        [Fact]
        public void Unity_ResolveTwoObjectsTest()
        {
            var storage1 = Unity.Resolve<IDataStorage>();
            var storage2 = Unity.Resolve<IDataStorage>();

            Assert.NotNull(storage1);
            Assert.NotNull(storage2);
            Assert.Same(storage1, storage2);
        }
    }
}
