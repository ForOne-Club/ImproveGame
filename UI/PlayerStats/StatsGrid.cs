using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.PlayerStats;

public class StatsGrid : ScrollView
{
    public readonly TimerView ListView2 = new TimerView();

    public StatsGrid()
    {
        ListView.Width.Pixels = 160f;

        ListView2.IsAdaptiveHeight = true;
        ListView2.Width.Pixels = 160f;
        ListView2.Left.Pixels = 164f;
        ListView2.JoinParent(this);

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
        float maxBottom1 = ListView.Children.Any() ? ListView.Children.Last().Bottom() : 0f;
        float maxBottom2 = ListView2.Children.Any() ? ListView2.Children.Last().Bottom() : 0f;

        if (ListView.Height.Pixels != maxBottom1 || ListView2.Height.Pixels != maxBottom2)
        {
            ListView.Height.Pixels = maxBottom1;
            ListView2.Height.Pixels = maxBottom2;
            Recalculate();
        }

        Scrollbar.SetView(GetInnerDimensions().Height, Math.Max(maxBottom1, maxBottom2));
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Math.Abs(-Scrollbar.BarTop - ListView.Top.Pixels) > 0.000000001f)
        {
            ListView.Top.Pixels = -Scrollbar.BarTop;
            ListView.Recalculate();

            ListView2.Top.Pixels = -Scrollbar.BarTop;
            ListView2.Recalculate();
        }

        base.DrawSelf(spriteBatch);
    }
}
