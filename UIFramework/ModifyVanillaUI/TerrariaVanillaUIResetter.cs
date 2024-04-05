using ImproveGame.Common.Configs;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.Graphics2D;
using ImproveGame.UIFramework.UIStructs;
using ReLogic.Graphics;
using System.Reflection;
using System.Text;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.States;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.ModifyVanillaUI;

// Who What Do
public class TerrariaVanillaUIResetter : ModSystem
{
    private MethodInfo _uiTextPaenl_DrawText;
    private MethodInfo _uiWorkshopHub_MakeFancyButtonInner;

    public override void Load()
    {
        // UIElement
        // UIBestiaryTest
        // UIBestiaryEntryInfoPage
        if (Main.dedServ || !UIConfigs.Instance.ResetNativeUI)
        {
            return;
        }

        // 修改原版 UI 裁切算法
        On_UIElement.GetClippingRectangle += (_, self, batch) => ClippingRectangleFix.FixedGet(self, batch);

        // 让所有 UI 都立刻绘制
        On_UIElement.OnInitialize += (orig, self) =>
        {
            orig.Invoke(self);
            self.UseImmediateMode = true;
        };

        // 中键显示 UIElement 信息
        /*On.Terraria.UI.UIElement.MiddleMouseDown += (orig, self, evt) =>
        {
            orig.Invoke(self, evt);
            PrintParent(self, evt.Target);
        };*/

        // 生物图鉴，生物介绍。设置 PaddingTop = 0
        // On_FlavorTextBestiaryInfoElement.ProvideUIElement += FlavorTextBestiaryInfoElement_ProvideUIElement;

        // 获取方法
        _uiTextPaenl_DrawText = typeof(UITextPanel<string>).GetMethod("DrawText", BindingFlags.Instance | BindingFlags.NonPublic);
        _uiWorkshopHub_MakeFancyButtonInner = typeof(UIWorkshopHub).GetMethod("MakeFancyButtonInner", BindingFlags.Instance | BindingFlags.NonPublic);

        // 原版 WorkShop Hub 提示框
        On_UIWorkshopHub.AddDescriptionPanel += UIWorkshopHub_AddDescriptionPanel;

        // 原版 WorkShop Hub 菜单按钮
        if (_uiWorkshopHub_MakeFancyButtonInner != null)
        {
            MonoModHooks.Add(_uiWorkshopHub_MakeFancyButtonInner, MakeFancyButtonInner);
        }

        // 原版 UISlicedImage
        On_UISlicedImage.DrawSelf += UISlicedImage_DrawSelf;

        // 原版 Text
        On_UIText.DrawSelf += UIText_DrawSelf;
        // 原版 TextPanel
        if (_uiTextPaenl_DrawText != null)
        {
            MonoModHooks.Add(_uiTextPaenl_DrawText, UITextPanel_DrawSelf);
        }
        // 原版 Panel
        On_UIPanel.DrawSelf += UIPanel_DrawSelf;
        // 原版 ScrollBar
        On_UIScrollbar.DrawSelf += UIScrollbar_DrawSelf;
        // 替换游戏内原来的 Utils.DrawInvBG
        On_Utils.DrawInvBG_SpriteBatch_int_int_int_int_Color += Utils_DrawInvBG;
    }

    private UIElement FlavorTextBestiaryInfoElement_ProvideUIElement(On_FlavorTextBestiaryInfoElement.orig_ProvideUIElement orig, FlavorTextBestiaryInfoElement self, BestiaryUICollectionInfo info)
    {
        UIElement uie = orig.Invoke(self, info);
        if (uie != null) uie.PaddingTop = 0f;
        return uie;
    }

    private static UIElement MakeFancyButtonInner(Func<UIWorkshopHub, Asset<Texture2D>, string, UIElement> orig, UIWorkshopHub self, Asset<Texture2D> iconImage, string textKey)
    {
        UIPanel uIPanel = new UIPanel
        {
            Width = new StyleDimension(-3, 0.5f),
            Height = new StyleDimension(-3, 0.33f)
        };
        uIPanel.OnMouseOver += self.SetColorsToHovered;
        uIPanel.OnMouseOut += self.SetColorsToNotHovered;
        uIPanel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
        uIPanel.BorderColor = new Color(89, 116, 213) * 0.7f;
        uIPanel.SetPadding(15f);

        var uIImage = new UIImage(iconImage)
        {
            IgnoresMouseInteraction = true,
            VAlign = 0.5f,
        };
        uIPanel.Append(uIImage);
        uIPanel.OnMouseOver += self.ShowOptionDescription;
        uIPanel.OnMouseOut += self.ClearOptionDescription;

        var title = new UIText(Language.GetText(textKey), 0.5f, true)
        {
            Width = new StyleDimension(-80f, 1f),
            Height = new StyleDimension(0f, 1f),
            Left = new StyleDimension(80f, 0f),
            PaddingRight = 40f,
            IgnoresMouseInteraction = true,
            TextOriginX = 0.5f,
            TextOriginY = 0f,
            IsWrapped = true
        };
        uIPanel.Append(title);
        uIPanel.SetSnapPoint("Button", 0);
        return uIPanel;
    }

