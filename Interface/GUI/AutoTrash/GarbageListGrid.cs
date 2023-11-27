using ImproveGame.Common.Players;
using ImproveGame.Interface.UIElements;

namespace ImproveGame.Interface.GUI.AutoTrash;

public class GarbageListGrid : ScrollView
{
    public static List<Item> Garbages => AutoTrashPlayer.Instance.AutoDiscardItems;

    public GarbageListGrid()
    {
        ListView.SetInnerPixels(GridSize(44f, 4f, 4));

        Scrollbar.HAlign = 1f;
        Scrollbar.Left.Pixels = -1;
        Scrollbar.Height.Percent = 1f;
        Scrollbar.Width.Pixels = 16f;
        Scrollbar.SetPadding(4);
    }

    public override void Update(GameTime gameTime)
    {
        if (Garbages.Count != ListView.Children.Count())
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

        for (int i = 0; i < Garbages.Count; i++)
        {
            GarbageListSlot itemSlot = new GarbageListSlot(AutoTrashPlayer.Instance.AutoDiscardItems, i);
            itemSlot.Wrap = true;
            itemSlot.Spacing = new Vector2(4f);
            itemSlot.Relative = RelativeMode.Horizontal;
            itemSlot.Join(ListView);
        }

        int length = Math.Max(16, Garbages.Count);
        ListView.SetInnerPixels(GridSize(44f, 4f, 4, (int)MathF.Ceiling(length / 4f)));
        Scrollbar.SetView(GetInnerDimensions().Height, ListView.Height.Pixels);
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
