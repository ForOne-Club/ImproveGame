using ImproveGame.Common.ModHooks;
using ImproveGame.Common.Players;
using ImproveGame.Interface.BannerChestUI;
using ImproveGame.Interface.Common;
using NetSimplified;
using System.Collections.Generic;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace ImproveGame.Content.Items
{
    public class PotionBag : ModItem, IItemOverrideLeftClick, IPackage
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems;

        public List<Item> storedPotions = new();
        private bool autoStorage;
        private bool autoSort;
        public bool AutoStorage { get => autoStorage; set => autoStorage = value; }
        public bool AutoSort { get => autoSort; set => autoSort = value; }

        // 克隆内容不克隆引用
        public override ModItem Clone(Item newEntity)
        {
            PotionBag bag = base.Clone(newEntity) as PotionBag;
            bag.storedPotions = new(storedPotions); // 创建一个新的集合，依旧会拷贝 list 内的引用，但是它本身是一个新的对象。
            return bag;
        }

        public override bool CanRightClick() => storedPotions is not null;

        public override void RightClick(Player player)
        {
            UISystem.Instance.PackageGUI.Open(storedPotions, Item.Name, PackageGUI.StorageType.Potions, this);
            // player.QuickSpawnItem(player.GetSource_OpenItem(Type), storedPotions[^1], storedPotions[^1].stack);
            // storedPotions.RemoveAt(storedPotions.Count - 1);
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

        public bool OverrideLeftClick(Item[] inventory, int context, int slot)
        {
            // 很多的条件
            if (ItemSlot.ShiftInUse || ItemSlot.ControlInUse || !availableContexts.Contains(context) ||
                Main.mouseItem.IsAir || !Main.mouseItem.consumable || Main.mouseItem.buffType <= 0)
            {
                return false;
            }
            PutInPotionBag(storedPotions, ref Main.mouseItem, autoSort);
            if (context != 114514 && Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot, inventory[slot].prefix);
            }
            return true;
        }

        public static bool PutInPotionBag(List<Item> storedPotions, ref Item item, bool AutoSort)
        {
            for (int i = 0; i < storedPotions.Count; i++)
            {
                if (storedPotions[i].IsAir)
                {
                    storedPotions.RemoveAt(i);
                    i--;
                    continue;
                }
                if (storedPotions[i].type == item.type && storedPotions[i].stack < storedPotions[i].maxStack && ItemLoader.CanStack(storedPotions[i], item))
                {
                    int stackAvailable = storedPotions[i].maxStack - storedPotions[i].stack;
                    int stackAddition = Math.Min(item.stack, stackAvailable);
                    item.stack -= stackAddition;
                    storedPotions[i].stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                    Recipe.FindRecipes();
                    if (item.stack <= 0)
                        item.TurnToAir();
                }
            }
            if (!item.IsAir && storedPotions.Count < 200)
            {
                storedPotions.Add(item.Clone());
                item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }
            // 依照type对物品进行排序
            if (AutoSort)
                storedPotions.Sort((a, b) => { return a.type.CompareTo(b.type); });
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (storedPotions is not null && storedPotions.Count > 0)
            {
                string storeText = storedPotions.Count >= 200
                    ? GetText("Tips.PotionBagCurrentFull")
                    : GetTextWith("Tips.PotionBagCurrent", new {StoredCount = storedPotions.Count});
                tooltips.Add(new(Mod, "PotionBagCurrent", storeText)
                {
                    OverrideColor = Color.LightGreen
                });

                // 20+类药水时不显示详细信息
                int cow = 0;
                if (storedPotions.Count > 20)
                {
                    string cachedText = string.Empty;
                    for (int i = 0; i < storedPotions.Count; i++)
                    {
                        Item potion = storedPotions[i];
                        int stackDisplayed = potion.stack >= 99 ? 99 : potion.stack;
                        string text = $"[i/s{stackDisplayed}:{potion.type}]";
                        cachedText += text;
                        if ((i + 1) % 20 == 0)
                        {
                            tooltips.Add(new(Mod, $"PotionBagP{++cow}", cachedText));
                            cachedText = string.Empty;
                        }
                    }
                    if (!string.IsNullOrEmpty(cachedText))
                    {
                        tooltips.Add(new(Mod, "PotionBagPX", cachedText));
                    }
                }
                // 药水少于20类时显示详细信息
                else
                {
                    for (int i = 0; i < storedPotions.Count; i++)
                    {
                        var potion = storedPotions[i];
                        var color = Color.SkyBlue;
                        bool available = potion.stack >= Config.NoConsume_PotionRequirement;
                        string text = $"[i/s{potion.stack}:{potion.type}] [{Lang.GetItemNameValue(potion.type)}]";
                        // 有30个
                        if (available)
                        {
                            if (!Config.NoConsume_Potion || !InfBuffPlayer.CheckInfBuffEnable(potion.buffType))
                            { // 被禁用了
                                text += $"  {GetText("Tips.PotionBagDisabled")}";
                            }
                            else
                            {
                                text += $"  {GetText("Tips.PotionBagAvailable")}";
                                color = Color.LightGreen;
                            }
                        }
                        // 没有30个
                        else
                        {
                            text += $"  {GetText("Tips.PotionBagUnavailable")} ({potion.stack}/{Config.NoConsume_PotionRequirement})";
                        }
                        tooltips.Add(new(Mod, $"PotionBagP{i}", text)
                        {
                            OverrideColor = color
                        });
                    }
                }
            }
            else
            {
                tooltips.Add(new(Mod, "PotionBagNone", GetText("Tips.PotionBagNone"))
                {
                    OverrideColor = Color.SkyBlue
                });
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == Mod.Name && line.Name.StartsWith("PotionBagP") && Main.SettingsEnabled_OpaqueBoxBehindTooltips)
            {
                var font = FontAssets.MouseText.Value;
                var position = new Vector2(line.X, line.Y);
                var color = line.OverrideColor ?? line.Color;
                TextSnippet[] snippets = ChatManager.ParseMessage(line.Text, color).ToArray();
                ChatManager.ConvertNormalSnippets(snippets);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, snippets, position, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.EyeOfCthulhuBossBag);
            Item.consumable = false;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Red;
            Item.expert = false;
            Item.width = 48;
            Item.height = 42;
        }

        public override void SetStaticDefaults() => CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        
        public override void LoadData(TagCompound tag)
        {
            tag.TryGet(nameof(autoStorage), out autoStorage);
            tag.TryGet(nameof(autoSort), out autoSort);
            storedPotions = tag.Get<List<Item>>("potions");
            storedPotions ??= new();

            // 旧版迁移
            if (!tag.ContainsKey("storedPotions"))
            {
                return;
            }

            List<Item> list = new();
            foreach (var entry in tag.GetList<TagCompound>("storedPotions"))
            {
                if (!entry.TryGet("potion", out Item potion) || potion.IsAir)
                {
                    continue;
                }
                list.Add(potion);
            }
            storedPotions = list;
        }

        public override void SaveData(TagCompound tag)
        {
            tag[nameof(autoStorage)] = autoStorage;
            tag[nameof(autoSort)] = autoSort;
            tag["potions"] = storedPotions;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(storedPotions.ToArray());
        }

        public override void NetReceive(BinaryReader reader)
        {
            storedPotions = new(reader.ReadItemArray());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.WorkBenches).Register();
        }
    }
}
