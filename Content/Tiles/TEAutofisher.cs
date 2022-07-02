using ImproveGame.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
            if (Main.netMode != NetmodeID.Server)
                return;

            int finalFishingLevel = GetFishingConditions().FinalFishingLevel;
            if (Main.rand.Next(300) < finalFishingLevel)
                FishingTimer += Main.rand.Next(1, 3);

            FishingTimer += finalFishingLevel / 30;
            FishingTimer += Main.rand.Next(1, 3);
            if (Main.rand.NextBool(60))
                FishingTimer += 60;

            if (FishingTimer > 660f) {
                FishingTimer = 0;
                //FishingCheck();
            }
        }

        //public void FishingCheck() {
        //    FishingAttempt fisher = default(FishingAttempt);
        //    fisher.X = Position.X;
        //    fisher.Y = Position.Y;
        //    fisher.bobberType = type;
        //    GetFishingPondState(fisher.X, fisher.Y, out fisher.inLava, out fisher.inHoney, out fisher.waterTilesCount, out fisher.chumsInWater);

        //    fisher.playerFishingConditions = Main.player[owner].GetFishingConditions();
        //    if (fisher.playerFishingConditions.BaitItemType == 2673) {
        //        Main.player[owner].displayedFishingInfo = Language.GetTextValue("GameUI.FishingWarning");
        //        if ((fisher.X < 380 || fisher.X > Main.maxTilesX - 380) && fisher.waterTilesCount > 1000 && !NPC.AnyNPCs(370)) {
        //            ai[1] = Main.rand.Next(-180, -60) - 100;
        //            localAI[1] = 1f;
        //            netUpdate = true;
        //        }

        //        return;
        //    }

        //    fisher.fishingLevel = fisher.playerFishingConditions.FinalFishingLevel;
        //    if (fisher.fishingLevel == 0)
        //        return;

        //    fisher.CanFishInLava = (ItemID.Sets.CanFishInLava[fisher.playerFishingConditions.PoleItemType] || ItemID.Sets.IsLavaBait[fisher.playerFishingConditions.BaitItemType] || Main.player[owner].accLavaFishing);
        //    if (fisher.chumsInWater > 0)
        //        fisher.fishingLevel += 11;

        //    if (fisher.chumsInWater > 1)
        //        fisher.fishingLevel += 6;

        //    if (fisher.chumsInWater > 2)
        //        fisher.fishingLevel += 3;

        //    Main.player[owner].displayedFishingInfo = Language.GetTextValue("GameUI.FishingPower", fisher.fishingLevel);
        //    fisher.waterNeededToFish = 300;
        //    float num = Main.maxTilesX / 4200;
        //    num *= num;
        //    fisher.atmo = (float)((double)(position.Y / 16f - (60f + 10f * num)) / (Main.worldSurface / 6.0));
        //    if ((double)fisher.atmo < 0.25)
        //        fisher.atmo = 0.25f;

        //    if (fisher.atmo > 1f)
        //        fisher.atmo = 1f;

        //    fisher.waterNeededToFish = (int)((float)fisher.waterNeededToFish * fisher.atmo);
        //    fisher.waterQuality = (float)fisher.waterTilesCount / (float)fisher.waterNeededToFish;
        //    if (fisher.waterQuality < 1f)
        //        fisher.fishingLevel = (int)((float)fisher.fishingLevel * fisher.waterQuality);

        //    fisher.waterQuality = 1f - fisher.waterQuality;
        //    if (fisher.waterTilesCount < fisher.waterNeededToFish)
        //        Main.player[owner].displayedFishingInfo = Language.GetTextValue("GameUI.FullFishingPower", fisher.fishingLevel, 0.0 - Math.Round(fisher.waterQuality * 100f));

        //    if (Main.player[owner].luck < 0f) {
        //        if (Main.rand.NextFloat() < 0f - Main.player[owner].luck)
        //            fisher.fishingLevel = (int)((double)fisher.fishingLevel * (0.9 - (double)Main.rand.NextFloat() * 0.3));
        //    }
        //    else if (Main.rand.NextFloat() < Main.player[owner].luck) {
        //        fisher.fishingLevel = (int)((double)fisher.fishingLevel * (1.1 + (double)Main.rand.NextFloat() * 0.3));
        //    }

        //    int num2 = (fisher.fishingLevel + 75) / 2;
        //    if (Main.rand.Next(100) > num2)
        //        return;

        //    fisher.heightLevel = 0;
        //    if ((double)fisher.Y < Main.worldSurface * 0.5)
        //        fisher.heightLevel = 0;
        //    else if ((double)fisher.Y < Main.worldSurface)
        //        fisher.heightLevel = 1;
        //    else if ((double)fisher.Y < Main.rockLayer)
        //        fisher.heightLevel = 2;
        //    else if (fisher.Y < Main.maxTilesY - 300)
        //        fisher.heightLevel = 3;
        //    else
        //        fisher.heightLevel = 4;

        //    FishingCheck_RollDropLevels(fisher.fishingLevel, out fisher.common, out fisher.uncommon, out fisher.rare, out fisher.veryrare, out fisher.legendary, out fisher.crate);
        //    FishingCheck_ProbeForQuestFish(ref fisher);
        //    FishingCheck_RollEnemySpawns(ref fisher);
        //    FishingCheck_RollItemDrop(ref fisher);
        //    bool flag = false;
        //    AdvancedPopupRequest sonar = new AdvancedPopupRequest();
        //    //Bobber position as default
        //    Vector2 sonarPosition = new Vector2(position.X, position.Y);
        //    PlayerLoader.CatchFish(Main.player[owner], fisher, ref fisher.rolledItemDrop, ref fisher.rolledEnemySpawn, ref sonar, ref sonarPosition);

        //    if (sonar.Text != null && Main.player[owner].sonarPotion) {
        //        PopupText.AssignAsSonarText(PopupText.NewText(sonar, sonarPosition));
        //    }

        //    if (fisher.rolledItemDrop > 0) {
        //        if (sonar.Text == null && Main.player[owner].sonarPotion) {
        //            Item item = new Item();
        //            item.SetDefaults(fisher.rolledItemDrop);
        //            item.position = position;
        //            PopupText.AssignAsSonarText(PopupText.NewText(PopupTextContext.SonarAlert, item, 1, noStack: true));
        //        }

        //        float num3 = fisher.fishingLevel;
        //        ai[1] = (float)Main.rand.Next(-240, -90) - num3;
        //        localAI[1] = fisher.rolledItemDrop;
        //        netUpdate = true;
        //        flag = true;
        //    }

        //    if (fisher.rolledEnemySpawn > 0) {
        //        if (sonar.Text == null && Main.player[owner].sonarPotion)
        //            PopupText.AssignAsSonarText(PopupText.NewText(PopupTextContext.SonarAlert, fisher.rolledEnemySpawn, base.Center, stay5TimesLonger: false));

        //        float num4 = fisher.fishingLevel;
        //        ai[1] = (float)Main.rand.Next(-240, -90) - num4;
        //        localAI[1] = -fisher.rolledEnemySpawn;
        //        netUpdate = true;
        //        flag = true;
        //    }

        //    if (!flag && fisher.inLava) {
        //        int num5 = 0;
        //        if (ItemID.Sets.IsLavaBait[fisher.playerFishingConditions.BaitItemType])
        //            num5++;

        //        if (ItemID.Sets.CanFishInLava[fisher.playerFishingConditions.PoleItemType])
        //            num5++;

        //        if (Main.player[owner].accLavaFishing)
        //            num5++;

        //        if (num5 >= 2)
        //            localAI[1] += 240f;
        //    }

        //    if (fisher.CanFishInLava && fisher.inLava)
        //        AchievementsHelper.HandleSpecialEvent(Main.player[owner], 19);
        //}

        //private static void GetFishingPondState(int x, int y, out bool lava, out bool honey, out int numWaters, out int chumCount) {
        //    lava = false;
        //    honey = false;
        //    numWaters = 0;
        //    chumCount = 0;
        //    Point tileCoords = new Point(0, 0);
        //    GetFishingPondWidth(x, y, out int minX, out int maxX);
        //    for (int i = minX; i <= maxX; i++) {
        //        int num = y;
        //        while (Main.tile[i, num].liquid > 0 && !WorldGen.SolidTile(i, num) && num < Main.maxTilesY - 10) {
        //            numWaters++;
        //            num++;
        //            //patch file: flag, num4
        //            if (Main.tile[i, num].lava())
        //                lava = true;
        //            else if (Main.tile[i, num].honey())
        //                //patch file: flag2
        //                honey = true;

        //            tileCoords.X = i;
        //            tileCoords.Y = num;
        //            chumCount += Main.instance.ChumBucketProjectileHelper.GetChumsInLocation(tileCoords);
        //        }
        //    }

        //    void RecursionSetHovered(int i, int j) {
        //        if (tileChecked[i - drawRange.X, j - drawRange.Y])
        //            return;
        //        if (Math.Abs(fisherPos.X - i) > width || Math.Abs(fisherPos.Y - j) > height)
        //            return;

        //        tileChecked[i - drawRange.X, j - drawRange.Y] = true;
        //        var tile = Framing.GetTileSafely(i, j);
        //        if (tile.LiquidAmount > 0 && !WorldGen.SolidTile(i, j) && i >= drawRange.Left && j >= drawRange.Top && i <= drawRange.Right && j <= drawRange.Bottom) {
        //            mouseHovering[i - drawRange.X, j - drawRange.Y] = true;
        //            // 递归临近的四个物块
        //            RecursionSetHovered(i - 1, j);
        //            RecursionSetHovered(i + 1, j);
        //            RecursionSetHovered(i, j - 1);
        //            RecursionSetHovered(i, j + 1);
        //            return;
        //        }
        //        return; // 虽然不是必要的，但是写上感觉规范点
        //    }

        //    if (honey)
        //        numWaters = (int)((double)numWaters * 1.5);
        //}

        private static void GetFishingPondWidth(int x, int y, out int minX, out int maxX) {
            minX = x;
            maxX = x;
            while (minX > 10 && Main.tile[minX, y].LiquidType > 0 && !WorldGen.SolidTile(minX, y)) {
                minX--;
            }

            while (maxX < Main.maxTilesX - 10 && Main.tile[maxX, y].LiquidType > 0 && !WorldGen.SolidTile(maxX, y)) {
                maxX++;
            }
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
