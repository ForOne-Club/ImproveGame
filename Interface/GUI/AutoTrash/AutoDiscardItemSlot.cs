using ImproveGame.Common.Animations;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using System;
using Terraria.ID;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class AutoDiscardItemSlot : BaseItemSlot
{
    public readonly List<Item> AutoDiscardItems;
    public readonly int Index;
    public override Item Item
    {
        get
        {
            return AutoDiscardItems.IndexInRange(Index) ? AutoDiscardItems[Index] : AirItem;
        }
    }

    public readonly Texture2D textureTrash;

    public AutoDiscardItemSlot(List<Item> items, int index)
    {
        AutoDiscardItems = items;
        Index = index;
        SetBaseItemSlotValues(true, false);
        textureTrash = GetTexture("UI/AutoTrash/Trash").Value;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        List<Item> trashItems = AutoTrashPlayer.Instance.TrashItems;
        if (!(Main.mouseItem?.IsAir ?? true)) // 当鼠标上有物品
        {
            int trashIndex = trashItems.FindIndex(item => item.type == Item.type);
            if (trashIndex > -1)
            {
                int worldIndex = Item.NewItem(null, Main.LocalPlayer.getRect(), trashItems[trashIndex].type, trashItems[trashIndex].stack, true);
                if (worldIndex > -1)
                {
                    trashItems[trashIndex].active = true;
                    trashItems[trashIndex].position = Main.item[worldIndex].position;
                    Main.item[worldIndex] = trashItems[trashIndex];
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, worldIndex, 0);
                }

                trashItems.RemoveAt(trashIndex);
            }
        }
        else if (!Item.IsAir) // 当鼠标上没物品
        {
            int trashIndex = trashItems.FindIndex(item => item.type == Item.type);
            if (trashIndex > -1)
            {
                Main.mouseItem = trashItems[trashIndex];
                trashItems.RemoveAt(trashIndex);
            }
        }

        AutoDiscardItems.RemoveAt(Index);
        SoundEngine.PlaySound(SoundID.Grab);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (IsMouseHovering)
        {
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Main.spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            // 此处设置是为了确实中间在不使用 SpriteBatch 的合批绘制模式的时候 RasterizerState 失效
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            Main.spriteBatch.Begin(0, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.Default,
                rasterizerState, null, Main.UIScaleMatrix);

            Vector2 pos = GetDimensions().Position();
            Vector2 size = GetDimensionsSize();

            SDFRectangle.NoBorder(pos + new Vector2(4f), size - new Vector2(8f), new Vector4(8f), HoverTimer.Lerp(Color.Transparent, UIColor.TitleBg2));
            spriteBatch.Draw(textureTrash, pos + size / 2f, null, HoverTimer.Lerp(Color.Transparent, Color.White), 0f, textureTrash.Size() / 2f, 1f, 0, 0);
        }
    }
}
