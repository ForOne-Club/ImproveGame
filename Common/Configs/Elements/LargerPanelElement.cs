using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ImproveGame.Common.Configs.Elements;

public class LargerPanelElement : ConfigElement
{
    public override void OnBind() {
        base.OnBind();
        Height.Set(36f, 0f);
        DrawLabel = false;
        
        Append(new UIText(Label, 0.4f, true) {
            TextOriginX = 0.5f,
            TextOriginY = 0.5f,
            Width = StyleDimension.Fill,
            Height = StyleDimension.Fill
        });
    }
    
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dimensions = GetDimensions();
        float num = dimensions.Width + 1f;
        var pos = new Vector2(dimensions.X, dimensions.Y);
        var color = IsMouseHovering ? UICommon.DefaultUIBlue : UICommon.DefaultUIBlue.MultiplyRGBA(new Color(180, 180, 180));
        DrawPanel2(spriteBatch, pos, TextureAssets.SettingsPanel.Value, num, dimensions.Height, color);

        base.DrawSelf(spriteBatch);
    }
}