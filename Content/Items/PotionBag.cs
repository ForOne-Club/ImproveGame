using ImproveGame.Common.ModHooks;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.GUI.BannerChest;
using Terraria.GameContent.Creative;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace ImproveGame.Content.Items
{
    public class PotionBag : ModItem, IItemOverrideLeftClick, IItemOverrideHover, IPackageItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.PotionBag;
        public List<Item> StoredPotions = new();
        public bool AutoStorage { get; set; }
        public bool AutoSort { get; set; }

        // 克隆内容不克隆引用
        public override ModItem Clone(Item newEntity)
        {
            PotionBag bag = base.Clone(newEntity) as PotionBag;
            bag.StoredPotions = new(StoredPotions); // 创建一个新的集合，依旧会拷贝 list 内的引用，但是它本身是一个新的对象。
            return bag;
        }

        public override bool CanRightClick() => StoredPotions != null;

        public override void RightClick(Player player)
        {
            if (PackageGUI.Visible && PackageGUI.StorageType is StorageType.Potions)
                UISystem.Instance.PackageGUI.Close();
            else
                UISystem.Instance.PackageGUI.Open(StoredPotions, Item.Name, StorageType.Potions, this);

            // player.QuickSpawnItem(player.GetSource_OpenItem(Type), storedPotions[^1], storedPotions[^1].stack);
            // storedPotions.RemoveAt(storedPotions.Count - 1);
        }

        public override bool ConsumeItem(Player player) => false;

        /// <summary>
        /// 只有在这些地方才可以放药水进去
        /// </summary>
        private static readonly List<int> availableContexts = new()
        {
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

            PutInPackage(ref Main.mouseItem);
            if (context != 114514 && Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot, inventory[slot].prefix);
            }

            return true;
        }

        public void PutInPackage(ref Item item)
        {
            for (int i = 0; i < StoredPotions.Count; i++)
            {
                if (StoredPotions[i].IsAir)
                {
                    StoredPotions.RemoveAt(i);
                    i--;
                    continue;
                }

                if (StoredPotions[i].type == item.type && StoredPotions[i].stack < StoredPotions[i].maxStack &&
                    ItemLoader.CanStack(StoredPotions[i], item))
                {
                    int stackAvailable = StoredPotions[i].maxStack - StoredPotions[i].stack;
                    int stackAddition = Math.Min(item.stack, stackAvailable);
                    item.stack -= stackAddition;
                    StoredPotions[i].stack += stackAddition;
                    SoundEngine.PlaySound(SoundID.Grab);
                    if (item.stack <= 0)
                        item.TurnToAir();
                }
            }

            if (!item.IsAir && StoredPotions.Count < 200)
            {
                StoredPotions.Add(item.Clone());
                item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
            }

            // 依照type对物品进行排序
            if (AutoSort)
            {
                Sort();
            }
        }

        public void Sort()
        {
            StoredPotions.Sort((a, b) =>
            {
                return a.type.CompareTo(b.type) + (a.stack > b.stack ? 1 : (a.stack == b.stack ? 0 : -1)) * 10;
            });
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // 决定文本显示的是“开启”还是“关闭”
            if (ItemInInventory)
            {
                string text = (PackageGUI.Visible && PackageGUI.StorageType is StorageType.Potions) ?
                    GetTextWith($"Tips.MouseMiddleClose", new { ItemName = Item.Name }) :
                    GetTextWith($"Tips.MouseMiddleOpen", new { ItemName = Item.Name });
                tooltips.Add(new TooltipLine(Mod, "CreateWand", text) { OverrideColor = Color.LightGreen });
            }

            ItemInInventory = false;

            if (StoredPotions is not null && StoredPotions.Count > 0)
            {
                string storeText = StoredPotions.Count >= 200
                    ? GetText("Tips.PotionBagCurrentFull")
                    : GetTextWith("Tips.PotionBagCurrent", new { StoredCount = StoredPotions.Count });
                tooltips.Add(new(Mod, "PotionBagCurrent", storeText)
                {
                    OverrideColor = Color.LightGreen
                });

                // 20+类药水时不显示详细信息
                int cow = 0;
                if (StoredPotions.Count > 20)
                {
                    string cachedText = string.Empty;
                    for (int i = 0; i < StoredPotions.Count; i++)
                    {
                        Item potion = StoredPotions[i];
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
                    for (int i = 0; i < StoredPotions.Count; i++)
                    {
                        var potion = StoredPotions[i];
                        var color = Color.SkyBlue;
                        bool available = potion.stack >= Config.NoConsume_PotionRequirement;
                        string text = $"[i/s{potion.stack}:{potion.type}] [{Lang.GetItemNameValue(potion.type)}]";
                        // 有30个
                        if (available)
                        {
                            if (!Config.NoConsume_Potion || !InfBuffPlayer.CheckInfBuffEnable(potion.buffType))
                            {
                                // 被禁用了
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
                            text +=
                                $"  {GetText("Tips.PotionBagUnavailable")} ({potion.stack}/{Config.NoConsume_PotionRequirement})";
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
            if (line.Mod == Mod.Name && line.Name.StartsWith("PotionBagP") &&
                Main.SettingsEnabled_OpaqueBoxBehindTooltips)
            {
                var font = FontAssets.MouseText.Value;
                var position = new Vector2(line.X, line.Y);
                var color = line.OverrideColor ?? line.Color;
                TextSnippet[] snippets = ChatManager.ParseMessage(line.Text, color).ToArray();
                ChatManager.ConvertNormalSnippets(snippets);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, snippets, position, Color.White, 0f,
                    Vector2.Zero, Vector2.One, out _, -1);
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

        public override void SetStaticDefaults() =>
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;

        public override void LoadData(TagCompound tag)
        {
            (this as IPackageItem).ILoadData(tag);
            StoredPotions = tag.Get<List<Item>>("potions");
            StoredPotions ??= new();

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

            StoredPotions = list;
        }

        public override void SaveData(TagCompound tag)
        {
            (this as IPackageItem).ISaveData(tag);
            tag["potions"] = StoredPotions;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(StoredPotions.ToArray());
        }

        public override void NetReceive(BinaryReader reader)
        {
            StoredPotions = new(reader.ReadItemArray());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.WorkBenches).Register();
        }

        private bool ItemInInventory;

        public bool OverrideHover(Item[] inventory, int context, int slot)
        {
            if (context == ItemSlot.Context.InventoryItem)
            {
                ItemInInventory = true;
                if (Main.mouseMiddle && Main.mouseMiddleRelease)
                    RightClick(Main.LocalPlayer);
            }

            return false;
        }
    }
}