    private static Texture2D uiSlicedImage = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight").Value;

    private void UISlicedImage_DrawSelf(On_UISlicedImage.orig_DrawSelf orig, UISlicedImage self, SpriteBatch spriteBatch)
    {
        if (self._texture.Value == uiSlicedImage)
        {
            Vector2 pos = self.GetDimensions().Position();
            Vector2 size = self.GetDimensions().Size();

            Vector3 colorX = new Vector3(156, 164, 229);
            Color color = new Color();

            color.R = (byte)Math.Round(self._color.R * colorX.X / 255f);
            color.G = (byte)Math.Round(self._color.G * colorX.Y / 255f);
            color.B = (byte)Math.Round(self._color.B * colorX.Z / 255f);
            color.A = self._color.A;

            SDFRectangle.NoBorder(pos, size, new Vector4(6f), color);
            return;
        }

        orig.Invoke(self, spriteBatch);
    }

    private void UIWorkshopHub_AddDescriptionPanel(On_UIWorkshopHub.orig_AddDescriptionPanel orig, UIWorkshopHub self, UIElement container, float accumulatedHeight, float height, string tagGroup)
    {
        UISlicedImage uISlicedImage = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight"))
        {
            HAlign = 0.5f,
            VAlign = 1f,
            Width = new StyleDimension(0f, 1f),
            Height = new StyleDimension(height, 0f),
            Top = new StyleDimension(2f, 0f)
        };
        uISlicedImage.SetSliceDepths(10);
        uISlicedImage.Color = Color.LightGray * 0.7f;
        container.Append(uISlicedImage);
        var text = new UIText(Language.GetText("Workshop.HubDescriptionDefault"))
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            Width = new StyleDimension(0f, 1f),
            Height = new StyleDimension(0f, 1f),
            PaddingLeft = 20f,
            PaddingRight = 20f,
            IsWrapped = true
        };
        uISlicedImage.Append(text);
        self._descriptionText = text;
    }

    private static void UITextPanel_DrawSelf(Action<UITextPanel<string>, SpriteBatch> orig, UITextPanel<string> self, SpriteBatch sb)
    {
        Vector2 innerPos = self.GetInnerDimensions().Position();
        Vector2 innerSize = self.GetInnerDimensions().Size();

        string text = self.Text;
        if (self.HideContents)
        {
            if (self._asterisks == null || self._asterisks.Length != text.Length)
            {
                self._asterisks = new string('*', text.Length);
            }

            text = self._asterisks;
        }

        Vector2 textSize = GetChatFontSize(text, self._textScale, self._isLarge);
        float textOffsetX = (innerSize.X - textSize.X) * self.TextHAlign;
        float textOffsetY = (innerSize.Y - textSize.Y) / 2f;
        Vector2 textPos = innerPos + new Vector2(textOffsetX, textOffsetY);

        if (self._isLarge)
        {
            textPos.Y += UIConfigs.Instance.BigFontOffsetY * self._textScale;
            TrUtils.DrawBorderStringBig(sb, text, textPos, self._color, self._textScale);
        }
        else
        {
            textPos.Y += UIConfigs.Instance.GeneralFontOffsetY * self._textScale;
            TrUtils.DrawBorderString(sb, text, textPos, self._color, self._textScale);
        }
    }

    private void UIText_DrawSelf(On_UIText.orig_DrawSelf orig, UIText self, SpriteBatch sb)
    {
        Vector2 pos = self.GetDimensions().Position();
        Vector2 size = self.GetDimensions().Size();

        //SDFRectangle.HasBorder(pos, size, new(10), Color.Transparent, 2f, Color.White);

        self.VerifyTextState();
        Vector2 innerSize = self.GetInnerDimensions().Size();

        Vector2 textPos = self.GetInnerDimensions().Position();

        textPos += (innerSize - self._textSize) * new Vector2(self.TextOriginX, self.TextOriginY);

        if (self.TextOriginY > 0f)
            textPos.Y += self._isLarge ? UIConfigs.Instance.BigFontOffsetY :
                UIConfigs.Instance.GeneralFontOffsetY;
        else
            textPos.Y -= self._isLarge ? 10 : 2;

        float textScale = self._textScale;
        if (self.DynamicallyScaleDownToWidth && self._textSize.X > innerSize.X)
        {
            textScale *= innerSize.X / self._textSize.X;
        }

        DynamicSpriteFont font = (self._isLarge ? FontAssets.DeathText : FontAssets.MouseText).Value;

        Vector2 vector = font.MeasureString(self._visibleText);
        Color baseColor = self._shadowColor * (self._color.A / 255f);
        Vector2 origin = new Vector2(0f, 0f) * vector;
        Vector2 baseScale = new Vector2(textScale);

        TextSnippet[] snippets = ChatManager.ParseMessage(self._visibleText, self._color).ToArray();

        ChatManager.ConvertNormalSnippets(snippets);
        ChatManager.DrawColorCodedStringShadow(sb, font, snippets, textPos, baseColor, 0f, origin, baseScale, -1f, 1.5f);
        ChatManager.DrawColorCodedString(sb, font, snippets, textPos, Color.White, 0f, origin, baseScale, out var _, -1f);
    }
    private void Utils_DrawInvBG(
        On_Utils.orig_DrawInvBG_SpriteBatch_int_int_int_int_Color orig, SpriteBatch sb, int x, int y,
        int w, int h, Color color)
    {
        if (color == default)
            color = new Color(63, 65, 151, 255) * 0.785f;

        if (w < 20)
            w = 20;

        if (h < 20)
            h = 20;

        Color borderColor = color;
        borderColor.R = (byte)Math.Round(borderColor.R * 19d / 255);
        borderColor.G = (byte)Math.Round(borderColor.G * 30d / 255);
        borderColor.B = (byte)Math.Round(borderColor.B * 39d / 255);
        SDFRectangle.HasBorder(new Vector2(x, y), new Vector2(w, h), new Vector4(10f), color, 2, borderColor);
    }

    private static CalculatedStyle GetHandleCalculatedStyle(UIScrollbar scrollbar)
    {
        CalculatedStyle innerDimensions = scrollbar.GetInnerDimensions();
        if (scrollbar._maxViewSize == 0f && scrollbar._viewSize == 0f)
        {
            scrollbar._viewSize = 1f;
            scrollbar._maxViewSize = 1f;
        }
        return new CalculatedStyle(
            innerDimensions.X, innerDimensions.Y + innerDimensions.Height * (scrollbar._viewPosition / scrollbar._maxViewSize) - 3,
            20, innerDimensions.Height * (scrollbar._viewSize / scrollbar._maxViewSize) + 7);
    }

    // 原版滚动条绘制，在 DrawBar 返回空阻止原版滚动条绘制。然后通过反射获取滚动条位置进行绘制。
    private void UIScrollbar_DrawSelf(On_UIScrollbar.orig_DrawSelf orig,
        UIScrollbar self, SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = self.GetDimensions();
        CalculatedStyle innerDimensions = self.GetInnerDimensions();
        Vector2 pos = dimensions.Position();
        Vector2 size = dimensions.Size();
        if (self._isDragging)
        {
            float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - self._dragYOffset;
            self._viewPosition = MathHelper.Clamp(num / innerDimensions.Height * self._maxViewSize, 0f, self._maxViewSize - self._viewSize);
        }
        Rectangle handleRectangle = self.GetHandleRectangle();
        Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
        bool isHoveringOverHandle = self._isHoveringOverHandle;
        self._isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));
        if (!isHoveringOverHandle && self._isHoveringOverHandle && Main.hasFocus)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        float bgRounded = MathF.Min(size.X, size.Y) / 2;
        SDFRectangle.HasBorder(pos, size, new Vector4(bgRounded), UIStyle.ScrollBarBg, 2, UIStyle.PanelBorder);
        CalculatedStyle barDimensions = GetHandleCalculatedStyle(self);
        Vector2 barPos = barDimensions.Position() + new Vector2(5, 3);
        Vector2 barSize = barDimensions.Size() - new Vector2(10, 7);
        float barRounded = MathF.Min(barSize.X, barSize.Y) / 2;
        SDFRectangle.NoBorder(barPos, barSize, new Vector4(barRounded), new(220, 220, 220));
    }

    private void UIPanel_DrawSelf(On_UIPanel.orig_DrawSelf orig, UIPanel self,
        SpriteBatch spriteBatch)
    {
        Vector2 pos = self.GetDimensions().Position();
        Vector2 size = self.GetDimensions().Size();
        SDFRectangle.HasBorder(pos, size, new Vector4(10f), self.BackgroundColor, 2, self.BorderColor);
    }

    private static void PrintParent(UIElement self, UIElement target)
    {
        var builder = new StringBuilder();
        if (self == target)
        {
            UIElement element = self;
            while (element != null)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" - ");
                }
                builder.Append(element.GetType().Name);
                element = element.Parent;
            }
            Main.NewText(builder.ToString());
            Main.NewText(self.Parent?.PaddingTop ?? -1);
            Main.NewText(self.Top.Precent);
            Main.NewText(self.Height.Precent);
            Main.NewText(self.Height.Pixels);
        }

    }
}
