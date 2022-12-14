using ImproveGame.Common.Packets;
using ImproveGame.Common.Systems;
using ImproveGame.Content.Items;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.GameContent.UI;
using Terraria.GameInput;

namespace ImproveGame.Common.Players
{
    public class ImprovePlayer : ModPlayer
    {
        public Vector2 MouseWorld { get; set; }

        public bool PiggyBank;
        public bool Safe;
        public bool DefendersForge;
        public BannerChest bannerChest;
        public PotionBag potionBag;

        public bool ShouldUpdateTeam;

        public override void OnEnterWorld(Player player)
        {
            if (Config.TeamAutoJoin && Main.netMode is NetmodeID.MultiplayerClient)
            {
                ShouldUpdateTeam = true;
            }
        }

        public override void PostUpdate()
        {
            if (Main.gameMenu)
                return;

            if (Main.myPlayer == Player.whoAmI && Main.MouseWorld != MouseWorld)
                MouseWorldPacket.Get((byte)Player.whoAmI, Main.MouseWorld).Send(runLocally: true);

            if (Config.JourneyResearch)
            {
                foreach (var item in from i in Player.inventory where !i.IsAir select i)
                {
                    // 旅行自动研究
                    if (Main.netMode is not NetmodeID.Server && item.favorited &&
                        Main.LocalPlayer.difficulty is PlayerDifficultyID.Creative &&
                        CreativeItemSacrificesCatalog.Instance.TryGetSacrificeCountCapToUnlockInfiniteItems(item.type,
                            out int amountNeeded))
                    {
                        int sacrificeCount =
                            Main.LocalPlayerCreativeTracker.ItemSacrifices.GetSacrificeCount(item.type);
                        if (amountNeeded - sacrificeCount > 0 && item.stack >= amountNeeded - sacrificeCount)
                        {
                            CreativeUI.SacrificeItem(item.Clone(), out _);
                            SoundEngine.PlaySound(SoundID.Research);
                            SoundEngine.PlaySound(SoundID.ResearchComplete);
                        }
                    }
                }
            }
        }

        public override void ResetEffects()
        {
            if (Config.SuperVoidVault)
            {
                PiggyBank = Player.HasItem(ItemID.PiggyBank);
                Safe = Player.HasItem(ItemID.Safe);
                DefendersForge = Player.HasItem(ItemID.DefendersForge);
            }

            bannerChest = null;
            potionBag = null;

            // 玩家背包
            foreach (var item in from i in Player.inventory where !i.IsAir select i)
            {
                if (Config.LoadModItems.BannerChest)
                {
                    if (bannerChest is null && item.ModItem is BannerChest chest)
                    {
                        bannerChest = chest;
                    }
                }
                
                if (Config.LoadModItems.PotionBag)
                {
                    if (potionBag is null && item.ModItem is PotionBag bag)
                    {
                        potionBag = bag;
                    }
                }
            }

            // 大背包
            Item[] SuperVault = Player.GetModPlayer<DataPlayer>().SuperVault;
            foreach (var item in from i in SuperVault where !i.IsAir select i)
            {
                if (Config.LoadModItems.BannerChest)
                {
                    if (bannerChest is null && item.ModItem is BannerChest chest)
                    {
                        bannerChest = chest;
                    }
                }
                
                if (Config.LoadModItems.PotionBag)
                {
                    if (potionBag is null && item.ModItem is PotionBag bag)
                    {
                        potionBag = bag;
                    }
                }
            }

            if (Player.whoAmI == Main.myPlayer)
            {
                if (ShouldUpdateTeam)
                {
                    Player.team = 1;
                    NetMessage.SendData(MessageID.PlayerTeam, -1, -1, null, Player.whoAmI);
                    ShouldUpdateTeam = false;
                }
            }

            if (Config.NoCD_FishermanQuest)
            {
                if (Main.anglerQuestFinished || Main.anglerWhoFinishedToday.Contains(Name))
                {
                    Main.anglerQuestFinished = false;
                    Main.anglerWhoFinishedToday.Clear();
                    Main.NewText(Language.GetTextValue($"Mods.ImproveGame.Tips.AnglerQuest"), ItemRarityID.Pink);
                }
            }

            if (Player.whoAmI == Main.myPlayer && !Player.HeldItem.IsAir && Config.ImproveTileSpeedAndTileRange)
            {
                string internalName = ItemID.Search.GetName(Player.HeldItem.type).ToLower(); // 《英文名》因为没法在非英语语言获取英文名，只能用内部名了
                string currentLanguageName = Lang.GetItemNameValue(Player.HeldItem.type).ToLower();
                if (Config.TileSpeed_Blacklist.Any(str => internalName.Contains(str) || currentLanguageName.Contains(str)))
                {
                    return;
                }
                Player.tileSpeed = 3f;
                Player.wallSpeed = 3f;
                Player.tileRangeX += 5;
                Player.tileRangeY += 4;
            }
        }

