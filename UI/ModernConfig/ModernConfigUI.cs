using ImproveGame.Common;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.ModernConfig;

public sealed class ModernConfigUI : UIState
{
    public static ModernConfigUI Instance { get; set; }

    public bool Enabled { get; set; }

    public bool OpenFromMasterControl;

    public static bool DrawCalledForMakingGlass;

    public static RenderTarget2D Glass; // 云母效果用

    // 主面板
    public SUIPanel MainPanel;

    // 侧栏放类别
    public CategorySidePanel CategoryPanel;

    // 主栏上放选项
    public ConfigOptionsPanel OptionsPanel;

    // 主栏下放描述
    public TooltipPanel TooltipPanel;

    public override void OnInitialize()
    {
        const int gapBetweenPanels = 20;
        const int sidePanelWidth = 220;
        const int tooltipPanelHeight = 146;
        Instance = this;

        // 主面板
        MainPanel = new SUIPanel(UIStyle.PanelBorder, UIStyle.PanelBg)
        {
            Shaded = true,
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        MainPanel.SetPadding(gapBetweenPanels);
        MainPanel.SetPosPixels(0f, -20f);
        MainPanel.SetSizePercent(0.86f, 0.82f)
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
        
        // 返回按钮
        var backButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true) {
            Width = { Pixels = -30f, Percent = 0.5f },
            Height = { Pixels = 50f },
            Top = { Pixels = -35f },
            VAlign = 1f,
            HAlign = 0.5f,
        }.WithFadedMouseOver();
        backButton.OnLeftClick += (_, _) => Close();

        this.Append(backButton);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Glass is not null && !Main.gameMenu && !DrawCalledForMakingGlass)
        {
            // 云母效果特殊处理
            Main.spriteBatch.ReBegin(null, Matrix.Identity);
            Main.spriteBatch.Draw(Glass, Vector2.Zero, Color.White);
            Main.spriteBatch.ReBegin(null, Main.UIScaleMatrix);
        }

        CenteredItemTagHandler.ModernConfigDrawing = true;
        base.Draw(spriteBatch);
        CenteredItemTagHandler.ModernConfigDrawing = false;
    }

    public void Open()
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);
        ConfigOptionsPanel.CategoryToSelectOnOpen = CategorySidePanel.Cards["AboutPage"].Category;
        Enabled = true;

        if (Main.gameMenu)
        {
            Main.menuMode = 888;
            Main.MenuUI.SetState(this);
        }
        else
        {
            IngameFancyUI.OpenUIState(this);
        }
    }

    public void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Enabled = false;

        if (!Main.gameMenu)
        {
            if (OpenFromMasterControl)
                IngameFancyUI.Close();
            else
                Main.InGameUI.SetState(Interface.modConfigList);
        }
        else
        {
            Main.menuMode = Interface.modConfigListID;
            Interface.modConfigList.ModToSelectOnOpen = ImproveGame.Instance;
        }

        OpenFromMasterControl = false;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape) &&
            UISystem.FocusedEditableText is null)
        {
            Close();
        }
    }

    #region Mica - 云母效果特殊处理

    public static void MakeGlass(RenderTarget2D blurredTarget, RenderTarget2D uiTarget)
    {
        if (Instance?.Enabled is not true)
            return;
        
        var shader = ModAsset.Mask.Value;
        var device = Main.instance.GraphicsDevice;
        var batch = Main.spriteBatch;

        var glass = Glass;

        device.SetRenderTarget(uiTarget);
        device.Clear(Color.Transparent);
        batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
        SDFRectangle.DontDrawShadow = true;
        DrawCalledForMakingGlass = true;
        Instance?.Draw(batch);
        DrawCalledForMakingGlass = false;
        SDFRectangle.DontDrawShadow = false;
        batch.End();

        device.SetRenderTarget(glass);
        device.Clear(Color.Transparent);
        batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        shader.CurrentTechnique.Passes["Mask"].Apply();
        device.Textures[1] = blurredTarget;
        device.Textures[2] = uiTarget;
        // 颜色是 Transparent，所以背景图是完全透明
        batch.Draw(uiTarget, Vector2.Zero, Color.White);
        batch.End();

        // 复原
        device.Textures[0] = null;
        device.Textures[1] = null;
        device.Textures[2] = null;
    }

    #endregion
}