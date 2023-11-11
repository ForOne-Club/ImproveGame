using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

public class PropertyGrid : ScrollView
{
    public ListView ListView2;

    public PropertyGrid()
    {
        ListView.Width.Pixels = 160f;

        ListView2 = new ListView();
        ListView2.Left.Pixels = 164f;
        ListView2.Width.Pixels = 160f;
        ListView2.Join(this);

        Scrollbar.HAlign = 1f;
        Scrollbar.Left.Pixels = -1;

        Scrollbar.Height.Percent = 1f;
        Scrollbar.Width.Pixels = 16f;

        Scrollbar.SetPadding(4);
    }

    public override void Update(GameTime gameTime)
    {
        ResetGrid();

        base.Update(gameTime);
    }

    /// <summary>
    /// 顾名思义：重置下面那个表格
    /// </summary>
    public void ResetGrid()
    {
        float maxBottom = Math.Max(
            ListView.Children.Count() > 0 ? ListView.Children.Last().Bottom() : 0f,
            ListView2.Children.Count() > 0 ? ListView2.Children.Last().Bottom() : 0f);

        if (ListView.Height.Pixels != maxBottom || ListView2.Height.Pixels != maxBottom)
        {
            ListView.Height.Pixels = maxBottom;
            ListView2.Height.Pixels = maxBottom;
            Recalculate();
        }

        Scrollbar.SetView(GetInnerDimensions().Height, maxBottom);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Math.Abs(-Scrollbar.ViewPosition - ListView.Top.Pixels) > 0.000000001f)
        {
            ListView.Top.Pixels = -Scrollbar.ViewPosition;
            ListView.Recalculate();

            ListView2.Top.Pixels = -Scrollbar.ViewPosition;
            ListView2.Recalculate();
        }

        base.DrawSelf(spriteBatch);
    }
}
