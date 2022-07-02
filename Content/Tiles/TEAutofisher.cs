using ImproveGame.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Tiles
{
    public class TEAutofisher : ModTileEntity
    {
        internal Point16 locatePoint = Point16.NegativeOne;
        internal Item fishingPole = new();
        internal Item bait = new();
        internal Item[] fish = new Item[15];
        internal const int checkWidth = 50;
        internal const int checkHeight = 30;

        public override bool IsTileValidForEntity(int x, int y) {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Autofisher>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate) {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                //Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 2;
                int height = 2;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                //Sync the placement of the tile entity with other clients
                //The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
            }

            //ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            //Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            Point16 tileOrigin = new(1, 1);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;
        }

        public static Player GetClosestPlayer(Point16 Position) => Main.player[Player.FindClosest(new Vector2(Position.X * 16, Position.Y * 16), 1, 1)];

        public int FishingTimer;
        public override void Update() {
            if (Main.netMode != NetmodeID.Server && Main.netMode != NetmodeID.SinglePlayer)
                return;
            if (locatePoint.X < 0 || locatePoint.Y < 0)
                return;
            if (Framing.GetTileSafely(locatePoint).LiquidAmount == 0) {
                locatePoint = Point16.NegativeOne;
                return;
            }

            int finalFishingLevel = GetFishingConditions().FinalFishingLevel;
            if (Main.rand.Next(300) < finalFishingLevel)
                FishingTimer += Main.rand.Next(1, 3);

            FishingTimer += finalFishingLevel / 30;
            FishingTimer += Main.rand.Next(1, 3);
            if (Main.rand.NextBool(60))
                FishingTimer += 60;

            if (FishingTimer > 60f) {
                FishingTimer = 0;
                FishingCheck();
            }
        }

        public void FishingCheck() {
            var player = GetClosestPlayer(Position);

            FishingAttempt fisher = default(FishingAttempt);
            fisher.X = locatePoint.X;
            fisher.Y = locatePoint.Y;
            fisher.bobberType = ProjectileID.BobberGolden;
            GetFishingPondState(fisher.X, fisher.Y, out fisher.inLava, out fisher.inHoney, out fisher.waterTilesCount, out fisher.chumsInWater);

            fisher.playerFishingConditions = GetFishingConditions();
            if (fisher.playerFishingConditions.BaitItemType == ItemID.TruffleWorm) {
                // 召唤猪鲨 （？？？）
                return;
            }

            fisher.fishingLevel = fisher.playerFishingConditions.FinalFishingLevel;
            if (fisher.fishingLevel == 0)
                return;

            fisher.CanFishInLava = ItemID.Sets.CanFishInLava[fisher.playerFishingConditions.PoleItemType] || ItemID.Sets.IsLavaBait[fisher.playerFishingConditions.BaitItemType]/* || Main.player[owner].accLavaFishing*/;
            if (fisher.chumsInWater > 0)
                fisher.fishingLevel += 11;

            if (fisher.chumsInWater > 1)
                fisher.fishingLevel += 6;

            if (fisher.chumsInWater > 2)
                fisher.fishingLevel += 3;

            fisher.waterNeededToFish = 300;
            float num = Main.maxTilesX / 4200;
            num *= num;
            fisher.atmo = (float)((double)(Position.Y - (60f + 10f * num)) / (Main.worldSurface / 6.0));
            if ((double)fisher.atmo < 0.25)
                fisher.atmo = 0.25f;

            if (fisher.atmo > 1f)
                fisher.atmo = 1f;

            fisher.waterNeededToFish = (int)((float)fisher.waterNeededToFish * fisher.atmo);
            fisher.waterQuality = (float)fisher.waterTilesCount / (float)fisher.waterNeededToFish;
            if (fisher.waterQuality < 1f)
                fisher.fishingLevel = (int)((float)fisher.fishingLevel * fisher.waterQuality);

            fisher.waterQuality = 1f - fisher.waterQuality;

            if (player.active && !player.dead) {
                if (player.luck < 0f) {
                    if (Main.rand.NextFloat() < 0f - player.luck)
                        fisher.fishingLevel = (int)((double)fisher.fishingLevel * (0.9 - (double)Main.rand.NextFloat() * 0.3));
                }
                else if (Main.rand.NextFloat() < player.luck) {
                    fisher.fishingLevel = (int)((double)fisher.fishingLevel * (1.1 + (double)Main.rand.NextFloat() * 0.3));
                }
            }

            int fishChance = (fisher.fishingLevel + 75) / 2;
            if (Main.rand.Next(100) > fishChance)
                return;

            fisher.heightLevel = 0;
            if ((double)fisher.Y < Main.worldSurface * 0.5)
                fisher.heightLevel = 0;
            else if ((double)fisher.Y < Main.worldSurface)
                fisher.heightLevel = 1;
            else if ((double)fisher.Y < Main.rockLayer)
                fisher.heightLevel = 2;
            else if (fisher.Y < Main.maxTilesY - 300)
                fisher.heightLevel = 3;
            else
                fisher.heightLevel = 4;

            FishingCheck_RollDropLevels(player, fisher.fishingLevel, out fisher.common, out fisher.uncommon, out fisher.rare, out fisher.veryrare, out fisher.legendary, out fisher.crate);
            //FishingCheck_ProbeForQuestFish(ref fisher);
            //FishingCheck_RollEnemySpawns(ref fisher);

            // 伪装一个proj，用反射调用Projectile.FishingCheck_RollItemDrop
            var fakeProj = new Projectile {
                owner = 250
            };
            int tileType1 = Main.tile[Position.X, Position.Y + 2].TileType;
            int tileType2 = Main.tile[Position.X + 1, Position.Y + 2].TileType;

            bool dungeon = Main.player[250].ZoneDungeon;
            bool beach = Main.player[250].ZoneBeach;
            bool corrupt = Main.player[250].ZoneCorrupt;
            bool crimson = Main.player[250].ZoneCrimson;
            bool hallow = Main.player[250].ZoneHallow;
            bool jungle = Main.player[250].ZoneJungle;
            bool snow = Main.player[250].ZoneSnow;
            bool desert = Main.player[250].ZoneDesert;

            Main.player[250].ZoneDungeon = IsDungeonBrick(tileType1) || IsDungeonBrick(tileType2);
            Main.player[250].ZoneBeach = tileType1 == TileID.ShellPile || tileType2 == TileID.ShellPile;
            Main.player[250].ZoneCorrupt = TileID.Sets.Corrupt[tileType1] || TileID.Sets.Corrupt[tileType2];
            Main.player[250].ZoneCrimson = TileID.Sets.Crimson[tileType1] || TileID.Sets.Crimson[tileType2];
            Main.player[250].ZoneHallow = TileID.Sets.Hallow[tileType1] || TileID.Sets.Hallow[tileType2];
            Main.player[250].ZoneJungle = IsJungleTile(tileType1) || IsJungleTile(tileType2);
            Main.player[250].ZoneSnow = TileID.Sets.Snow[tileType1] || TileID.Sets.Snow[tileType2];
            Main.player[250].ZoneDesert = TileID.Sets.isDesertBiomeSand[tileType1] || TileID.Sets.isDesertBiomeSand[tileType2];
            Main.NewText(Main.player[250].ZoneDungeon);
            // 反射调用 FishingCheck_RollItemDrop(ref fisher);
            var targetMethod = fakeProj.GetType().GetMethod("FishingCheck_RollItemDrop",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var args = new object[] { fisher };
            targetMethod.Invoke(fakeProj, args);
            fisher = (FishingAttempt)args[0];
            Main.NewText($"[i:{fisher.rolledItemDrop}]");

            Main.player[250].ZoneDungeon = dungeon;
            Main.player[250].ZoneBeach = beach;
            Main.player[250].ZoneCorrupt = corrupt;
            Main.player[250].ZoneCrimson = crimson;
            Main.player[250].ZoneHallow = hallow;
            Main.player[250].ZoneJungle = jungle;
            Main.player[250].ZoneSnow = snow;
            Main.player[250].ZoneDesert = desert;
        }

        private static bool IsDungeonBrick(int type) => type == TileID.BlueDungeonBrick || type == TileID.PinkDungeonBrick || type == TileID.GreenDungeonBrick;

        private static bool IsJungleTile(int type) => type == TileID.JungleGrass || type == TileID.LihzahrdBrick;

        private static void FishingCheck_RollDropLevels(Player closetPlayer, int fishingLevel, out bool common, out bool uncommon, out bool rare, out bool veryrare, out bool legendary, out bool crate) {
            int commonChance = 150 / fishingLevel;
            int uncommonChance = 150 * 2 / fishingLevel;
            int rareChance = 150 * 7 / fishingLevel;
            int veryRareChance = 150 * 15 / fishingLevel;
            int legendaryChance = 150 * 30 / fishingLevel;
            int crateChance = 10;
            if (closetPlayer.cratePotion)
                crateChance += 10;

            if (commonChance < 2)
                commonChance = 2;

            if (uncommonChance < 3)
                uncommonChance = 3;

            if (rareChance < 4)
                rareChance = 4;

            if (veryRareChance < 5)
                veryRareChance = 5;

            if (legendaryChance < 6)
                legendaryChance = 6;

            common = false;
            uncommon = false;
            rare = false;
            veryrare = false;
            legendary = false;
            crate = false;
            if (Main.rand.NextBool(commonChance))
                common = true;

            if (Main.rand.NextBool(uncommonChance))
                uncommon = true;

            if (Main.rand.NextBool(rareChance))
                rare = true;

            if (Main.rand.NextBool(veryRareChance))
                veryrare = true;

            if (Main.rand.NextBool(legendaryChance))
                legendary = true;

            if (Main.rand.Next(100) < crateChance)
                crate = true;
        }

        private void GetFishingPondState(int x, int y, out bool lava, out bool honey, out int numWaters, out int chumCount) {
            chumCount = 0;

            lava = false;
            honey = false;
            for (int i = 0; i < tileChecked.GetLength(0); i++) {
                for (int j = 0; j < tileChecked.GetLength(1); j++) {
                    tileChecked[i, j] = false;
                }
            }
            numWaters = GetFishingPondSize(x, y, ref lava, ref honey);

            if (honey)
                numWaters = (int)((double)numWaters * 1.5);
        }

        private bool[,] tileChecked = new bool[checkWidth * 2 + 1, checkHeight * 2 + 1];
        private int GetFishingPondSize(int x, int y, ref bool lava, ref bool honey) {
            Point16 arrayLeftTop = new(Position.X + 1 - checkWidth, Position.Y + 1 - checkHeight);
            if (x - arrayLeftTop.X < 0 || x - arrayLeftTop.X > checkWidth * 2 || y - arrayLeftTop.Y < 0 || y - arrayLeftTop.Y > checkHeight * 2)
                return 0;
            if (tileChecked[x - arrayLeftTop.X, y - arrayLeftTop.Y])
                return 0;

            tileChecked[x - arrayLeftTop.X, y - arrayLeftTop.Y] = true;
            var tile = Framing.GetTileSafely(x, y);
            if (tile.LiquidAmount > 0 && !WorldGen.SolidTile(x, y)) {
                if (tile.LiquidType == LiquidID.Lava)
                    lava = true;
                if (tile.LiquidType == LiquidID.Honey)
                    honey = true;
                // 递归临近的四个物块
                int left = GetFishingPondSize(x - 1, y, ref lava, ref honey);
                int right = GetFishingPondSize(x + 1, y, ref lava, ref honey);
                int up = GetFishingPondSize(x, y - 1, ref lava, ref honey);
                int bottom = GetFishingPondSize(x, y + 1, ref lava, ref honey);
                return left + right + up + bottom + 1;
            }
            return 0;
        }

        public PlayerFishingConditions GetFishingConditions() {
            PlayerFishingConditions result = default;
            result.Pole = fishingPole;
            result.Bait = bait;
            if (result.BaitItemType == ItemID.TruffleWorm)
                return result;

            if (result.BaitPower == 0 || result.PolePower == 0)
                return result;

            var player = GetClosestPlayer(Position);
            int num = result.BaitPower + result.PolePower + player.fishingSkill;
            result.LevelMultipliers = Fishing_GetPowerMultiplier(result.Pole, result.Bait, player);
            result.FinalFishingLevel = (int)((float)num * result.LevelMultipliers);
            return result;
        }

        private float Fishing_GetPowerMultiplier(Item pole, Item bait, Player player) {
            float num = 1f;
            if (Main.raining)
                num *= 1.2f;

            if (Main.cloudBGAlpha > 0f)
                num *= 1.1f;

            if (Main.dayTime && (Main.time < 5400.0 || Main.time > 48600.0))
                num *= 1.3f;

            if (Main.dayTime && Main.time > 16200.0 && Main.time < 37800.0)
                num *= 0.8f;

            if (!Main.dayTime && Main.time > 6480.0 && Main.time < 25920.0)
                num *= 0.8f;

            if (Main.moonPhase == 0)
                num *= 1.1f;

            if (Main.moonPhase == 1 || Main.moonPhase == 7)
                num *= 1.05f;

            if (Main.moonPhase == 3 || Main.moonPhase == 5)
                num *= 0.95f;

            if (Main.moonPhase == 4)
                num *= 0.9f;

            if (Main.bloodMoon)
                num *= 1.1f;

            PlayerLoader.GetFishingLevel(player, pole, bait, ref num);
            return num;
        }

        public override void OnNetPlace() {
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public override void LoadData(TagCompound tag) {
            locatePoint = tag.Get<Point16>("locatePoint");
            fishingPole = tag.Get<Item>("fishingPole");
            bait = tag.Get<Item>("bait");
            for (int i = 0; i < 15; i++)
                if (tag.TryGet<Item>($"fish{i}", out var savedFish))
                    fish[i] = savedFish;
        }

        public override void SaveData(TagCompound tag) {
            tag["locatePoint"] = locatePoint;
            tag["fishingPole"] = fishingPole;
            tag["bait"] = bait;
            for (int i = 0; i < 15; i++)
                tag[$"fish{i}"] = fish[i];
        }

        public override void NetSend(BinaryWriter writer) {
            writer.Write(locatePoint.X);
            writer.Write(locatePoint.Y);
            ItemIO.Send(fishingPole, writer, true, false);
            ItemIO.Send(bait, writer, true, false);
            for (int i = 0; i < 15; i++)
                ItemIO.Send(fish[i], writer, true, false);
        }

        public override void NetReceive(BinaryReader reader) {
            locatePoint = new(reader.ReadInt16(), reader.ReadInt16());
            ItemIO.Receive(fishingPole, reader, true, false);
            ItemIO.Receive(bait, reader, true, false);
            for (int i = 0; i < 15; i++)
                ItemIO.Receive(fish[i], reader, true, false);
        }
    }
}
