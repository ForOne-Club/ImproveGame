using ReLogic.Graphics;
using System.Text;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config.UI;
using Terraria.UI.Chat;

namespace ImproveGame.Common.Configs.Elements;

public class UncontrollablesElement : ConfigElement
{
    private string _content;

    public override void OnBind()
    {
        base.OnBind();
        DrawLabel = false;

        Append(new UIText(Label, 0.6f, true)
        {
            Top = {Pixels = 8f},
            Width = {Percent = 1f},
            Height = {Pixels = 40f},
            TextOriginX = 0.5f,
            TextOriginY = 0.5f
        });

        Append(new UIText(Language.GetTextValue("Mods.ImproveGame.Configs.ImproveConfigs.OtherFunctions.Subtitle"),
            0.7f)
        {
            Top = {Pixels = 40f},
            Width = {Percent = 1f},
            Height = {Pixels = 40f},
            TextOriginX = 0.5f,
            TextOriginY = 0.5f
        });

        // var uiText = new UIText(TooltipFunction(), 0.9f) {
        //     Top = {Pixels = 82f},
        //     Left = {Pixels = 10f},
        //     Width = {Pixels = -20f, Percent = 1f},
        //     IsWrapped = true,
        // };
        // uiText.OnInternalTextChange += () => {
        //     float newHeight = uiText.MinHeight.Pixels + 80f;
        //     Height.Set(newHeight, 0f);
        //
        //     if (Parent is UISortableElement) {
        //         Parent.Height.Pixels = newHeight;
        //     }
        // };
        // Append(uiText);

        _content = TooltipFunction();
        TooltipFunction = null;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var dimensions = GetInnerDimensions();
        var font = FontAssets.MouseText.Value;
        const float textScale = 0.9f;
        float width = dimensions.Width - 50f;
        var entries = _content.Split('\n');
        float currentY = 82f + dimensions.Y;

        for (var i = 0; i < entries.Length; i++)
        {
            string entry = entries[i];
            string visibleText = font.CreateWrappedText(entry, width / textScale);
            float x = 40f + dimensions.X;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, visibleText, new Vector2(x, currentY),
                Color.White, 0f, Vector2.Zero, new Vector2(textScale), spread: 1.2f);

            int entryNumber = i + 1;
            string entryIdentifier = entryNumber + ". ";
            float numberWidth = font.MeasureString(entryIdentifier).X * textScale;
            x -= numberWidth;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, entryIdentifier, new Vector2(x, currentY),
                Color.White, 0f, Vector2.Zero, new Vector2(textScale), spread: 1.2f);

            currentY += font.LineSpacing * (visibleText.Count(c => c == '\n') + 1) * textScale;
        }

        float height = currentY - dimensions.Y + 10f;
        Height.Set(height, 0f);
        if (Parent is UISortableElement) {
            Parent.Height.Pixels = height;
        }
    }
}