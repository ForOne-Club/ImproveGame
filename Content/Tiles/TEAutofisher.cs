using ImproveGame.Common.Systems;
using System.IO;
using System.Reflection;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Tiles
{
    public class TEAutofisher : ModTileEntity
    {
        internal Point16 locatePoint = Point16.NegativeOne;
        internal Item fishingPole = new();
        internal Item bait = new();
        internal Item accessory = new();
        internal Item[] fish = new Item[15];
        internal const int checkWidth = 50;
        internal const int checkHeight = 30;

        internal string FishingTip { get; private set; } = "Error";
        internal double FishingTipTimer { get; private set; } = 0;
        internal bool Opened = false;
        internal int OpenAnimationTimer = 0;

        public override bool IsTileValidForEntity(int x, int y) {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Autofisher>();
        }

        public void SetFishingTip(string text) {
            FishingTip = text;
            FishingTipTimer = 0;
            if (Main.netMode == NetmodeID.Server)
                NetAutofish.ServerSendTipChange(Position, FishingTip);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate) {
            for (int k = 0; k < 15; k++) {
                fish[k] = new();
            }
            fishingPole = new();
            bait = new();
            accessory = new();

            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 2, 2);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i - 1, j - 1, Type);
                return -1;
            }

            int placedEntity = Place(i - 1, j - 1);
            return placedEntity;
        }

        public static int Hook_AfterPlacement_NoEntity(int i, int j, int type, int style, int direction, int alternate) {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 2, 2);
                return 0;
            }
            return 0;
        }

        public static Player GetClosestPlayer(Point16 Position) => Main.player[Player.FindClosest(new Vector2(Position.X * 16, Position.Y * 16), 1, 1)];

        #region 钓鱼

        public int FishingTimer;
        public override void Update() {
            FishingTipTimer += 1.0 / 60.0;
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

            float fishingSpeedBonus = 1f;

            //配饰为AnglerEarring可使钓鱼速度*200%
            //配饰为AnglerTackleBag可使钓鱼速度*300%
            //配饰为LavaproofTackleBag可使钓鱼速度*500%
            switch (accessory.type)
            {
                case ItemID.AnglerEarring:
                    fishingSpeedBonus = 2f;
                    break;
                case ItemID.AnglerTackleBag:
                    fishingSpeedBonus = 3f;
                    break;
                case ItemID.LavaproofTackleBag:
                    fishingSpeedBonus = 5f;
                    break;
            }

            // 存储的 Bass 将以 20:1 的比例转化为钓鱼速度加成，最高可达 500% 加成
            int bassCount = 0;
            for (int i = 0; i < 15; i++) {
            if (fish[i].type == ItemID.Bass) {
                    bassCount += fish[i].stack;
            }
            }
            fishingSpeedBonus += Math.Min(bassCount / 20f, 5f);

            float fishingCooldown = 6600f; // 钓鱼机基础冷却在这里改，原版写的是660
            if (FishingTimer > fishingCooldown / fishingSpeedBonus) {
                FishingTimer = 0;
                ApplyAccessories();
                FishingCheck();
            }
        }

        private bool lavaFishing = false;
        private bool tackleBox = false;
        private int fishingSkill = 0;

        private void ApplyAccessories() {
            lavaFishing = false;
            tackleBox = false;
            fishingSkill = 0;
            switch (accessory.type) {
                case ItemID.TackleBox:
                    tackleBox = true;
                    break;
                case ItemID.AnglerEarring:
                    fishingSkill += 10;
                    break;
                case ItemID.AnglerTackleBag:
                    tackleBox = true;
                    fishingSkill += 10;
                    break;
                case ItemID.LavaFishingHook:
                    lavaFishing = true;
                    break;
                case ItemID.LavaproofTackleBag:
                    tackleBox = true;
                    fishingSkill += 10;
                    lavaFishing = true;
                    break;
            }
        }

        public void FishingCheck() {
            var player = GetClosestPlayer(Position);

            FishingAttempt fisher = default(FishingAttempt);
            fisher.X = locatePoint.X;
            fisher.Y = locatePoint.Y;
            fisher.bobberType = fishingPole.shoot;
            GetFishingPondState(fisher.X, fisher.Y, out fisher.inLava, out fisher.inHoney, out fisher.waterTilesCount, out fisher.chumsInWater);
            if (fisher.waterTilesCount < 75) {
                SetFishingTip(Language.GetTextValue("GameUI.NotEnoughWater"));
                return;
            }

            fisher.playerFishingConditions = GetFishingConditions();
            if (fisher.playerFishingConditions.BaitItemType == ItemID.TruffleWorm) {
                SetFishingTip(Language.GetTextValue("GameUI.FishingWarning"));
                if (Main.rand.NextBool(5) && (fisher.X < 380 || fisher.X > Main.maxTilesX - 380) && fisher.waterTilesCount > 1000 && player.active && !player.dead && player.Distance(new(fisher.X * 16, fisher.Y * 16)) <= 2000 && NPC.CountNPCS(NPCID.DukeFishron) < 3) {
                    // 召唤猪鲨 （？？？   上限是3个
                    int npc = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), fisher.X * 16, fisher.Y * 16, NPCID.DukeFishron, 1);
                    if (npc == 200)
                        return;

                    Main.npc[npc].alpha = 255;
                    Main.npc[npc].target = player.whoAmI;
                    Main.npc[npc].timeLeft *= 20;
                    string typeName = Main.npc[npc].TypeName;
                    if (Main.netMode == NetmodeID.Server && npc < 200)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc);

                    if (Main.netMode == NetmodeID.SinglePlayer) {
                        Main.NewText(MyUtils.GetText("Autofisher.CarefulNextTime"), 175, 75);
                        Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName), 175, 75);
                    }
                    else if (Main.netMode == NetmodeID.Server) {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.ImproveGame.Autofisher.CarefulNextTime"), new(175, 75, 255));
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[npc].GetTypeNetName()), new Color(175, 75, 255));
                    }

                    bait.stack--;
                    if (bait.stack <= 0)
                        bait = new();

                    UISystem.Instance.AutofisherGUI.RefreshItems(16);
                }
                return;
            }

            fisher.fishingLevel = fisher.playerFishingConditions.FinalFishingLevel;
            if (fisher.fishingLevel == 0)
                return;

            fisher.CanFishInLava = ItemID.Sets.CanFishInLava[fisher.playerFishingConditions.PoleItemType] || ItemID.Sets.IsLavaBait[fisher.playerFishingConditions.BaitItemType] || lavaFishing;
            if (fisher.chumsInWater > 0)
                fisher.fishingLevel += 11;

            if (fisher.chumsInWater > 1)
                fisher.fishingLevel += 6;

            if (fisher.chumsInWater > 2)
                fisher.fishingLevel += 3;

            SetFishingTip(Language.GetTextValue("GameUI.FishingPower", fisher.fishingLevel));
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
            if (fisher.waterTilesCount < fisher.waterNeededToFish)
                SetFishingTip(Language.GetTextValue("GameUI.FullFishingPower", fisher.fishingLevel, 0.0 - Math.Round(fisher.waterQuality * 100f)));

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

            Main.player[250].ZoneDungeon = (IsDungeonBrick(tileType1) || IsDungeonBrick(tileType2)) && NPC.downedBoss3;
            Main.player[250].ZoneBeach = (fisher.X < 380 || fisher.X > Main.maxTilesX - 380) && fisher.waterTilesCount > 1000; // 注意: 原版代码里渔获是根据传入的position判定海边，这里是为了开启海洋匣
            Main.player[250].ZoneCorrupt = TileID.Sets.Corrupt[tileType1] || TileID.Sets.Corrupt[tileType2];
            Main.player[250].ZoneCrimson = TileID.Sets.Crimson[tileType1] || TileID.Sets.Crimson[tileType2];
            Main.player[250].ZoneHallow = TileID.Sets.Hallow[tileType1] || TileID.Sets.Hallow[tileType2];
            Main.player[250].ZoneJungle = IsJungleTile(tileType1) || IsJungleTile(tileType2);
            Main.player[250].ZoneSnow = TileID.Sets.Snow[tileType1] || TileID.Sets.Snow[tileType2];
            Main.player[250].ZoneDesert = TileID.Sets.isDesertBiomeSand[tileType1] || TileID.Sets.isDesertBiomeSand[tileType2];
            
            // 反射调用 FishingCheck_RollItemDrop(ref fisher);
            var targetMethod = fakeProj.GetType().GetMethod("FishingCheck_RollItemDrop",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var args = new object[] { fisher };
            targetMethod.Invoke(fakeProj, args);

            fisher = (FishingAttempt)args[0]; // ref之后用这个获取
            if (fisher.rolledItemDrop != 0) {
                GiveItemToStorage(player, fisher.rolledItemDrop);
                //Main.NewText($"[i:{fisher.rolledItemDrop}]");
            }

            Main.player[250].ZoneDungeon = dungeon;
            Main.player[250].ZoneBeach = beach;
            Main.player[250].ZoneCorrupt = corrupt;
            Main.player[250].ZoneCrimson = crimson;
            Main.player[250].ZoneHallow = hallow;
            Main.player[250].ZoneJungle = jungle;
            Main.player[250].ZoneSnow = snow;
            Main.player[250].ZoneDesert = desert;
        }

        private void GiveItemToStorage(Player player, int itemType) {
            Item item = new();
            item.SetDefaults(itemType);
            int finalFishingLevel = player.GetFishingConditions().FinalFishingLevel;
            if (itemType == ItemID.BombFish) {
                int minStack = (finalFishingLevel / 20 + 3) / 2;
                int maxStack = (finalFishingLevel / 10 + 6) / 2;
                if (Main.rand.Next(50) < finalFishingLevel)
                    maxStack++;

                if (Main.rand.Next(100) < finalFishingLevel)
                    maxStack++;

                if (Main.rand.Next(150) < finalFishingLevel)
                    maxStack++;

                if (Main.rand.Next(200) < finalFishingLevel)
                    maxStack++;

                item.stack = Main.rand.Next(minStack, maxStack + 1);
            }

            if (itemType == 3197) {
                int minStack = (finalFishingLevel / 4 + 15) / 2;
                int maxStack = (finalFishingLevel / 2 + 30) / 2;
                if (Main.rand.Next(50) < finalFishingLevel)
                    maxStack += 4;

                if (Main.rand.Next(100) < finalFishingLevel)
                    maxStack += 4;

                if (Main.rand.Next(150) < finalFishingLevel)
                    maxStack += 4;

                if (Main.rand.Next(200) < finalFishingLevel)
                    maxStack += 4;

                item.stack = Main.rand.Next(minStack, maxStack + 1);
            }

            //PlayerLoader.ModifyCaughtFish(player, item);
            ItemLoader.CaughtFishStack(item);
            item.newAndShiny = true;
            int oldStack = item.stack;
            item = MyUtils.ItemStackToInventory(fish, item, false);

            // 必须是消耗了，也就是真的能存
            if (item.stack != oldStack) {
                TryConsumeBait(player);
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
                UISystem.Instance.AutofisherGUI.RefreshItems();
            else
                NetAutofish.ServerSendSyncItem(Position, 18);
        }

        private void TryConsumeBait(Player player) {
            bool canCunsume = false;
            float num2 = 1f + (float)bait.bait / 6f;
            if (num2 < 1f)
                num2 = 1f;

            if (tackleBox)
                num2 += 1f;

            if (Main.rand.NextFloat() * num2 < 1f)
                canCunsume = true;

            if (bait.type == ItemID.TruffleWorm)
                canCunsume = true;

            if (CombinedHooks.CanConsumeBait(player, bait) ?? canCunsume) {
                if (bait.type == ItemID.LadyBug || bait.type == ItemID.GoldLadyBug)
                    NPC.LadyBugKilled(Position.ToWorldCoordinates(), bait.type == ItemID.GoldLadyBug);

                bait.stack--;
                if (bait.stack <= 0)
                    bait.SetDefaults();
            }
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

            numWaters = GetFishingPondSize(x, y, ref lava, ref honey, ref chumCount);
            if (ModIntegrationsSystem.NoLakeSizePenaltyLoaded) // 不用if else是为了判定是否在熔岩/蜂蜜
                numWaters = 10000;

            if (honey)
                numWaters = (int)((double)numWaters * 1.5);
        }

        private bool[,] tileChecked = new bool[checkWidth * 2 + 1, checkHeight * 2 + 1];
        private int GetFishingPondSize(int x, int y, ref bool lava, ref bool honey, ref int chumCount) {
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
                chumCount += Main.instance.ChumBucketProjectileHelper.GetChumsInLocation(new Point(x, y));
                // 递归临近的四个物块
                int left = GetFishingPondSize(x - 1, y, ref lava, ref honey, ref chumCount);
                int right = GetFishingPondSize(x + 1, y, ref lava, ref honey, ref chumCount);
                int up = GetFishingPondSize(x, y - 1, ref lava, ref honey, ref chumCount);
                int bottom = GetFishingPondSize(x, y + 1, ref lava, ref honey, ref chumCount);
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

            if (result.BaitPower == 0 || result.PolePower == 1) // 原版PolePower判断的是0，但我发现这个最小(没鱼竿)其实是1
                return result;

            var player = GetClosestPlayer(Position);
            int num = result.BaitPower + result.PolePower + fishingSkill;
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

        #endregion

        public override void OnNetPlace() {
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public override void LoadData(TagCompound tag) {
            locatePoint = tag.Get<Point16>("locatePoint");
            fishingPole = tag.Get<Item>("fishingPole");
            bait = tag.Get<Item>("bait");
            accessory = tag.Get<Item>("accessory");
            for (int i = 0; i < 15; i++)
                if (tag.TryGet<Item>($"fish{i}", out var savedFish))
                    fish[i] = savedFish;
        }

        public override void SaveData(TagCompound tag) {
            tag["locatePoint"] = locatePoint;
            tag["fishingPole"] = fishingPole;
            tag["bait"] = bait;
            tag["accessory"] = accessory;
            for (int i = 0; i < 15; i++)
                tag[$"fish{i}"] = fish[i];
        }

        public override void NetSend(BinaryWriter writer) {
            writer.Write(locatePoint.X);
            writer.Write(locatePoint.Y);
            ItemIO.Send(fishingPole, writer, true, false);
            ItemIO.Send(bait, writer, true, false);
            ItemIO.Send(accessory, writer, true, false);
            for (int i = 0; i < 15; i++) {
                if (fish[i] is null)
                    ItemIO.Send(new(), writer, true, false);
                else
                    ItemIO.Send(fish[i], writer, true, false);
            }
        }

        public override void NetReceive(BinaryReader reader) {
            locatePoint = new(reader.ReadInt16(), reader.ReadInt16());
            fishingPole = ItemIO.Receive(reader, true, false);
            bait = ItemIO.Receive(reader, true, false);
            accessory = ItemIO.Receive(reader, true, false);
            for (int i = 0; i < 15; i++)
                fish[i] = ItemIO.Receive(reader, true, false).Clone();
        }
    }
}
