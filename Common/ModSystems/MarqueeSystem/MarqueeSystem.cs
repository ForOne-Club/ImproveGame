using ImproveGame.UIFramework.Graphics2D;

namespace ImproveGame.Common.ModSystems.MarqueeSystem;

public class MarqueeSystem : ModSystem
{
    private class MarqueeLayer : GameInterfaceLayer
    {
        public readonly MarqueeSystem MarqueeSystem;

        public MarqueeLayer(MarqueeSystem marqueeSystem) : base("ImproveGame: Marquee", InterfaceScaleType.Game)
        {
            MarqueeSystem = marqueeSystem;
        }

        public override bool DrawSelf()
        {
            BaseDraw();
            return true;
        }
    }

    private readonly MarqueeLayer _marqueeLayer;

    public MarqueeSystem()
    {
        _marqueeLayer = new MarqueeLayer(this);
    }

    public override void PreUpdatePlayers()
    {
        Player player = Main.LocalPlayer;
        Item item = player.HeldItem;

        if (item.ModItem is IMarqueeItem marqueeItem && marqueeItem.ShouldDraw)
        {
            marqueeItem.ShouldDraw = false;
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layers => layers.Name == "Vanilla: Ruler");

        if (index++ > -1)
        {
            layers.Insert(index, _marqueeLayer);
        }
    }

    public static void BaseDraw()
    {
        Player player = Main.LocalPlayer;
        Item item = player.HeldItem;

        if (item.ModItem is IMarqueeItem {ShouldDraw: true } marqueeItem)
        {
            Rectangle marquee = marqueeItem.Marquee;
            Vector2 position = new Vector2(marquee.X, marquee.Y);
            Vector2 size = marquee.Size();

            Color borderColor = marqueeItem.BorderColor;
            Color backgroundColor = marqueeItem.BackgroundColor;

            bool shouldDraw = true;

            marqueeItem.PreDrawMarquee(ref shouldDraw, marquee, backgroundColor, borderColor);

            if (shouldDraw)
            {
                SDFRectangle.HasBorder(
                    position - new Vector2(2) - Main.screenPosition,
                    size + new Vector2(4),
                    new Vector4(2),
                    backgroundColor, 2f, borderColor, false);
            }

            marqueeItem.PostDrawMarquee(marquee, backgroundColor, borderColor);
        }
    }

    /// <summary>
    /// 根据给定的坐标，为对应物块画上框
    /// </summary>
    /// <param name="tilePosList"></param>
    public static void DrawSelectedTiles(IEnumerable<Point> tilePosList, Color borderColor, Color backgroundColor = default)
    {
        var hashList = tilePosList.ToHashSet();
        var screenCenter = Main.Camera.Center.ToTileCoordinates();
        foreach (var pos in hashList)
        {
            Vector2 worldPosition = pos.ToWorldCoordinates(autoAddXY: Vector2.Zero);
            int i = pos.X;
            int j = pos.Y;

            if (Math.Abs(screenCenter.X - i) > 80 || Math.Abs(screenCenter.Y - j) > 60)
                continue;

            if (Main.LocalPlayer.gravDir is -1f)
            {
                worldPosition.Y = Main.screenPosition.Y + Main.screenHeight - (worldPosition.Y - Main.screenPosition.Y);
                worldPosition.Y -= 16f;
            }

            // 画背景
            if (backgroundColor != Color.Transparent && backgroundColor != default)
            {
                var blockDrawPosition = new Vector2(worldPosition.X, worldPosition.Y) - Main.screenPosition;
                var rect = new Rectangle?(new Rectangle(0, 0, 16, 16));
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, blockDrawPosition, rect, backgroundColor);
            }

            // 画边缘线，旁边不是被选物块时
            var left = new Point(i - 1, j);
            var right = new Point(i + 1, j);
            var up = new Point(i, j - 1);
            var bottom = new Point(i, j + 1);
            if (!hashList.Contains(left))
            {
                TrUtils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X, worldPosition.Y),
                    new Vector2(worldPosition.X, worldPosition.Y + 18), borderColor, borderColor, 2f);
            }

            if (!hashList.Contains(right))
            {
                TrUtils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X + 16, worldPosition.Y),
                    new Vector2(worldPosition.X + 16, worldPosition.Y + 18), borderColor, borderColor, 2f);
            }

            if (!hashList.Contains(up))
            {
                int offset = Main.LocalPlayer.gravDir is -1f ? 16 : 0;
                TrUtils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X, worldPosition.Y + offset),
                    new Vector2(worldPosition.X + 16, worldPosition.Y + offset), borderColor, borderColor, 2f);
            }

            if (!hashList.Contains(bottom))
            {
                int offset = Main.LocalPlayer.gravDir is -1f ? 0 : 16;
                TrUtils.DrawLine(Main.spriteBatch, new Vector2(worldPosition.X, worldPosition.Y + offset),
                    new Vector2(worldPosition.X + 16, worldPosition.Y + offset), borderColor, borderColor, 2f);
            }
        }
    }
}