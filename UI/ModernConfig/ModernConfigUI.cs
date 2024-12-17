using ImproveGame.Common;
using ImproveGame.Common.Configs;
using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.SUIElements;
using Microsoft.Xna.Framework.Input;
using Terraria.Graphics.Renderers;
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

    //当前打开的mod
    public Mod currentMod;

    // 有点爽的东西，中键收藏生成粒子
    private UIParticleLayer _particleSystem = new()
    {
        Width = new StyleDimension(0f, 1f),
        Height = new StyleDimension(0f, 1f),
        AnchorPositionOffsetByPercents = Vector2.One / 2f,
        AnchorPositionOffsetByPixels = Vector2.Zero
    };

    public override void OnInitialize()
    {
        const int gapBetweenPanels = 20;
        const int sidePanelWidth = 220;
        const int tooltipPanelHeight = 146;
        Instance = this;

        // 主面板
        MainPanel = new SUIPanel(ConfigColors.MainPanelBorder, ConfigColors.MainPanelBg)
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
        CategoryPanel = new CategorySidePanel(ConfigColors.DarkBorderlessPanel)
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
        OptionsPanel = new ConfigOptionsPanel(ConfigColors.DarkBorderlessPanel)
        {
            Spacing = new Vector2(gapBetweenPanels),
            RelativeMode = RelativeMode.Vertical
        };
        OptionsPanel.SetSize(0f, -tooltipPanelHeight - gapBetweenPanels, 1f, 1f);
        OptionsPanel.JoinParent(mainPanelContainer);

        // 主栏下放描述
        TooltipPanel = new TooltipPanel(ConfigColors.DarkBorderlessPanel)
        {
            Spacing = new Vector2(gapBetweenPanels),
            RelativeMode = RelativeMode.Vertical
        };
        TooltipPanel.SetSize(0f, tooltipPanelHeight, 1f, 0f);
        TooltipPanel.JoinParent(mainPanelContainer);

        MainPanel.Append(_particleSystem);

        // 返回按钮
        var backButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true)
        {
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
        // 修复鼠标移到标牌上会导致标牌文字一直显示的问题
        Main._MouseOversCanClear = true;

        if (Glass is not null && !Main.gameMenu && !DrawCalledForMakingGlass && GlassVfxEnabled)
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

    public void Open(Mod mod)
    {
        SoundEngine.PlaySound(SoundID.MenuOpen);

        if (mod.Name == "ImproveGame")
            ConfigOptionsPanel.CategoryToSelectOnOpen = CategorySidePanel.Cards["AboutPage"].Category;
        else if (CategorySidePanel.ModedAboutPage.TryGetValue(mod, out var value))
            ConfigOptionsPanel.CategoryToSelectOnOpen = value;
        else
            ConfigOptionsPanel.CategoryToSelectOnOpen = CategorySidePanel.AboutPage_ModConfig;
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
        currentMod = mod;
        CategoryPanel.ChangeMod(mod,true);
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
            {
                Interface.modConfigList.ModToSelectOnOpen = currentMod ?? ImproveGame.Instance;
                Main.InGameUI.SetState(Interface.modConfigList);


            }

        }
        else
        {
            Main.menuMode = Interface.modConfigListID;
            Interface.modConfigList.ModToSelectOnOpen = currentMod ?? ImproveGame.Instance;
        }

        OpenFromMasterControl = false;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape) &&
            UISystem.FocusedEditableText is null && Main.gameMenu) // 游戏里按照物品栏快捷键关闭，只有gameMenu才用Esc
        {
            Close();
        }

        // 适配即时风格切换
        MainPanel.BorderColor = ConfigColors.MainPanelBorder;
        MainPanel.BgColor = ConfigColors.MainPanelBg;
    }

    public void GenerateParticleAtMouse()
    {
        var parentPosition = Instance.MainPanel.GetInnerDimensions().Center();
        var mousePosition = Main.MouseScreen;
        var finalPosition = mousePosition - parentPosition;
        finalPosition.Y += 4f;
        Instance.GenerateParticleAt(finalPosition);
    }

    public void GenerateParticleAt(Vector2 position)
    {
        Vector2 accelerationPerFrame = new(0f, 0.16350001f);
        var texture = Main.Assets.Request<Texture2D>("Images/UI/Creative/Research_Spark");

        for (int i = 0; i < 12; i++)
        {
            Vector2 initialVelocity = Main.rand.NextVector2Circular(4f, 3f);

            initialVelocity.Y -= 2f;

            _particleSystem.AddParticle(new CreativeSacrificeParticle(texture, null, initialVelocity, position)
            {
                AccelerationPerFrame = accelerationPerFrame,
                ScaleOffsetPerFrame = -1f / 60f,
                _scale = Main.rand.NextFloat(0.2f, 0.5f)
            });
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