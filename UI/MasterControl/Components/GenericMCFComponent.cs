using ImproveGame.UIFramework.BaseViews;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.MasterControl.Components;

public class GenericMCFComponent : TimerView
{
    public readonly IReadOnlyList<MasterControlFunction> MCFunctions;
    public readonly int Index;
    public MasterControlFunction MasterControlFunction =>
        MCFunctions.Count > 0 && Index < MCFunctions.Count ? MCFunctions[Index] : null;

    public GenericMCFComponent(IReadOnlyList<MasterControlFunction> functions, int index)
    {
        MCFunctions = functions;
        Index = index;

        Border = 2f;
        Rounded = new Vector4(12f);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        MasterControlFunction?.MouseDown(this);
        base.LeftMouseDown(evt);
    }

    public override void RightMouseDown(UIMouseEvent evt)
    {
        if (MasterControlFunction != null)
        {
            MasterControlFunction.Favorite = !MasterControlFunction.Favorite;
            MasterControlManager.Instance.OrderMCFunctions();
        }

        base.RightMouseDown(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (MasterControlFunction != null)
        {
            MasterControlFunction?.Update(this);

            BgColor = MasterControlFunction.GetBgColor(this);
            BorderColor = MasterControlFunction.GetBorderColor(this);
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (MasterControlFunction != null && IsMouseHovering)
        {
            string text =
                $"{MasterControlFunction.GetDisplayName()}\n" +
                $"{MasterControlFunction.GetDescription()}";
            UICommon.TooltipMouseText(text);
        }
    }
}
