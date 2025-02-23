using ImproveGame.UI.ModernConfig.OptionElements;
using ImproveGame.UIFramework.SUIElements;
using Terraria.ModLoader.Config.UI;

namespace ImproveGame.UI.ModernConfig;

public class TooltipPanel : SUIPanel
{
    internal static TooltipPanel Instance;

    public TooltipTextElement Text;
    public SUIDrawingImage PreviewDrawer;
    public ModernConfigOption currentOption;

    public TooltipPanel(Color color) : base(color, color)
    {
        Instance = this;

        SetPadding(16, 10, 16, 10);
        Text = new TooltipTextElement();
        Text.JoinParent(this);
        PreviewDrawer = new SUIDrawingImage(view =>
        {
            if (currentOption == null) return;
            if (!CategorySidePanel.ModdedPreviews.TryGetValue(currentOption.VariableInfo, out CategorySidePanel.PreviewDrawing drawingMethod))
                foreach (var pair in CategorySidePanel.ModdedPreviews) 
                {
                    if (pair.Key.fieldInfo == currentOption.VariableInfo.fieldInfo && pair.Key.IsField)
                        drawingMethod = pair.Value;
                    else if (pair.Key.propertyInfo == currentOption.VariableInfo.propertyInfo && pair.Key.IsProperty)
                        drawingMethod = pair.Value;
                    if (drawingMethod != null) break;
                }
            if (drawingMethod == null) return;

            drawingMethod.Invoke(view, currentOption.Config, currentOption.VariableInfo, currentOption.Item, currentOption.List, currentOption.index);
        })
        {
            Width = new(0, 1),
            Height = new(0, 1),
            RelativeMode = UIFramework.BaseViews.RelativeMode.None
        };
        PreviewDrawer.JoinParent(this);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var dimension = PreviewDrawer.GetDimensions();
        if (Instance.Text.TextOrKey == "")
            Instance.Text.TextOrKey = GetText("ModernConfig.NoTooltip");
        base.Draw(spriteBatch);
        Instance.Text.TextOrKey = ConfigOptionsPanel.CurrentCategory?.Tooltip ?? "";
        currentOption = null;
    }

    public static void SetText(string text)
    {
        Instance.Text.TextOrKey = text;
    }

    public static void SetOption(ModernConfigOption option) => Instance.currentOption = option;
}