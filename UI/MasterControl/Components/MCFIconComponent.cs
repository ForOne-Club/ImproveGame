using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.MasterControl.Components;

public class MCFIconComponent : GenericMCFComponent
{
    public SUIImage SUIImage { get; init; }

    public MCFIconComponent(IReadOnlyList<MasterControlFunction> functions, int index) : base(functions, index)
    {
        SUIImage = new SUIImage(MasterControlFunction?.Icon, false);
        SUIImage.SetSizePercent(1f, 1f);
        SUIImage.ImageAlign = new Vector2(0.5f);
        SUIImage.JoinParent(this);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        SUIImage.Texture = MasterControlFunction?.Icon;

        base.Draw(spriteBatch);
    }
}
