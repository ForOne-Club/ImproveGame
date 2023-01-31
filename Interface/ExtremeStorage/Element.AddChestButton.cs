using ImproveGame.Common.Animations;
using ImproveGame.Common.Configs;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.UIElements;
using Terraria.UI.Chat;

namespace ImproveGame.Interface.ExtremeStorage;

public class AddChestButton : HoverView
{
    public AddChestButton()
    {
        endWidth = 0;
        var showSize = ModItemList.GetSize(10, 4, 40, 4f);
        this.SetSize(showSize.X, 50f);
    }

    public override void Click(UIMouseEvent evt)
    {
        base.Click(evt);
        ChestSelection.IsSelecting = !ChestSelection.IsSelecting;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        CalculatedStyle dimensions = GetDimensions();
        var position = dimensions.Position();
        var size = dimensions.Size();
        var center = dimensions.Center() + new Vector2(0, UIConfigs.Instance.GeneralFontOffsetY);

        Color borderColor = Color.Lerp(UIColor.PanelBorder, UIColor.ItemSlotBorderFav, hoverTimer.Schedule);
        PixelShader.RoundedRectangle(position, size, new Vector4(10f), UIColor.ButtonBg, 2, borderColor);

        string text = GetText("UI.ExtremeStorage.AddChest");
        if (ChestSelection.IsSelecting)
            text = GetText("Common.Cancel");

        var origin = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One) / 2f;
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, center, Color.White,
            0f, origin, Vector2.One);
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }
}