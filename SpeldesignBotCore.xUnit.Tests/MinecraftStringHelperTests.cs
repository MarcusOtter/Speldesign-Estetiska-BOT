using Xunit;
using SpeldesignBotCore.Modules.Minecraft.Helpers;
using SpeldesignBotCore.Modules.Minecraft.Entities;

namespace SpeldesignBotCore.xUnit.Tests
{
    public class MinecraftStringHelperTests
    {
        [Fact]
        public void MinecraftStringHelper_ToEnumString_CorrectOutputTest1()
        {
            const string input = "Mossy cobblestone Slab";
            const string expected = "MossyCobblestoneSlab";

            var actual = input.ToEnumString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToEnumString_CorrectOutputTest2()
        {
            const string input = "Damage dealt (Absorbed)";
            const string expected = "DamageDealtAbsorbed";

            var actual = input.ToEnumString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToEnumString_CorrectOutputTest3()
        {
            const string input = "Jumps";
            const string expected = "Jumps";

            var actual = input.ToEnumString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForItemTest()
        {
            const MinecraftItem input1 = MinecraftItem.MagmaCream;
            const string expected = "minecraft:magma_cream";

            var actual = input1.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForMobTest()
        {
            const MinecraftMob input1 = MinecraftMob.MagmaCube;
            const string expected = "minecraft:magma_cube";

            var actual = input1.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForActionTest()
        {
            const MinecraftAction input1 = MinecraftAction.PickedUp;
            const string expected = "minecraft:picked_up";

            var actual = input1.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForStatisticTest()
        {
            const MinecraftStatistic input1 = MinecraftStatistic.MobKills;
            const string expected = "minecraft:mob_kills";

            var actual = input1.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }
    }
}
