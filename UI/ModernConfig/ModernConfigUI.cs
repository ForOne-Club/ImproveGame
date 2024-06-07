using ImproveGame.Common;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.ModernConfig;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Modern Config UI")]
public sealed class ModernConfigUI : BaseBody
{
    public static ModernConfigUI Instance { get; set; }

    public override bool IsNotSelectable => !Enabled;

    public override bool Enabled { get; set; }

    public override bool CanSetFocusTarget(UIElement target)
        => (target != this && MainPanel.IsMouseHovering) || MainPanel.IsLeftMousePressed;
    
    // 主面板
    public SUIPanel MainPanel;
    
    // 侧栏放类别
    public CategorySidePanel CategoryPanel;
    
    // 主栏上放选项
    public ConfigOptionsPanel OptionsPanel;
    
    // 主栏下放描述
    public TooltipPanel TooltipPanel;

    public ModernConfigUI()
    {
        int gapBetweenPanels = 20;
        int sidePanelWidth = 200;
        int tooltipPanelHeight = 146;
        Instance = this;
        
        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        MainPanel.SetPadding(gapBetweenPanels);
        MainPanel.SetPosPixels(0f, -10f);
        MainPanel.SetSizePercent(0.76f, 0.76f)
            .JoinParent(this);
        
        // 侧栏放类别
        CategoryPanel = new CategorySidePanel
        {
            RelativeMode = RelativeMode.Horizontal
        };
        CategoryPanel.SetSize(sidePanelWidth, 0f, 0f, 1f);
        CategoryPanel.JoinParent(MainPanel);
        
        // 把主栏框起来的容器
        var mainPanelContainer = new View
        {
            Spacing = new Vector2(gapBetweenPanels),
            RelativeMode = RelativeMode.Horizontal,
            BorderColor = Color.White
        };
        mainPanelContainer.SetSize(-sidePanelWidth - gapBetweenPanels, 0f, 1f, 1f);
        mainPanelContainer.JoinParent(MainPanel);
        
        // 主栏上放选项
        OptionsPanel = new ConfigOptionsPanel
        {
            Spacing = new Vector2(gapBetweenPanels),
            RelativeMode = RelativeMode.Vertical
        };
        OptionsPanel.SetSize(0f, -tooltipPanelHeight - gapBetweenPanels, 1f, 1f);
        OptionsPanel.JoinParent(mainPanelContainer);
        
        // 主栏下放描述
        TooltipPanel = new TooltipPanel
        {
            Spacing = new Vector2(gapBetweenPanels),
            RelativeMode = RelativeMode.Vertical
        };
        TooltipPanel.SetSize(0f, tooltipPanelHeight, 1f, 0f);
        TooltipPanel.JoinParent(mainPanelContainer);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CenteredItemTagHandler.ModernConfigDrawing = true;
        base.Draw(spriteBatch);
        CenteredItemTagHandler.ModernConfigDrawing = false;
    }

    public void Open()
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        Enabled = true;
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Enabled = false;
    }
}