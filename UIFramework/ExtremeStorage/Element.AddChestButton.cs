using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.UIElements;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.ExtremeStorage;

public class AddChestButton : TimerView
{
    public AddChestButton()
    {
        var showSize = ModItemList.GetSize(10, 4, 40, 4f);
        this.SetSize(showSize.X, 50f);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        ChestSelection.IsSelecting = !ChestSelection.IsSelecting;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        CalculatedStyle dimensions = GetDimensions();
        var position = dimensions.Position();
        var size = dimensions.Size();
        var center = dimensions.Center() + new Vector2(0, UIConfigs.Instance.GeneralFontOffsetY);

        Color borderColor = Color.Lerp(UIStyle.PanelBorder, UIStyle.ItemSlotBorderFav, HoverTimer.Schedule);
        SDFRectangle.HasBorder(position, size, new Vector4(10f), UIStyle.ButtonBg, 2, borderColor);

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