using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items
{
    public class PotionBag : ModItem
    {
        [CloneByReference]
        public List<Item> storedPotions = new();

        public override bool CanRightClick() => storedPotions is not null && storedPotions.Count != 0;

        public override void RightClick(Player player) {
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), storedPotions[^1], storedPotions[^1].stack);
            storedPotions.RemoveAt(storedPotions.Count - 1);
        }

        public override bool ConsumeItem(Player player) => false;

        public override void Load() {
            On.Terraria.UI.ItemSlot.OverrideLeftClick += TryPutInPotions;
        }

        private bool TryPutInPotions(On.Terraria.UI.ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot) {
            var item = inv[slot];
            // 很多的条件
            if (item.IsAir || item.type != ModContent.ItemType<PotionBag>() ||
                ItemSlot.ShiftInUse || ItemSlot.ControlInUse || context != ItemSlot.Context.InventoryItem ||
                Main.mouseItem.IsAir || !Main.mouseItem.consumable || Main.mouseItem.buffType <= 0 ||
                item.ModItem is null || item.ModItem is not PotionBag || !Main.mouseLeft || !Main.mouseLeftRelease) {
                return orig.Invoke(inv, context, slot);
            }
            var potionBag = item.ModItem as PotionBag;
            for (int i = 0; i < potionBag.storedPotions.Count; i++) {
                if (potionBag.storedPotions[i].IsAir) {
                    potionBag.storedPotions.RemoveAt(i);
                    i--;
                    continue;
                }
                if (potionBag.storedPotions[i].type == Main.mouseItem.type && potionBag.storedPotions[i].stack < potionBag.storedPotions[i].maxStack && ItemLoader.CanStack(potionBag.storedPotions[i], Main.mouseItem)) {
                    int stackAvailable = potionBag.storedPotions[i].maxStack - potionBag.storedPotions[i].stack;
                    int stackAddition = Math.Min(Main.mouseItem.stack, stackAvailable);
                    Main.mouseItem.stack -= stackAddition;
                    potionBag.storedPotions[i].stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                    Recipe.FindRecipes();
                    if (Main.mouseItem.stack <= 0)
                        Main.mouseItem.TurnToAir();
                }
            }
            if (!Main.mouseItem.IsAir) {
                potionBag.storedPotions.Add(Main.mouseItem.Clone());
                Main.mouseItem.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot, inv[slot].prefix);
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (storedPotions is not null && storedPotions.Count > 0) {
                tooltips.Add(new(Mod, "PotionBagCurrent", MyUtils.GetText("Tips.PotionBagCurrent")) {
                    OverrideColor = Color.LightGreen
                });
                for (int i = 0; i < storedPotions.Count; i++) {
                    var potion = storedPotions[i];
                    bool available = potion.stack >= 30;
                    string text = $"[i:{potion.type}] [{Lang.GetItemNameValue(potion.type)}] x{potion.stack}";
                    text += $"  {MyUtils.GetText($"Tips.PotionBag{(available ? "Available" : "Unavailable")}")}";
                    tooltips.Add(new(Mod, $"PotionBagP{i}", text) {
                        OverrideColor = available ? Color.LightGreen : Color.SkyBlue
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
