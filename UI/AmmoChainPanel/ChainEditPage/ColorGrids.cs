using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.AmmoChainPanel.ChainEditPage;

public class BrownColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (138, 111, 48);

    protected override Asset<Texture2D> GetTexture() => ModAsset.BrownColorGrid;
}

public class RedColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (172, 50, 50);

    protected override Asset<Texture2D> GetTexture() => ModAsset.RedColorGrid;
}

public class PinkColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (215, 123, 186);

    protected override Asset<Texture2D> GetTexture() => ModAsset.PinkColorGrid;
}

public class BlueColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (99, 155, 255);

    protected override Asset<Texture2D> GetTexture() => ModAsset.BlueColorGrid;
}

public class GreenColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (153, 229, 80);

    protected override Asset<Texture2D> GetTexture() => ModAsset.GreenColorGrid;
}

public class YellowColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (251, 242, 54);

    protected override Asset<Texture2D> GetTexture() => ModAsset.YellowColorGrid;
}

public class OrangeColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => new (223, 113, 38);

    protected override Asset<Texture2D> GetTexture() => ModAsset.OrangeColorGrid;
}

public class BlackColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => Color.Black;

    protected override Asset<Texture2D> GetTexture() => ModAsset.BlackColorGrid;
}

public class WhiteColorGrid (ChainEditPage parent) : BaseColorGrid(parent)
{
    protected override Color GetAssignedColor() => Color.White;

    protected override Asset<Texture2D> GetTexture() => ModAsset.WhiteColorGrid;
}

public abstract class BaseColorGrid : TimerView
{
    protected abstract Color GetAssignedColor();
    protected abstract Asset<Texture2D> GetTexture();
    private AnimationTimer _chooseTimer = new (3);
    public ChainEditPage Parent;

    public BaseColorGrid(ChainEditPage parent)
    {
        Parent = parent;
        SetSizePixels(26, 26);
        Spacing = new Vector2(6f);
        PreventOverflow = true;
        SetPadding(0, 0);

        BorderColor = Color.Transparent;
        BgColor = Color.Transparent;
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);

        if (Parent.EditingChain.Color != GetAssignedColor())
        {
            Parent.EditingChain.Color = GetAssignedColor();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        _chooseTimer.Update();
        if (Parent.EditingChain.Color != GetAssignedColor())
            _chooseTimer.Close();
        else
            _chooseTimer.Open();
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        var innerDimensions = GetInnerDimensions();
        var position = innerDimensions.Position();
        var color = HoverTimer.Lerp(Color.White, Color.White * 0.8f);
        color.A = 255;
        spriteBatch.Draw(GetTexture().Value, position, color);

        // 描边
        spriteBatch.Draw(ModAsset.ColorGridHighlight.Value, position, Color.White * _chooseTimer.Schedule);
    }
}