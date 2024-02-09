using ImproveGame.UIFramework;

namespace ImproveGame.UI.ExtremeStorage.ToolButtons;

public class RecipeToggleButton : ToolButtonBase
{
    public override void OnTakeEffect() =>
        ExtremeStorageGUI.DisplayCrafting = !ExtremeStorageGUI.DisplayCrafting;

    public override Texture2D Texture =>
        TextureAssets.CraftToggle[ExtremeStorageGUI.DisplayCrafting.ToInt() * 2 + IsMouseHovering.ToInt()].Value;

    public override string HoverText => Language.GetTextValue("GameUI.CraftingWindow");
}