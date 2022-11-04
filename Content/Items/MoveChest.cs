using ImproveGame.Common;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Packets;
using ImproveGame.Common.Packets.Chest;
using ImproveGame.Common.Players;
using System.Collections.ObjectModel;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace ImproveGame.Content.Items;

/// <summary>
/// 用于寻找其他服务器中对应的物品
/// </summary>
public record ItemPosition(byte player, int slot)
{
    /// <summary>
    /// 该物品所属的玩家的 <see cref="Player.whoAmI"/>
    /// </summary>
    public byte player = player;

    /// <summary>
    /// 该物品所在的栏位
    /// </summary>
    public int slot = slot;
}

public class MoveChest : ModItem
{
    /// <summary>
    /// 强制冷却，放置过快反复与服务器交换出bug
    /// </summary>
    public const int MaxForceCoolDown = 10;

    /// <summary>
    /// 箱子名称，服务器暂时无法同步
    /// </summary>
    public string chestName = null;

    /// <summary>
    /// 箱子的类型
    /// </summary>
    public ushort chestType = 0;

    /// <summary>
    /// 强制冷却倒计时
    /// </summary>
    public int forceCoolDown = 0;

    /// <summary>
    /// 是否有箱子
    /// </summary>
    public bool hasChest = false;

    /// <summary>
    /// 箱子中的物品
    /// </summary>
    public Item[] items = null;

    /// <summary>
    /// 模组箱子的全名
    /// </summary>
    public string modChestName = null;

    /// <summary>
    /// 箱子的样式
    /// </summary>
    public int style = 0;

    /// <summary>
    /// 重写了Clone
    /// </summary>
    public override bool IsCloneable => true;

