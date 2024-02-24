using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Tiles;
using ImproveGame.UI.ExtremeStorage;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.DataStructures;

namespace ImproveGame.Content.Functions;

internal class PortableStationSystem : ModSystem
{
    public override void Load()
    {
        // 便携制作站
        IL_Player.AdjTiles += AddPortableStations;
    }

    private void AddPortableStations(ILContext il)
    {
        var c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdsfld<Main>(nameof(Main.playerInventory))))
            return;
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<Player>>(player =>
        {
            if (!Config.PortableCraftingStation)
                return;
            
            // 从玩家身上获取所有的无尽Buff物品
            CheckStationsFromPlayer(player);
            if (Config.ShareCraftingStation)
                CheckTeamPlayers(player.whoAmI, CheckStationsFromPlayer);
            
            // 从TE中获取所有的无尽Buff物品
            foreach ((int _, TileEntity tileEntity) in TileEntity.ByID)
            {
                if (tileEntity is not TEExtremeStorage {UsePortableStations: true} storage)
                {
                    continue;
                }

                var alchemyItems = storage.FindAllNearbyChestsWithGroup(ItemGroup.Furniture);
                alchemyItems.ForEach(i => CheckStations(Main.chest[i].item));
            }
        });
    }

    internal void CheckStationsFromPlayer(Player inventorySource) =>
        CheckStations(GetAllInventoryItemsList(inventorySource));

    /// <summary>
    /// 从某个玩家的各种物品栏中拿效果
    /// </summary>
    internal void CheckStations(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            int tileType = item.createTile;
            if (tileType > -1 && tileType < TileLoader.TileCount)
            {
                CheckChainedStations(tileType, Main.LocalPlayer);
            }

            if (ModIntegrationsSystem.PortableStations.TryGetValue(item.type, out var tileIDs))
            {
                foreach (int tileID in tileIDs)
                {
                    CheckChainedStations(tileID, Main.LocalPlayer);
                }
            }

            if (item.type is ItemID.WaterBucket or ItemID.BottomlessBucket)
            {
                Main.LocalPlayer.adjWater = true;
            }

            if (item.type is ItemID.LavaBucket or ItemID.BottomlessLavaBucket)
            {
                Main.LocalPlayer.adjLava = true;
            }

            if (item.type is ItemID.HoneyBucket /*or ItemID.BottomlessHoneyBucket*/)
            {
                Main.LocalPlayer.adjHoney = true;
            }
        }
    }

    // 一系列的Station链条操作，每个tileID都要经历一次的
    private static void CheckChainedStations(int tileType, Player player)
    {
        player.adjTile[tileType] = true;
        if (TileID.Sets.CountsAsWaterSource[tileType])
        {
            player.adjWater = true;
        }

        if (TileID.Sets.CountsAsLavaSource[tileType])
        {
            player.adjLava = true;
        }

        if (TileID.Sets.CountsAsHoneySource[tileType])
        {
            player.adjHoney = true;
        }

        switch (tileType)
        {
            case TileID.Hellforge:
            case TileID.GlassKiln:
                player.adjTile[TileID.Furnaces] = true;
                break;
            case TileID.AdamantiteForge:
                player.adjTile[TileID.Furnaces] = true;
                player.adjTile[TileID.Hellforge] = true;
                break;
            case TileID.MythrilAnvil:
                player.adjTile[TileID.Anvils] = true;
                break;
            case TileID.BewitchingTable:
            case TileID.Tables2:
            case TileID.PicnicTable:
                player.adjTile[TileID.Tables] = true;
                break;
            case TileID.AlchemyTable:
                player.adjTile[TileID.Bottles] = true;
                player.adjTile[TileID.Tables] = true;
                player.alchemyTable = true;
                break;
        }

        TileLoader.AdjTiles(player, tileType);
    }
}