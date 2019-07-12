using SpeldesignBotCore.Modules.Minecraft.Entities;
using System;
using System.Text;

namespace SpeldesignBotCore.Modules.Minecraft.Helpers
{
    public static class MinecraftStringHelper
    {
        public static string ToNamespacedId(this MinecraftStatistic statistic)
        {
            string id = "minecraft:";

            switch (statistic)
            {
                default:
                case MinecraftStatistic.AnimalsBred:
                case MinecraftStatistic.DamageAbsorbed:
                case MinecraftStatistic.DamageBlockedByShield:
                case MinecraftStatistic.DamageDealt:
                case MinecraftStatistic.DamageDealtAbsorbed:
                case MinecraftStatistic.DamageDealtResisted:
                case MinecraftStatistic.DamageResisted:
                case MinecraftStatistic.DamageTaken:
                case MinecraftStatistic.FishCaught:
                case MinecraftStatistic.MobKills:
                case MinecraftStatistic.PlayerKills:
                case MinecraftStatistic.SneakTime:
                    return statistic.ToMinecraftJsonString();

                case MinecraftStatistic.ArmorPiecesCleaned:               id += "clean_armor";                     break;
                case MinecraftStatistic.BannersCleaned:                   id += "clean_banner";                    break;
                case MinecraftStatistic.BarrelsOpened:                    id += "open_barrel";                     break;
                case MinecraftStatistic.BellsRung:                        id += "bell_ring";                       break;
                case MinecraftStatistic.CakeSlicesEaten:                  id += "eat_cake_slice";                  break;
                case MinecraftStatistic.CauldronsFilled:                  id += "fill_cauldron";                   break;
                case MinecraftStatistic.ChestsOpened:                     id += "open_chest";                      break;
                case MinecraftStatistic.DispensersSearched:               id += "inspect_dispenser";               break;
                case MinecraftStatistic.DistanceByBoat:                   id += "boat_one_cm";                     break;
                case MinecraftStatistic.DistanceByElytra:                 id += "aviate_one_cm";                   break;
                case MinecraftStatistic.DistanceByHorse:                  id += "horse_one_cm";                    break;
                case MinecraftStatistic.DistanceByMinecart:               id += "minecart_one_cm";                 break;
                case MinecraftStatistic.DistanceByPig:                    id += "pig_one_cm";                      break;
                case MinecraftStatistic.DistanceClimbed:                  id += "climb_one_cm";                    break;
                case MinecraftStatistic.DistanceCrouched:                 id += "crouch_one_cm";                   break;
                case MinecraftStatistic.DistanceFallen:                   id += "fall_one_cm";                     break;
                case MinecraftStatistic.DistanceFlown:                    id += "fly_one_cm";                      break;
                case MinecraftStatistic.DistanceSprinted:                 id += "sprint_one_cm";                   break;
                case MinecraftStatistic.DistanceSwum:                     id += "swim_one_cm";                     break;
                case MinecraftStatistic.DistanceWalked:                   id += "walk_one_cm";                     break;
                case MinecraftStatistic.DistanceWalkedOnWater:            id += "walk_on_water_one_cm";            break;
                case MinecraftStatistic.DistanceWalkedUnderWater:         id += "walk_under_water_one_cm";         break;
                case MinecraftStatistic.DroppersSearched:                 id += "inspect_dropper";                 break;
                case MinecraftStatistic.EnderChestsOpened:                id += "open_enderchest";                 break; // not ender_chest for arbitrary reasons
                case MinecraftStatistic.GamesQuit:                        id += "leave_game";                      break;
                case MinecraftStatistic.HoppersSearched:                  id += "inspect_hopper";                  break;
                case MinecraftStatistic.InteractionsWithBeacon:           id += "interact_with_beacon";            break;
                case MinecraftStatistic.InteractionsWithBlastFurnace:     id += "interact_with_blast_furnace";     break;
                case MinecraftStatistic.InteractionsWithBrewingStand:     id += "interact_with_brewingstand";      break; // not brewing_stand for arbitrary reasons
                case MinecraftStatistic.InteractionsWithCampfire:         id += "interact_with_campfire";          break;
                case MinecraftStatistic.InteractionsWithCartographyTable: id += "interact_with_cartography_table"; break;
                case MinecraftStatistic.InteractionsWithCraftingTable:    id += "interact_with_crafting_table";    break;
                case MinecraftStatistic.InteractionsWithFurnace:          id += "interact_with_furnace";           break;
                case MinecraftStatistic.InteractionsWithLectern:          id += "interact_with_lectern";           break;
                case MinecraftStatistic.InteractionsWithLoom:             id += "interact_with_loom";              break;
                case MinecraftStatistic.InteractionsWithSmoker:           id += "interact_with_smoker";            break;
                case MinecraftStatistic.InteractionsWithStonecutter:      id += "interact_with_stonecutter";       break;
                case MinecraftStatistic.ItemsDropped:                     id += "drop";                            break;
                case MinecraftStatistic.ItemsEnchanted:                   id += "enchant_item";                    break;
                case MinecraftStatistic.Jumps:                            id += "jump";                            break;
                case MinecraftStatistic.MusicDiscsPlayed:                 id += "play_record";                     break;
                case MinecraftStatistic.NoteBlocksPlayed:                 id += "play_noteblock";                  break;
                case MinecraftStatistic.NoteBlocksTuned:                  id += "tune_noteblock";                  break;
                case MinecraftStatistic.NumberOfDeaths:                   id += "deaths";                          break;
                case MinecraftStatistic.PlantsPotted:                     id += "pot_flower";                      break;
                case MinecraftStatistic.RaidsTriggered:                   id += "raid_trigger";                    break;
                case MinecraftStatistic.RaidsWon:                         id += "raid_win";                        break;
                case MinecraftStatistic.ShulkerBoxesCleaned:              id += "clean_shulker_box";               break;
                case MinecraftStatistic.ShulkerBoxesOpened:               id += "open_shulker_box";                break;
                case MinecraftStatistic.SinceLastDeath:                   id += "time_since_death";                break;
                case MinecraftStatistic.SinceLastRest:                    id += "time_since_rest";                 break;
                case MinecraftStatistic.TalkedToVillagers:                id += "talked_to_villager";              break;
                case MinecraftStatistic.TimePlayed:                       id += "play_one_minute";                 break;
                case MinecraftStatistic.TimesSleptInABed:                 id += "sleep_in_bed";                    break;
                case MinecraftStatistic.TradedWithVillagers:              id += "traded_with_villager";            break;
                case MinecraftStatistic.TrappedChestsTriggered:           id += "trigger_trapped_chest";           break;
                case MinecraftStatistic.WaterTakenFromCauldron:           id += "use_cauldron";                    break;
            }

            return id;
        }

