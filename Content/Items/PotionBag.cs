using ImproveGame.Common.ModHooks;
using ImproveGame.Common.Players;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public class PotionBag : ModItem, IItemOverrideLeftClick
    {
        [CloneByReference]
        public List<Item> storedPotions = new();

        public override bool CanRightClick() => storedPotions is not null && storedPotions.Count != 0;

        public override void RightClick(Player player) {
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), storedPotions[^1], storedPotions[^1].stack);
            storedPotions.RemoveAt(storedPotions.Count - 1);
        }

        public override bool ConsumeItem(Player player) => false;

        /// <summary>
        /// 只有在这些地方才可以放药水进去
        /// </summary>
        private static readonly List<int> availableContexts = new() {
            ItemSlot.Context.InventoryItem,
            ItemSlot.Context.ChestItem,
            114514
        };

        public bool OverrideLeftClick(Item[] inventory, int context, int slot) {
            // 很多的条件
            if (ItemSlot.ShiftInUse || ItemSlot.ControlInUse || !availableContexts.Contains(context) ||
                Main.mouseItem.IsAir || !Main.mouseItem.consumable || Main.mouseItem.buffType <= 0) {
                return false;
            }
            for (int i = 0; i < storedPotions.Count; i++) {
                if (storedPotions[i].IsAir) {
                    storedPotions.RemoveAt(i);
                    i--;
                    continue;
                }
                if (storedPotions[i].type == Main.mouseItem.type && storedPotions[i].stack < storedPotions[i].maxStack && ItemLoader.CanStack(storedPotions[i], Main.mouseItem)) {
                    int stackAvailable = storedPotions[i].maxStack - storedPotions[i].stack;
                    int stackAddition = Math.Min(Main.mouseItem.stack, stackAvailable);
                    Main.mouseItem.stack -= stackAddition;
                    storedPotions[i].stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                    Recipe.FindRecipes();
                    if (Main.mouseItem.stack <= 0)
                        Main.mouseItem.TurnToAir();
                }
            }
            if (!Main.mouseItem.IsAir && storedPotions.Count < 20) {
                storedPotions.Add(Main.mouseItem.Clone());
                Main.mouseItem.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            if (context != 114514 && Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot, inventory[slot].prefix);
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (storedPotions is not null && storedPotions.Count > 0) {
                if (storedPotions.Count >= 20) {
                    tooltips.Add(new(Mod, "PotionBagCurrent", MyUtils.GetText("Tips.PotionBagCurrentFull")) {
                        OverrideColor = Color.LightGreen
                    });
                }
                else {
                    tooltips.Add(new(Mod, "PotionBagCurrent", MyUtils.GetTextWith("Tips.PotionBagCurrent", new { StoredCount = storedPotions.Count })) {
                        OverrideColor = Color.LightGreen
                    });
                }
                for (int i = 0; i < storedPotions.Count; i++) {
                    var potion = storedPotions[i];
                    var color = Color.SkyBlue;
                    bool available = potion.stack >= MyUtils.Config.NoConsume_PotionRequirement;
                    string text = $"[i:{potion.type}] [{Lang.GetItemNameValue(potion.type)}] x{potion.stack}";
                    // 有30个
                    if (available) {
                        if (!MyUtils.Config.NoConsume_Potion || !InfBuffPlayer.Get(Main.LocalPlayer).CheckInfBuffEnable(potion.buffType)) { // 被禁用了
                            text += $"  {MyUtils.GetText("Tips.PotionBagDisabled")}";
                        }
                        else {
                            text += $"  {MyUtils.GetText("Tips.PotionBagAvailable")}";
                            color = Color.LightGreen;
                        }
                    }
                    // 没有30个
                    else {
                        text += $"  {MyUtils.GetText("Tips.PotionBagUnavailable")}";
                    }
                    tooltips.Add(new(Mod, $"PotionBagP{i}", text) {
                        OverrideColor = color
                    });
                }
            }
            else {
                tooltips.Add(new(Mod, "PotionBagNone", MyUtils.GetText("Tips.PotionBagNone")) {
                    OverrideColor = Color.SkyBlue
                });
            }
        }

        public override void AddRecipes() {
            base.AddRecipes();
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.EyeOfCthulhuBossBag);
            Item.consumable = false;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.LightRed;
            Item.expert = false;
        }

        public override void LoadData(TagCompound tag) {
            List<Item> list = new();
            foreach (var entry in tag.GetList<TagCompound>("storedPotions")) {
                if (!entry.TryGet("potion", out Item potion) || potion.IsAir) {
                    continue;
                }
                list.Add(potion);
            }
            storedPotions = list;
        }

        public override void SaveData(TagCompound tag) {
            if (storedPotions is not null && storedPotions.Count != 0) {
                tag["storedPotions"] = storedPotions.Select(item => new TagCompound {
                    ["potion"] = item
                }).ToList();
            }
        }
    }
}
