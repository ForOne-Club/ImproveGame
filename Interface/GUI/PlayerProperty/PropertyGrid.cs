using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI.PlayerProperty;

public class PropertyGrid : ScrollView
{
    public PropertyGrid()
    {
        ListView.Width.Pixels = 160f;

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
        float maxBottom = 0f;

        foreach (var item in ListView.Children)
        {
            if (item.Bottom() > maxBottom)
            {
                maxBottom = item.Bottom();
            }
        }

        if (ListView.Height.Pixels != maxBottom)
        {
            ListView.Height.Pixels = maxBottom;
            Recalculate();
        }

        Scrollbar.SetView(GetInnerDimensions().Height, ListView.Height.Pixels);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Math.Abs(-Scrollbar.ViewPosition - ListView.Top.Pixels) > 0.000000001f)
        {
            ListView.Top.Pixels = -Scrollbar.ViewPosition;
            ListView.Recalculate();
        }

        base.DrawSelf(spriteBatch);
    }
}
