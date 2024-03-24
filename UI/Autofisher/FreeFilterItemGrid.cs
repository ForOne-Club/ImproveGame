using ImproveGame.UI.AutoTrash;
using ImproveGame.UIFramework.BaseViews;

namespace ImproveGame.UI.Autofisher;

public class FreeFilterItemGrid : ScrollView
{
    public static List<Item> Items => CatchRecord.GetRecordedCatches;

    public FreeFilterItemGrid()
    {
        ListView.SetInnerPixels(new Vector2(GridSize(44f, 4f, 4)));

        Scrollbar.HAlign = 1f;
        Scrollbar.Left.Pixels = -1;
        Scrollbar.Height.Percent = 1f;
        Scrollbar.Width.Pixels = 16f;
        Scrollbar.SetPadding(4);
    }

    public override void Update(GameTime gameTime)
    {
        if (Items.Count != ListView.Children.Count())
        {
            ResetBottomGrid();
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// 顾名思义：重置下面那个表格
    /// </summary>
    public void ResetBottomGrid()
    {
        ListView.RemoveAllChildren();

        for (int i = 0; i < Items.Count; i++)
        {
            var itemSlot = new FreeFilterItemSlot(Items, i)
            {
                PreventOverflow = true,
                Spacing = new Vector2(4f),
                RelativeMode = RelativeMode.Horizontal
            };
            itemSlot.JoinParent(ListView);
        }

        int length = Math.Max(16, Items.Count);
        ListView.SetInnerPixels(GridSize(44f, 4f, 4, (int)MathF.Ceiling(length / 4f)));
        Scrollbar.SetView(GetInnerDimensions().Height, ListView.Height.Pixels);
        Recalculate();
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (Math.Abs(-Scrollbar.BarTop - ListView.Top.Pixels) > 0.000000001f)
        {
            ListView.Top.Pixels = -Scrollbar.BarTop;
            ListView.Recalculate();
        }

        base.DrawSelf(spriteBatch);
    }
}