    /// <summary>
    /// 在所有端执行的函数，执行主要效果
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public override bool CanUseItem(Player player)
    {
        if (forceCoolDown > 0)
        {
            return false;
        }

        var coord = player.GetModPlayer<ImprovePlayer>().MouseWorld.ToTileCoordinates();

        if (!player.IsInTileInteractionRange(coord.X, coord.Y)) // 必须在可操作范围内
        {
            return true;
        }

        // 放置箱子
        if (hasChest)
        {
            if (!TileObject.CanPlace(coord.X, coord.Y, chestType, style, 1, out _, true))
            {
                return false;
            }
            hasChest = false;
            forceCoolDown = MaxForceCoolDown;
            Item.createTile = -1;

            if (Main.myPlayer == player.whoAmI)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    int index = WorldGen.PlaceChest(coord.X, coord.Y, chestType, false, style);
                    if (index == -1)
                    {
                        Mod.Logger.Error("Unexpected Error: Unable to Place Chest");
                        return false;
                    }
                    SoundEngine.PlaySound(SoundID.Dig, player.position);
                    var chest = Main.chest[index];
                    chest.item = items;
                    chest.name = chestName ?? "";
                    Reset();
                }
                else
                {
                    items ??= new Item[Chest.maxItems];
                    chestName ??= string.Empty;
                    PlaceChestPacket.Get(coord, chestType, style, items, chestName, new((byte)player.whoAmI, Array.IndexOf(player.inventory, Item))).Send(-1, -1, false);
                    Reset();
                }
            }
            return true;
        }

        var tile = Main.tile[coord.X, coord.Y];
        // 拿走箱子
        if (tile.HasTile && TileID.Sets.BasicChest[tile.TileType])
        {
            forceCoolDown = MaxForceCoolDown;
            hasChest = true;
            chestType = tile.TileType;
            if (chestType >= TileID.Count)
            {
                modChestName = ModContent.GetModTile(chestType).FullName;
            }
            style = TileObjectData.GetTileStyle(tile);

            // 坐标修正到箱子左上角
            coord -= new Point((tile.TileFrameX / 18) - (style * 2), tile.TileFrameY / 18);
            Main.tile[coord.X, coord.Y].ClearTile();
            Main.tile[coord.X + 1, coord.Y].ClearTile();
            Main.tile[coord.X, coord.Y + 1].ClearTile();
            Main.tile[coord.X + 1, coord.Y + 1].ClearTile();
            
            SoundEngine.PlaySound(SoundID.Dig, player.position);

            // 客户端可能没有缓存箱子的物品
            for (int i = 0; i < Main.chest.Length; i++)
            {
                var chest = Main.chest[i];
                if (chest == null)
                {
                    continue;
                }

                if (chest.x != coord.X || chest.y != coord.Y)
                {
                    continue;
                }
                //Copy Item and Destroy Chest
                items = chest.item;
                chestName = chest.name;
                Chest.DestroyChestDirect(coord.X, coord.Y, i);

                if (Main.netMode == NetmodeID.Server)
                {
                    TakeChestPacket.Get(new((byte)player.whoAmI, Array.IndexOf(player.inventory, Item)), items, chestName).Send(-1, -1, false);
                }

                return true;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Mod.Logger.Error("Unexpected error: Chest not found");
            }
            return true;
        }
        return true;
    }

    /// <summary>
    /// 对Item数组的浅拷贝<br/>
    /// 因为当物品拿在手上时会疯狂触发Clone，深复制套娃物品开销巨大<br/>
    /// </summary>
    /// <param name="newEntity"></param>
    /// <returns></returns>
    public override ModItem Clone(Item newEntity)
    {
        var clone = base.Clone(newEntity) as MoveChest;
        clone.items = items?.ToArray();
        return clone;
    }

    public override void HoldItem(Player player)
    {
        if (forceCoolDown > 0)
        {
            forceCoolDown--;
        }

        if (player.itemAnimation == 0)
        {
            //为了显示放置预览
            Item.createTile = hasChest ? chestType : -1;
            Item.placeStyle = style;
        }
        if (hasChest)
        {
            ModifyPlayerSpeed(player);
        }
    }

    /// <summary>
    /// 读取包括物品在内的数据
    /// </summary>
    /// <param name="tag"></param>
    public override void LoadData(TagCompound tag)
    {
        if (tag.Count == 0)
        {
            Reset();
            return;
        }

        hasChest = true;
        chestType = (ushort)tag.GetAsShort("chest");
        style = tag.GetAsInt("style");
        if (chestType >= TileID.Count)
        {
            modChestName = tag.GetString("mod");
            chestType = ModContent.TryFind<ModTile>(modChestName, out var tile) ? tile.Type : TileID.Containers;
        }
        chestName = tag.GetString("name");
        items = tag.Get<Item[]>("items");
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        if (hasChest)
        {
            tooltips.Add(new(Mod, "TooltipHeavy", GetText("MoveChest.Heavy")));
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        hasChest = reader.ReadBoolean();
        if (!hasChest)
            return;
        chestType = reader.ReadUInt16();
        style = reader.ReadInt32();
        if (chestType >= TileID.Count)
        {
            modChestName = reader.ReadString();
        }
        chestName = reader.ReadString();
        items = reader.ReadItemArray();
    }
    
    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(hasChest);
        if (!hasChest)
            return;
        writer.Write(chestType);
        writer.Write(style);
        if (chestType >= TileID.Count)
        {
            writer.Write(modChestName);
        }
        writer.Write(chestName);
        writer.Write(items);
    }

    public void Reset()
    {
        hasChest = false;
        items = null;
        modChestName = null;
        chestType = 0;
        style = 0;
        Item.createTile = -1;
    }

    public override void SaveData(TagCompound tag)
    {
        if (!hasChest)
        {
            return;
        }

        tag.Add("chest", (short)chestType);
        tag.Add("style", style);
        if (chestType >= TileID.Count)
        {
            tag.Add("mod", modChestName);
        }
        tag["name"] = chestName;
        if (items is not null)
            tag["items"] = items;
    }

    public override void SetDefaults()
    {
        Item.width = 1;
        Item.height = 1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.rare = ItemRarityID.Red;
        Item.autoReuse = true;
        Item.tileBoost = 6;
    }

    public override void UpdateInventory(Player player)
    {
        if (hasChest && player.HeldItem != Item)
        {
            ModifyPlayerSpeed(player);
        }
    }

    /// <summary>
    /// 当携带箱子时降低玩家移动速度，增加坠落速度
    /// </summary>
    /// <param name="player"></param>
    private static void ModifyPlayerSpeed(Player player)
    {
        player.accRunSpeed -= 0.5f;
        player.maxRunSpeed = 0.5f;
        player.jumpSpeedBoost *= 0.5f;
        player.maxFallSpeed += 0.5f;
        player.moveSpeed -= 0.1f;
    }

    /// <summary>
    /// 显示其内箱子的物品
    /// </summary>
    public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
    {
        if (!hasChest)
            return base.PreDrawTooltip(lines, ref x, ref y);

        List<TooltipLine> list = new();

        for (int i = 0; i < 4; i++)
        {
            string line = "";
            for (int j = 0; j <= 9; j++)
            {
                line += BgItemTagHandler.GenerateTag(items[i * 10 + j]);
            }
            list.Add(new(Mod, $"ChestItemLine_{i}", line));
        }

        TagItem.DrawTooltips(lines, list, x, y);

        return base.PreDrawTooltip(lines, ref x, ref y);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddIngredient(ItemID.Bone, 20)
            .AddRecipeGroup(RecipeGroupID.Wood, 18)
            .AddIngredient(ItemID.Diamond)
            .AddTile(TileID.Anvils).Register();
    }
}