        // 重生加速
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            float TimeShortened = Player.respawnTimer * MathHelper.Clamp(Config.ResurrectionTimeShortened, 0, 100) / 100;
            if (TimeShortened > 0)
            {
                int ct = CombatText.NewText(Player.getRect(), new(25, 255, 25), GetTextWith("CombatText.Commonds.ResurrectionTimeShortened", new
                {
                    Name = Player.name,
                    Time = MathF.Round(TimeShortened / 60)
                }));
                if (Main.combatText.IndexInRange(ct))
                {
                    Main.combatText[ct].lifeTime *= 3;
                }
            }
            Player.respawnTimer -= (int)TimeShortened;
        }

        private bool _cacheSwitchSlot;

        /// <summary>
        /// 快捷键
        /// </summary>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.SuperVaultKeybind.JustPressed)
            {
                if (Config.SuperVault)
                {
                    if (BigBagGUI.Visible)
                    {
                        UISystem.Instance.BigBagGUI.Close();
                    }
                    else
                    {
                        UISystem.Instance.BigBagGUI.Open();
                    }
                }
            }
            if (KeybindSystem.BuffTrackerKeybind.JustPressed)
            {
                if (BuffTrackerGUI.Visible)
                    UISystem.Instance.BuffTrackerGUI.Close();
                else
                    UISystem.Instance.BuffTrackerGUI.Open();
            }
            if (KeybindSystem.GrabBagKeybind.JustPressed && Main.HoverItem is not null)
            {
                var item = Main.HoverItem;
                bool hasLoot = Main.ItemDropsDB.GetRulesForItemID(item.type, includeGlobalDrops: true).Count > 0;
                hasLoot &= CollectHelper.ItemCanRightClick[Main.HoverItem.type] || ItemLoader.CanRightClick(Main.HoverItem);
                if (GrabBagInfoGUI.Visible && (GrabBagInfoGUI.ItemID == item.type || item.IsAir || !hasLoot))
                    UISystem.Instance.GrabBagInfoGUI.Close();
                else if (hasLoot)
                    UISystem.Instance.GrabBagInfoGUI.Open(Main.HoverItem.type);
            }
            if (KeybindSystem.HotbarSwitchKeybind.JustPressed || _cacheSwitchSlot)
            {
                if (Main.LocalPlayer.ItemTimeIsZero && Main.LocalPlayer.itemAnimation is 0)
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        (Player.inventory[i], Player.inventory[i + 10]) = (Player.inventory[i + 10], Player.inventory[i]);
                        if (Main.netMode is NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.SyncEquipment, number: Main.myPlayer, number2: i, number3: Main.LocalPlayer.inventory[i].prefix);
                            NetMessage.SendData(MessageID.SyncEquipment, number: Main.myPlayer, number2: i + 10, number3: Main.LocalPlayer.inventory[i].prefix);
                        }
                    }
                    _cacheSwitchSlot = false;
                }
                else
                {
                    _cacheSwitchSlot = true;
                }
            }
        }
    }
}
