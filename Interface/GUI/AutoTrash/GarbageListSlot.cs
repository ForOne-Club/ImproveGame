using ImproveGame.Common.Animations;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;

namespace ImproveGame.Interface.GUI.AutoTrash;

/// <summary>
/// 垃圾列表槽
/// </summary>
public class GarbageListSlot : BaseItemSlot
{
    /// <summary>
    /// 垃圾列表
    /// </summary>
    public readonly List<Item> Garbages;
    public readonly int Index;
    public override Item Item
    {
        get
        {
            return Garbages.IndexInRange(Index) ? Garbages[Index] : AirItem;
        }
    }

    public Texture2D TrashTexture { get; protected set; }

    public GarbageListSlot(List<Item> items, int index)
    {
        Garbages = items;
        Index = index;
        SetBaseItemSlotValues(true, false);
        SetSizePixels(44, 44);
        ItemIconMaxWidthAndHeight = 32f;
        ItemIconScale = 0.85f;
        TrashTexture = ModAsset.TakeOutFromTrash.Value;

        HoverTimer.Close();
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        List<Item> trashItems = AutoTrashPlayer.Instance.TrashItems;

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

        Garbages.RemoveAt(Index);
        SoundEngine.PlaySound(SoundID.Grab);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        ItemIconMaxWidthAndHeight = 32f * HoverTimer.Lerp(1f, 0.9f);
        ItemIconOffset = HoverTimer.Lerp(new Vector2(), GetInnerDimensions().Size() * 0.15f);
        ItemIconOffset.X *= 0;
        ItemIconResize = HoverTimer.Lerp(new Vector2(), -GetInnerDimensions().Size() * 0.15f);

        base.DrawSelf(spriteBatch);

        if (HoverTimer.Opening || HoverTimer.Closing || IsMouseHovering)
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

            Vector2 boxSize = (size - new Vector2(8f)) * HoverTimer.Lerp(0.5f, 1f);

            // SDFRectangle.NoBorder(pos + (size - boxSize) / 2f, boxSize, new Vector4(8f), HoverTimer.Lerp(Color.Transparent, UIColor.TitleBg2 * 0.75f));
            spriteBatch.Draw(TrashTexture, pos + size / 3f * new Vector2(2, 1), null,
                HoverTimer.Lerp(Color.Transparent, Color.White), 0f,
                TrashTexture.Size() / 2f, HoverTimer.Lerp(0.5f, 0.65f), 0, 0);
        }
    }
}
