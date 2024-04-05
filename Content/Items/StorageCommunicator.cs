﻿/*
Partly from Magic Storage Mod: https://github.com/blushiemagic/MagicStorage
This is the original LICENSE of Magic Storage Mod

MIT License

Copyright (c) 2017 Kaylee Minsuh Kim

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using ImproveGame.Common.ModHooks;
using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Tiles;
using ImproveGame.Packets.NetStorager;
using ImproveGame.UI.ExtremeStorage;
using ImproveGame.UIFramework;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;

public class StorageCommunicator : ModItem, IItemOverrideHover, IItemMiddleClickable
{
    public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

    public override bool IsLoadingEnabled(Mod mod) => Config.LoadModItems.ExtremeStorage;

    [CloneByReference] internal Dictionary<string, Point16> locationsByWorld = new();

    public Point16 Location
    {
        get => locationsByWorld.TryGetValue(Main.worldName, out var pos) ? pos : Point16.NegativeOne;
        set => locationsByWorld[Main.worldName] = value;
    }

    public override void ModifyTooltips(List<TooltipLine> lines)
    {
        Point16 location = Location;
        bool isSet = location is {X: >= 0, Y: >= 0};

        if (!isSet)
        {
            int indexTooltip1 = lines.FindIndex(static line => line.Mod == "Terraria" && line.Name == "Tooltip1");
            if (indexTooltip1 >= 0)
                lines.RemoveAt(indexTooltip1);
        }

        int index = lines.FindIndex(static line => line.Mod == "Terraria" && line.Name == "Tooltip0");
        if (index < 0)
            return;

        GetMeterCoords(location.ToPoint(), out string compassText, out string depthText);
        string text = isSet
            ? this.GetLocalization("CurrentlySetTo").WithFormatArgs(compassText, depthText).Value
            : this.GetLocalizedValue("NotSet");

        lines[index].Text = text;
    }

    public override void SaveData(TagCompound tag)
    {
        locationsByWorld ??= new Dictionary<string, Point16>();

        tag["locations"] = locationsByWorld
            .Select(kvp => new TagCompound
            {
                ["world"] = kvp.Key,
                ["X"] = kvp.Value.X,
                ["Y"] = kvp.Value.Y
            })
            .ToList();
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.GetList<TagCompound>("locations") is List<TagCompound> locations)
            locationsByWorld = locations.ToDictionary(t => t.GetString("world"),
                t => new Point16(t.GetShort("X"), t.GetShort("Y")));
    }

    public override void NetSend(BinaryWriter writer)
    {
        Point16 location = Location;
        writer.Write(location.X);
        writer.Write(location.Y);
    }

    public override void NetReceive(BinaryReader reader)
    {
        Location = new Point16(reader.ReadInt16(), reader.ReadInt16());
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
            DoOpenStorage();

        return true;
    }

    public bool OverrideHover(Item[] inventory, int context, int slot)
    {
        ((IItemMiddleClickable)this).HandleHover(inventory, context, slot);

        return false;
    }

    public void OnMiddleClicked(Item item)
    {
        DoOpenStorage();
    }

    private void DoOpenStorage()
    {
        Point16 location = Location;
        if (location is {X: >= 0, Y: >= 0})
        {
            Tile tile = Main.tile[location.X, location.Y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<ExtremeStorage>() || tile.TileFrameX != 0 ||
                tile.TileFrameY != 0 || !TEExtremeStorage.TryGet(out var storage, location) ||
                TileLoader.GetTile(Main.tile[location.ToPoint()].TileType) is not ExtremeStorage storageTile)
            {
                Main.NewText(this.GetLocalizedValue("MissingStorage"));
                return;
            }

            if (Main.netMode is NetmodeID.MultiplayerClient && !storageTile.ServerOpenRequest &&
                SidedEventTrigger.IsClosed(UISystem.Instance.ExtremeStorageGUI))
            {
                OpenStoragePacket.Get(storage.ID).Send();
                return;
            }

            storageTile.ServerOpenRequest = false;

            ExtremeStorageGUI.Storage = storage;
            SidedEventTrigger.ToggleViewBody(UISystem.Instance.ExtremeStorageGUI);
        }
        else
        {
            Main.NewText(this.GetLocalizedValue("Unlocated"));
        }
    }

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.maxStack = 1;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 5);
    }

    public override Vector2? HoldoutOffset() => new Vector2(-8f, -10f);

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Sapphire, 3)
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddRecipeGroup(RecipeSystem.AnyGoldBar, 4)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}