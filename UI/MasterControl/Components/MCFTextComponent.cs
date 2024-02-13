using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.MasterControl.Components;

public class MCFTextComponent : GenericMCFComponent
{
    public SUIText SUIText { get; init; }

    public MCFTextComponent(IReadOnlyList<MasterControlFunction> functions, int index) : base(functions, index)
    {
        SUIText = new SUIText
        {
            Width = { Percent = 1f },
            Height = { Percent = 1f },
            TextAlign = new Vector2(0.5f),
        };
        SUIText.JoinParent(this);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (MasterControlFunction != null)
        {
            SUIText.TextOrKey = MasterControlFunction.GetDisplayName();
        }
    }
}