        /// <summary> Turns a string like "Mossy cobblestone Slab" to "MossyCobblestoneSlab". Also removes quotes and parentheses.</summary>
        public static string ToEnumString(this string input)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].UppercaseFirstChar();
            }

            return string.Join("", words)
                .Replace("\"", string.Empty)
                .Replace("(",  string.Empty)
                .Replace(")",  string.Empty);
        }

        public static string ToReadableString<TEnum>(this TEnum minecraftItem) => minecraftItem.ToString().ToReadableString();

        /// <summary> Turns a string like "MossyCobblestoneSlab" to "mossy cobblestone slab" </summary>
        private static string ToReadableString(this string input)
        {
            var builder = new StringBuilder(input);
            int addedSpaces = 0;

            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];

                // We're not interested in already lowercase characters
                if (char.IsLower(character)) { continue; }

                // Make this uppercased character lowercase
                builder.Replace(input[i], char.ToLower(input[i]), i + addedSpaces, 1);

                if (i is 0) { continue; }

                // Insert a space before this character
                builder.Insert(i + addedSpaces, " ");
                addedSpaces++;
            }

            return builder.ToString();
        }

        public static string ToMinecraftJsonString<TEnum>(this TEnum minecraftItem) => minecraftItem.ToString().ToMinecraftJsonString();

        /// <summary> Turns a string like "MossyCobblestoneSlab" to "minecraft:mossy_cobblestone_slab"</summary>
        private static string ToMinecraftJsonString(this string input)
        {
            var builder = new StringBuilder(input);
            int addedUnderscores = 0;

            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];

                // We're not interested in already lowercase characters
                if (char.IsLower(character)) { continue; }

                // Make this uppercased character lowercase
                builder.Replace(input[i], char.ToLower(input[i]), i + addedUnderscores, 1);

                if (i is 0) { continue; }

                // Insert an underscore before this character
                builder.Insert(i + addedUnderscores, "_");
                addedUnderscores++;
            }

            builder.Insert(0, "minecraft:");
            return builder.ToString();
        }
    }
}
