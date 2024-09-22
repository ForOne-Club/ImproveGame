using ImproveGame.Common.ModPlayers;
using ImproveGame.Common.ModSystems;
using ImproveGame.Core;
using ImproveGame.Packets.NetAutofisher;
using ImproveGame.UI.Autofisher;
using ImproveGame.UIFramework;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Tiles
{
    public class TEAutofisher : ModTileEntity
    {
        internal Point16 locatePoint = Point16.NegativeOne;
        internal Item fishingPole = new();
        internal Item bait = new();
        internal Item accessory = new();
        internal Item[] fish = new Item[40];
        internal const int checkWidth = 50;
        internal const int checkHeight = 30;

        internal string FishingTip { get; private set; } = "Error";
        internal double FishingTipTimer { get; private set; }

        public bool CatchCrates = true;
        public bool CatchAccessories = true;
        public bool CatchTools = true;
        public bool CatchWhiteRarityCatches = true;
        public bool CatchNormalCatches = true;
        public bool AutoDeposit = true;
        public List<ItemTypeData> ExcludedItems = [];

        public bool IsEmpty => accessory.IsAir && bait.IsAir && fishingPole.IsAir && (fish is null || fish.All(item => item.IsAir));
        
        public bool HasBait => !bait.IsAir;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Autofisher>();
        }

        public void SetFishingTip(Autofisher.TipType tipType, int fishingLevel = 0, float waterQuality = 0f)
        {
            FishingTip = tipType switch
            {
                Autofisher.TipType.FishingWarning => Language.GetTextValue("GameUI.FishingWarning"),
                Autofisher.TipType.NotEnoughWater => Language.GetTextValue("GameUI.NotEnoughWater"),
                Autofisher.TipType.FishingPower => Language.GetTextValue("GameUI.FishingPower", fishingLevel),
                Autofisher.TipType.FullFishingPower => Language.GetTextValue("GameUI.FullFishingPower", fishingLevel, 0.0 - Math.Round(waterQuality * 100f)),
                Autofisher.TipType.Unavailable => GetText("UI.Autofisher.Unavailable"),
                Autofisher.TipType.InShimmer => GetText("UI.Autofisher.InShimmer"),
                _ => ""
            };
            FishingTipTimer = 0;

            if (Main.netMode is not NetmodeID.Server)
            {
                return;
            }

            // 给距离钓鱼机 1000 像素内的玩家发包
            const int distance = 1000 * 1000;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var player = Main.player[i];
                // 距离用 DistanceSQ 判断，没有开方操作运行更快
                if (player.active && !player.DeadOrGhost && player.Center.DistanceSQ(Position.ToWorldCoordinates()) <= distance)
                    FishingTipPacket.Get(ID, tipType, fishingLevel, waterQuality).Send(i);
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            for (int k = 0; k < fish.Length; k++)
            {
                fish[k] = new();
            }
            fishingPole = new();
            bait = new();
            accessory = new();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 2, 2);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i - 1, j - 1, Type);
                return -1;
            }

            int placedEntity = Place(i - 1, j - 1);
            return placedEntity;
        }

        public static Player GetClosestPlayer(Point16 Position) => Main.player[Player.FindClosest(new Vector2(Position.X * 16, Position.Y * 16), 1, 1)];

        #region 钓鱼

        public int FishingTimer;
        public int AutoDepositTimer;

        public override void Update()
        {
            for (int i = 0; i < fish.Length; i++)
            {
                fish[i] ??= new();
            }

            FishingTipTimer += 1.0 / 60.0;
            if (Main.netMode != NetmodeID.Server && Main.netMode != NetmodeID.SinglePlayer)
                return;
            if (locatePoint.X < 0 || locatePoint.Y < 0)
                return;
            if (Framing.GetTileSafely(locatePoint).LiquidAmount == 0)
            {
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

            bool accAvailable = ModIntegrationsSystem.FishingStatLookup.TryGetValue(accessory.type, out FishingStat stat);

            float fishingSpeedBonus = accAvailable ? stat.SpeedMultiplier : 1f;

            // 钓鱼机内每条 Bass 将提供 5% 的钓鱼速度加成，最高可达 500% 加成
            int bassCount = 0;
            for (int i = 0; i < fish.Length; i++)
            {
                if (fish[i].type == ItemID.Bass)
                {
                    bassCount += fish[i].stack;
                }
            }
            fishingSpeedBonus += Math.Min(bassCount * 0.05f, 5f);
            
            // 肉后提升200%钓鱼速度
            if (Main.hardMode)
                fishingSpeedBonus += 2f;

            // 钓鱼机基础冷却在这里改，原版钓鱼速度是660
            // 2200 = 660 ÷ 3/10
            const float fishingCooldown = 2200;
            if (FishingTimer > fishingCooldown / fishingSpeedBonus)
            {
                FishingTimer = 0;
                _lavaFishing = false;
                _tackleBox = false;
                _fishingSkill = 0;
                if (accAvailable)
                    ApplyAccessories(stat);
                FishingCheck();
            }

            AutoDepositTimer++;
            if (AutoDepositTimer > 3600 && AutoDeposit)
            {
                AutoDepositTimer = 0;
                AutoDepositManipulation();
            }
        }

        private bool _lavaFishing;
        private bool _tackleBox;
        private int _fishingSkill;

        private void ApplyAccessories(FishingStat stat)
        {
            _lavaFishing = stat.LavaFishing;
            _tackleBox = stat.TackleBox;
            _fishingSkill += stat.Power;
        }

        public void FishingCheck()
        {
            var player = GetClosestPlayer(Position);

            FishingAttempt fisher = default;
            fisher.X = locatePoint.X;
            fisher.Y = locatePoint.Y;
            fisher.bobberType = fishingPole.shoot;
            GetFishingPondState(fisher.X, fisher.Y, out fisher.inLava, out fisher.inHoney, out bool inShimmer, out fisher.waterTilesCount, out fisher.chumsInWater);
            if (fisher.waterTilesCount < 75)
            {
                SetFishingTip(Autofisher.TipType.NotEnoughWater);
                return;
            }
            if (inShimmer)
            {
                SetFishingTip(Autofisher.TipType.InShimmer);
                return;
            }

            fisher.playerFishingConditions = GetFishingConditions();
            if (fisher.playerFishingConditions.BaitItemType == ItemID.TruffleWorm)
            {
                SetFishingTip(Autofisher.TipType.FishingWarning);
                if (Main.rand.NextBool(5) && (fisher.X < 380 || fisher.X > Main.maxTilesX - 380) && fisher.waterTilesCount > 1000 && player.active && !player.dead && player.Distance(new(fisher.X * 16, fisher.Y * 16)) <= 2000 && NPC.CountNPCS(NPCID.DukeFishron) < 3)
                {
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

                    switch (Main.netMode)
                    {
                        case NetmodeID.SinglePlayer:
                            Main.NewText(GetText("UI.Autofisher.CarefulNextTime"), 175, 75);
                            Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName), 175, 75);
                            break;
                        case NetmodeID.Server:
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.ImproveGame.Autofisher.CarefulNextTime"), new(175, 75, 255));
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[npc].GetTypeNetName()), new Color(175, 75, 255));
                            break;
                    }

                    bait.stack--;
                    if (bait.stack <= 0)
                        bait = new();

                    UISystem.Instance.AutofisherGUI.RefreshItems(ItemSyncPacket.Bait);
                }
                return;
            }

            fisher.fishingLevel = fisher.playerFishingConditions.FinalFishingLevel;
            if (fisher.fishingLevel == 0)
                return;

            fisher.CanFishInLava = ItemID.Sets.CanFishInLava[fisher.playerFishingConditions.PoleItemType] || ItemID.Sets.IsLavaBait[fisher.playerFishingConditions.BaitItemType] || _lavaFishing;
            if (fisher.chumsInWater > 0)
                fisher.fishingLevel += 11;

            if (fisher.chumsInWater > 1)
                fisher.fishingLevel += 6;

            if (fisher.chumsInWater > 2)
                fisher.fishingLevel += 3;

            SetFishingTip(Autofisher.TipType.FishingPower, fisher.fishingLevel);
            fisher.waterNeededToFish = 300;
            float num = Main.maxTilesX / 4200;
            num *= num;
            fisher.atmo = (float)((Position.Y - (60f + 10f * num)) / (Main.worldSurface / 6.0));
            if (fisher.atmo < 0.25)
                fisher.atmo = 0.25f;

            if (fisher.atmo > 1f)
                fisher.atmo = 1f;

            fisher.waterNeededToFish = (int)(fisher.waterNeededToFish * fisher.atmo);
            fisher.waterQuality = fisher.waterTilesCount / (float)fisher.waterNeededToFish;
            if (fisher.waterQuality < 1f)
                fisher.fishingLevel = (int)(fisher.fishingLevel * fisher.waterQuality);

            fisher.waterQuality = 1f - fisher.waterQuality;
            if (fisher.waterTilesCount < fisher.waterNeededToFish)
                SetFishingTip(Autofisher.TipType.FullFishingPower, fisher.fishingLevel, fisher.waterQuality);

            if (player.active && !player.dead)
            {
                if (player.luck < 0f)
                {
                    if (Main.rand.NextFloat() < 0f - player.luck)
                        fisher.fishingLevel = (int)(fisher.fishingLevel * (0.9 - Main.rand.NextFloat() * 0.3));
                }
                else if (Main.rand.NextFloat() < player.luck)
                {
                    fisher.fishingLevel = (int)(fisher.fishingLevel * (1.1 + Main.rand.NextFloat() * 0.3));
                }
            }

            int fishChance = (fisher.fishingLevel + 75) / 2;
            if (Main.rand.Next(100) > fishChance)
                return;

            fisher.heightLevel = 0;
            if (Main.remixWorld)
            {
                if (fisher.Y < Main.worldSurface * 0.5)
                {
                    fisher.heightLevel = 0;
                }
                else if (fisher.Y < Main.worldSurface)
                {
                    fisher.heightLevel = 1;
                }
                else if (fisher.Y < Main.rockLayer)
                {
                    fisher.heightLevel = 3;
                }
                else if (fisher.Y < Main.maxTilesY - 300)
                {
                    fisher.heightLevel = 2;
                }
                else
                {
                    fisher.heightLevel = 4;
                }
                if (fisher.heightLevel == 2 && Main.rand.NextBool(2))
                {
                    fisher.heightLevel = 1;
                }
            }
            else if (fisher.Y < Main.worldSurface * 0.5)
            {
                fisher.heightLevel = 0;
            }
            else if (fisher.Y < Main.worldSurface)
            {
                fisher.heightLevel = 1;
            }
            else if (fisher.Y < Main.rockLayer)
            {
                fisher.heightLevel = 2;
            }
            else if (fisher.Y < Main.maxTilesY - 300)
            {
                fisher.heightLevel = 3;
            }
            else
            {
                fisher.heightLevel = 4;
            }

            FishingCheck_RollDropLevels(player, fisher.fishingLevel, out fisher.common, out fisher.uncommon, out fisher.rare, out fisher.veryrare, out fisher.legendary, out fisher.crate);
            //FishingCheck_ProbeForQuestFish(ref fisher);
            //FishingCheck_RollEnemySpawns(ref fisher);

            // 伪装一个proj，用反射调用Projectile.FishingCheck_RollItemDrop
            var fakeProj = new Projectile
            {
                owner = 255
            };

            Main.player[255].Center = Position.ToWorldCoordinates();
            TileCounter tileCounter = new();
            tileCounter.ScanAndExportToMain(Position);
            tileCounter.Simulate(Main.player[255]);
            tileCounter.FargosFountainSupport(Main.player[255]);

            // AssemblyPublicizer 使得 FishingCheck_RollItemDrop 可以直接访问
            fakeProj.FishingCheck_RollItemDrop(ref fisher);

            AdvancedPopupRequest sonar = new();
            Vector2 sonarPosition = new(-1145141f, -919810f); // 直接fake到世界外面
            PlayerLoader.CatchFish(Main.player[255], fisher, ref fisher.rolledItemDrop, ref fisher.rolledEnemySpawn, ref sonar, ref sonarPosition);

            if (fisher.rolledItemDrop != 0)
            {
                GiveItemToStorage(player, fisher.rolledItemDrop);
                //Main.NewText($"[i:{fisher.rolledItemDrop}]");
            }

            // 单人模式里这还作为视效的判定，因此得强制更新
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.LocalPlayer.ForceUpdateBiomes();
        }

        private void GiveItemToStorage(Player player, int itemType)
        {
            // 怎么可能？
            if (!ContentSamples.ItemsByType.ContainsKey(itemType))
                return;

            // 物品筛选没过
            if (ExcludedItems.Any(i => i.Item.type == itemType))
                return;

            CatchRecord.AddCatch(itemType);
            Item item = new(itemType);

            int fishType = 0; // 0 普通鱼 (稀有度大于白)
            if (ItemLoader.CanRightClick(ContentSamples.ItemsByType[itemType]) &&
                (Main.ItemDropsDB.GetRulesForItemID(itemType).Any()))
                fishType = 1; // 1 宝匣
            else if (item.accessory) fishType = 2; // 2 饰品
            else if (item.damage > 0) fishType = 3; // 3 工具武器
            else if (item.OriginalRarity <= ItemRarityID.White) fishType = 4; // 4 白色稀有度

            switch (fishType)
            {
                case 1:
                    if (!CatchCrates) return;
                    break;
                case 2:
                    if (!CatchAccessories) return;
                    break;
                case 3:
                    if (!CatchTools) return;
                    break;
                case 4:
                    if (!CatchWhiteRarityCatches) return;
                    break;
                default:
                    if (!CatchNormalCatches) return;
                    break;
            }

            int finalFishingLevel = player.GetFishingConditions().FinalFishingLevel;

            if (itemType == ItemID.BombFish)
            {
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

            if (itemType == ItemID.FrostDaggerfish)
            {
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

            PlayerLoader.ModifyCaughtFish(player, item);
            ItemLoader.CaughtFishStack(item);
            item.newAndShiny = true;
            var dummyItem = item.Clone();
            int oldStack = item.stack;

            // 奇怪的文本，作为彩蛋
            if (HasDevMark && Main.rand.NextBool(10) && Language.ActiveCulture.Name is "zh-Hans")
            {
                var pos = Position.ToWorldCoordinates(16, 16).ToPoint();
                var rect = new Rectangle(pos.X, pos.Y, 16, 16);
                CombatText.NewText(rect, Color.Pink, "要装不下了...");
            }

            // 先填充和物品相同的
            for (int i = 0; i < fish.Length; i++)
            {
                int oldStackSlot = fish[i].stack;
                item = ItemStackToInventoryItem(fish, i, item, false);
                if (fish[i].stack != oldStackSlot && Main.netMode is NetmodeID.Server)
                {
                    // 这包是给开着钓鱼机的玩家用的，只给开着的发包就行了
                    for (int p = 0; p < Main.maxPlayers; p++)
                    {
                        var client = Main.player[p];
                        if (client.active && !client.DeadOrGhost && client.GetModPlayer<AutofishPlayer>().IsAutofisherOpened)
                            ItemsStackChangePacket.Get(ID, (byte)i, fish[i].stack - oldStackSlot).Send(p);
                    }
                }
                if (item.IsAir)
                    goto FilledEnd;
            }
            // 后填充空位
            for (int i = 0; i < fish.Length; i++)
            {
                if (fish[i].IsAir)
                {
                    fish[i] = item.Clone();
                    if (Main.netMode is NetmodeID.Server)
                    {
                        ItemSyncPacket.Get(ID, (byte)i).Send(runLocally: false);
                    }
                    item = new();
                    goto FilledEnd;
                }
            }

            FilledEnd:;

            // 必须是消耗了，也就是真的能存 | TryConsumeBait返回true表示鱼饵消耗了
            if (item.stack != oldStack)
            {
                // 用dummyItem，因为填充后item可能是Air
                Chest.VisualizeChestTransfer(locatePoint.ToWorldCoordinates(), Position.ToWorldCoordinates(16, 16), dummyItem, dummyItem.stack);

                if (TryConsumeBait(player) && Main.netMode is NetmodeID.Server)
                {
                    // 没了
                    if (bait.IsAir)
                        ItemSyncPacket.Get(ID, ItemSyncPacket.Bait).Send(runLocally: false);
                    else // 还在，同步stack
                        ItemsStackChangePacket.Get(ID, ItemSyncPacket.Bait, -1).Send(runLocally: false);
                }
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                UISystem.Instance.AutofisherGUI?.RefreshItems();
            }
        }

        private bool TryConsumeBait(Player player)
        {
            bool canConsume = false;
            float chanceDenominator = 1f + bait.bait / 6f;
            if (chanceDenominator < 1f)
                chanceDenominator = 1f;

            if (_tackleBox)
                chanceDenominator += 1f;

            // 诱饵消耗概率仅为手动钓鱼的 40%
            chanceDenominator *= 1f / 0.4f;

            if (Main.rand.NextFloat() * chanceDenominator < 1f)
                canConsume = true;

            if (bait.type == ItemID.TruffleWorm)
                canConsume = true;

            if (CombinedHooks.CanConsumeBait(player, bait) ?? canConsume)
            {
                if (bait.type == ItemID.LadyBug || bait.type == ItemID.GoldLadyBug)
                    NPC.LadyBugKilled(Position.ToWorldCoordinates(), bait.type == ItemID.GoldLadyBug);

                bait.stack--;
                if (bait.stack <= 0)
                {
                    bait.SetDefaults();
                    if (Config.EmptyAutofisher)
                    {
                        var center = new Point(Position.X + 1, Position.Y + 2);
                        GetMeterCoords(center, out string compassText, out string depthText);

                        string finalText = GetTextWith("Configs.ImproveConfigs.EmptyAutofisher.Tip", new
                        {
                            Compass = compassText,
                            Depth = depthText
                        });
                        WorldGen.BroadcastText(NetworkText.FromLiteral(finalText), Color.OrangeRed);
                    }
                }
                return true;
            }
            return false;
        }

        private static void FishingCheck_RollDropLevels(Player closetPlayer, int fishingLevel, out bool common, out bool uncommon, out bool rare, out bool veryrare, out bool legendary, out bool crate)
        {
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

        private void GetFishingPondState(int x, int y, out bool lava, out bool honey, out bool shimmer, out int numWaters, out int chumCount)
        {
            chumCount = 0;
            lava = false;
            honey = false;
            shimmer = false;
            for (int i = 0; i < tileChecked.GetLength(0); i++)
            {
                for (int j = 0; j < tileChecked.GetLength(1); j++)
                {
                    tileChecked[i, j] = false;
                }
            }

            numWaters = GetFishingPondSize(x, y, ref lava, ref honey, ref shimmer, ref chumCount);
            if (ModIntegrationsSystem.NoLakeSizePenaltyLoaded || Config.NoLakeSizePenalty) // 不用if else是为了判定是否在熔岩/蜂蜜
                numWaters = 10000;

            if (honey)
                numWaters = (int)(numWaters * 1.5);
        }

        private bool[,] tileChecked = new bool[checkWidth * 2 + 1, checkHeight * 2 + 1];

        public TEAutofisher()
        {
            _tackleBox = false;
            _fishingSkill = 0;
        }

        private int GetFishingPondSize(int x, int y, ref bool lava, ref bool honey, ref bool shimmer, ref int chumCount)
        {
            Point16 arrayLeftTop = new(Position.X + 1 - checkWidth, Position.Y + 1 - checkHeight);
            if (x - arrayLeftTop.X < 0 || x - arrayLeftTop.X > checkWidth * 2 || y - arrayLeftTop.Y < 0 || y - arrayLeftTop.Y > checkHeight * 2)
                return 0;
            if (tileChecked[x - arrayLeftTop.X, y - arrayLeftTop.Y])
                return 0;

            tileChecked[x - arrayLeftTop.X, y - arrayLeftTop.Y] = true;
            var tile = Framing.GetTileSafely(x, y);
            if (tile.LiquidAmount > 0 && !WorldGen.SolidTile(x, y))
            {
                if (tile.LiquidType == LiquidID.Lava)
                    lava = true;
                if (tile.LiquidType == LiquidID.Honey)
                    honey = true;
                if (tile.LiquidType == LiquidID.Shimmer)
                    shimmer = true;
                chumCount += Main.instance.ChumBucketProjectileHelper.GetChumsInLocation(new Point(x, y));
                // 递归临近的四个物块
                int left = GetFishingPondSize(x - 1, y, ref lava, ref honey, ref shimmer, ref chumCount);
                int right = GetFishingPondSize(x + 1, y, ref lava, ref honey, ref shimmer, ref chumCount);
                int up = GetFishingPondSize(x, y - 1, ref lava, ref honey, ref shimmer, ref chumCount);
                int bottom = GetFishingPondSize(x, y + 1, ref lava, ref honey, ref shimmer, ref chumCount);
                return left + right + up + bottom + 1;
            }
            return 0;
        }

        public PlayerFishingConditions GetFishingConditions()
        {
            PlayerFishingConditions result = default;
            result.Pole = fishingPole;
            result.Bait = bait;
            if (result.BaitItemType == ItemID.TruffleWorm)
                return result;

            if (result.BaitPower == 0 || result.PolePower < 5)
                return result;

            var player = GetClosestPlayer(Position);
            int num = result.BaitPower + result.PolePower + _fishingSkill;
            result.LevelMultipliers = Fishing_GetPowerMultiplier(result.Pole, result.Bait, player);
            result.FinalFishingLevel = (int)(num * result.LevelMultipliers);
            return result;
        }

        private float Fishing_GetPowerMultiplier(Item pole, Item bait, Player player)
        {
            float num = 1f;
            if (Main.raining)
                num *= 1.2f;

            if (Main.cloudBGAlpha > 0f)
                num *= 1.1f;

            switch (Main.dayTime)
            {
                case true when Main.time is < 5400.0 or > 48600.0: // 早上
                    num *= 1.3f;
                    break;
                case true when Main.time is > 16200.0 and < 37800.0: // 早上
                case false when Main.time is > 6480.0 and < 25920.0: // 晚上
                    num *= 0.8f;
                    break;
            }

            switch (Main.moonPhase)
            {
                case 0:
                    num *= 1.1f;
                    break;
                case 1:
                case 7:
                    num *= 1.05f;
                    break;
                case 3:
                case 5:
                    num *= 0.95f;
                    break;
                case 4:
                    num *= 0.9f;
                    break;
            }

            if (Main.bloodMoon)
                num *= 1.1f;

            PlayerLoader.GetFishingLevel(player, pole, bait, ref num);
            return num;
        }

        #endregion

        private void AutoDepositManipulation()
        {
            // 寻找离中心点最近的箱子
            var chests = new List<(int, float)>(8);
            Point16 center = Position + new Point16(1, 1);
            for (int i = 0; i < Main.maxChests; i++)
            {
                if (Main.chest[i] == null || Main.chest[i].x < 0 || Main.chest[i].y < 0 || Chest.IsLocked(Main.chest[i].x, Main.chest[i].y))
                    continue;

                Point16 chestPosition = new(Main.chest[i].x, Main.chest[i].y);
                float distance = center.DistanceSQ(chestPosition);
                if (distance < 30 * 30)
                    chests.Add((i, distance));
            }

            // 没找到箱子
            if (chests.Count is 0)
                return;

            // 按照距离排序
            chests.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            
            // 逐个箱子尝试存放
            foreach ((int index, float distance) in chests)
            {
                // 正在使用
                if (Chest.IsPlayerInChest(index) && Main.netMode is NetmodeID.Server)
                    continue;

                // 存入
                bool fullyDeposited = true;
                Chest chest = Main.chest[index];
                for (int i = 0; i < fish.Length; i++)
                {
                    if (fish[i] is null || fish[i].IsAir)
                        continue;

                    ref var item = ref fish[i];
                    if (item.favorited)
                        continue;

                    var dummyItem = item.Clone();
                    int oldStack = item.stack;
                
                    item = ItemStackToInventory(chest.item, fish[i], false);

                    // 必须是消耗了，也就是真的能存 | TryConsumeBait返回true表示鱼饵消耗了
                    if (item.stack != oldStack)
                    {
                        // 没了
                        if (item.IsAir)
                            ItemSyncPacket.Get(ID, (byte)i).Send(runLocally: false);
                        else // 还在，同步stack
                            ItemsStackChangePacket.Get(ID, (byte)i, -1).Send(runLocally: false);
                    
                        // 用dummyItem，因为填充后item可能是Air
                        Chest.VisualizeChestTransfer(Position.ToWorldCoordinates(16, 16), new Vector2(chest.x * 16 + 16, chest.y * 16 + 16), dummyItem, dummyItem.stack);
                    }

                    if (Main.netMode is NetmodeID.SinglePlayer)
                        UISystem.Instance.AutofisherGUI?.RefreshItems();

                    if (!item.IsAir)
                        fullyDeposited = false;
                }
                
                if (fullyDeposited)
                    break;
            }
        }

        public override void OnKill()
        {
            if (!fishingPole.IsAir)
                SpawnDropItem(ref fishingPole);
            if (!bait.IsAir)
                SpawnDropItem(ref bait);
            if (!accessory.IsAir)
                SpawnDropItem(ref accessory);
            for (int k = 0; k < fish.Length; k++)
                if (!fish[k].IsAir)
                    SpawnDropItem(ref fish[k]);
        }

        private void SpawnDropItem(ref Item item)
        {
            var position = Position.ToWorldCoordinates();
            int i = Item.NewItem(new EntitySource_Misc("FishingMachine"), (int)position.X, (int)position.Y, 32, 32, item.type);
            item.position = Main.item[i].position;
            Main.item[i] = item;
            var drop = Main.item[i];
            item = new Item();
            drop.velocity.Y = -2f;
            drop.velocity.X = Main.rand.NextFloat(-4f, 4f);
            drop.favorited = false;
            drop.newAndShiny = false;
        }

        // 返回的是物品禁用状态，true就是没禁用，false就是禁用了
        public bool ToggleItem(Item item)
        {
            bool alreadyExcluded = ExcludedItems.Any(i => ItemExtensions.IsSameItem(i.Item, item));
            if (alreadyExcluded)
            {
                ExcludedItems.RemoveAll(i => ItemExtensions.IsSameItem(i.Item, item));
                MachineExcludedItemSyncer.Sync(ID, ExcludedItems);
                return true;
            }

            ExcludedItems.Add(new ItemTypeData(item));
            MachineExcludedItemSyncer.Sync(ID, ExcludedItems);
            return false;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            locatePoint = tag.Get<Point16>("locatePoint");
            fishingPole = tag.Get<Item>("fishingPole");
            bait = tag.Get<Item>("bait");
            accessory = tag.Get<Item>("accessory");

            if (tag.ContainsKey("fishes"))
                fish = tag.Get<Item[]>("fishes");
            Array.Resize(ref fish, 40); // 旧版兼容
            for (int i = 0; i < fish.Length; i++)
                if (tag.ContainsKey($"fish{i}"))
                    fish[i] = tag.Get<Item>($"fish{i}");

            if (!tag.TryGet("CatchCrates", out CatchCrates))
                CatchCrates = true;
            if (!tag.TryGet("CatchAccessories", out CatchAccessories))
                CatchAccessories = true;
            if (!tag.TryGet("CatchTools", out CatchTools))
                CatchTools = true;
            if (!tag.TryGet("CatchWhiteRarityCatches", out CatchWhiteRarityCatches))
                CatchWhiteRarityCatches = true;
            if (!tag.TryGet("CatchNormalCatches", out CatchNormalCatches))
                CatchNormalCatches = true;
            if (!tag.TryGet("autoDeposit", out AutoDeposit))
                AutoDeposit = false;

            if (tag.TryGet("flags", out byte flags))
            {
                var bitsByte = (BitsByte) flags;
                CatchCrates = bitsByte[0];
                CatchAccessories = bitsByte[1];
                CatchTools = bitsByte[2];
                CatchWhiteRarityCatches = bitsByte[3];
                CatchNormalCatches = bitsByte[4];
                AutoDeposit = bitsByte[5];
            }

            ExcludedItems = tag.Get<List<ItemTypeData>>("excludedItemData") ?? [];
            // var excludedItemData = tag.Get<List<CatchData>>("excludedItemData") ?? [];
            // ExcludedItems = excludedItemData.Select(data => data.Item.type).ToHashSet();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["locatePoint"] = locatePoint;
            tag["fishingPole"] = fishingPole;
            tag["bait"] = bait;
            tag["accessory"] = accessory;
            tag["fishes"] = fish;
            // tag["CatchCrates"] = CatchCrates;
            // tag["CatchAccessories"] = CatchAccessories;
            // tag["CatchTools"] = CatchTools;
            // tag["CatchWhiteRarityCatches"] = CatchWhiteRarityCatches;
            // tag["CatchNormalCatches"] = CatchNormalCatches;
            // tag["autoDeposit"] = AutoDeposit;
            var flags = new BitsByte(CatchCrates, CatchAccessories, CatchTools, CatchWhiteRarityCatches,
                CatchNormalCatches, AutoDeposit);
            tag["flags"] = (byte) flags;

            tag["excludedItemData"] = ExcludedItems;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(locatePoint.X);
            writer.Write(locatePoint.Y);
            ItemIO.Send(fishingPole, writer, true);
            ItemIO.Send(bait, writer, true);
            ItemIO.Send(accessory, writer, true);
            writer.Write(fish);
            writer.Write(ExcludedItems);

            var flags = new BitsByte(
                CatchCrates,
                CatchAccessories,
                CatchTools,
                CatchWhiteRarityCatches,
                CatchNormalCatches,
                AutoDeposit
            );
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            locatePoint = new(reader.ReadInt16(), reader.ReadInt16());
            fishingPole = ItemIO.Receive(reader, true);
            bait = ItemIO.Receive(reader, true);
            accessory = ItemIO.Receive(reader, true);
            fish = reader.ReadItemArray();
            ExcludedItems = reader.ReadListItemTypeData();

            var flags = (BitsByte)reader.ReadByte();
            CatchCrates = flags[0];
            CatchAccessories = flags[1];
            CatchTools = flags[2];
            CatchWhiteRarityCatches = flags[3];
            CatchNormalCatches = flags[4];
            AutoDeposit = flags[5];
        }
    }
}
