using ImproveGame.Common;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Packets.NetChest;
using ImproveGame.Interface.BannerChest;
using ImproveGame.Interface.GUI;
using Microsoft.Xna.Framework.Input;
using System.Collections.ObjectModel;

namespace ImproveGame.Interface.Common;

public class ChestPreviewUISystem : ModSystem
{
    private static Item[] _previewItems;
    private static bool _hasChest;
    private static int _syncCooldown;

    public override void Load()
    {
        Terraria.On_Player.TileInteractionsMouseOver += ChestDisplayMouseOver;
    }

    private void ChestDisplayMouseOver(Terraria.On_Player.orig_TileInteractionsMouseOver orig, Player player, int myX,
        int myY)
    {
        orig.Invoke(player, myX, myY);

        if (myX != Player.tileTargetX || myY != Player.tileTargetY || !Main.keyState.IsKeyDown(Keys.LeftAlt))
            return;

        var t = Main.tile[myX, myY];
        if (!TileID.Sets.IsAContainer[t.TileType])
        {
            return;
        }

        int originX = myX;
        int originY = myY;
        if (TileID.Sets.BasicDresser[t.TileType])
        {
            originY = myY;
            originX = myX - t.TileFrameX % 54 / 18;
            if (t.TileFrameY % 36 != 0)
                originY--;
        }
        else
        {
            if (t.TileFrameX % 36 != 0)
                originX--;
            if (t.TileFrameY % 36 != 0)
                originY--;
        }

        int chestIndex = Chest.FindChest(originX, originY);
        if (chestIndex < 0 || Chest.IsLocked(originX, originY))
        {
            return;
        }

        _syncCooldown--;
        if (_syncCooldown <= 0)
        {
            ChestItemOperation.RequestChestItem(chestIndex);
            _syncCooldown = 60; // 防止发包太多
        }

        _previewItems = Main.chest[chestIndex].item;
        _hasChest = true;
        Main.cursorOverride = CursorOverrideID.Magnifiers;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Interact Item Icon");
        if (index != -1)
            layers.Insert(index + 1, new LegacyGameInterfaceLayer("ImproveGame: Chest Preview GUI", () =>
            {
                if (Main.HoveringOverAnNPC || Main.LocalPlayer.mouseInterface || !_hasChest || _previewItems is null ||
                    _previewItems.Any(i => i is null))
                    return true;

                _hasChest = false;
                List<TooltipLine> list = new();
                for (int i = 0; i < 4; i++)
                {
                    string line = "";
                    for (int j = 0; j <= 9; j++)
                    {
                        line += BgItemTagHandler.GenerateTag(_previewItems[i * 10 + j]);
                    }

                    list.Add(new(Mod, $"ChestItemLine_{i}", line));
                }

                TagItem.DrawTooltips(new ReadOnlyCollection<TooltipLine>(new List<TooltipLine>()), list, Main.mouseX,
                    Main.mouseY + 10);

                return true;
            }, InterfaceScaleType.UI));
    }
}