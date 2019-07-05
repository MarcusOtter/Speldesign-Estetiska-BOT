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
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForStringTest1()
        {
            const string input = "GoldenHorseArmor";
            const string expected = "minecraft:golden_horse_armor";

            var actual = input.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForStringTest2()
        {
            const string input = "MossyStoneBrickMonsterEgg";
            const string expected = "minecraft:mossy_stone_brick_monster_egg";

            var actual = input.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForStringTest3()
        {
            const string input = "Potato";
            const string expected = "minecraft:potato";

            var actual = input.ToMinecraftJsonString();

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
            const MinecraftStatisticAction input1 = MinecraftStatisticAction.PickedUp;
            const string expected = "minecraft:picked_up";

            var actual = input1.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MinecraftStringHelper_ToMinecraftJsonString_CorrectOutputForStatisticTest()
        {
            const MinecraftGeneralStatistic input1 = MinecraftGeneralStatistic.MobKills;
            const string expected = "minecraft:mob_kills";

            var actual = input1.ToMinecraftJsonString();

            Assert.Equal(expected, actual);
        }
    }
}
