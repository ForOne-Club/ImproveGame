using ImproveGame.Common;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Packets.NetChest;
using Microsoft.Xna.Framework.Input;
using System.Collections.ObjectModel;

namespace ImproveGame.UIFramework.Common;

public class ChestPreviewUISystem : ModSystem
{
    private class PreviewGlobalTile : GlobalTile
    {
        public override void MouseOverFar(int i, int j, int type)
        {
            if (!Main.keyState.IsKeyDown(Keys.LeftAlt))
                return;

            var t = Main.tile[i, j];
            if (!t.HasTile || !TileID.Sets.IsAContainer[type])
                return;

            int originX = i;
            int originY = j;
            if (TileID.Sets.BasicDresser[type])
            {
                originY = j;
                originX = i - t.TileFrameX % 54 / 18;
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
                return;

            _syncCooldown--;
            if (_syncCooldown <= 0)
            {
                ChestItemOperation.RequestChestItem(chestIndex);
                _syncCooldown = 60; // 防止发包太多
            }

            _previewItems = Main.chest[chestIndex].item;
            _hasChest = true;
            // Main.cursorOverride = CursorOverrideID.Magnifiers;
        }
    }
    
    private static Item[] _previewItems;
    private static bool _hasChest;
    private static int _syncCooldown;

    public override void UpdateUI(GameTime gameTime)
    {
        _hasChest = false;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (index != -1)
            layers.Insert(index + 1, new LegacyGameInterfaceLayer("ImproveGame: Chest Preview GUI", () =>
            {
                if (Main.HoveringOverAnNPC || Main.LocalPlayer.mouseInterface || !_hasChest || _previewItems is null ||
                    _previewItems.Any(i => i is null))
                    return true;

                // High FPS Support支持
                Main.cursorOverride = CursorOverrideID.Magnifiers;
                Main.LocalPlayer.cursorItemIconEnabled = false;

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