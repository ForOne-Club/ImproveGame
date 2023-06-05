using ImproveGame.Common.Animations;

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

        if (item.ModItem is IMarqueeItem marqueeItem && marqueeItem.ShouldDrawing)
        {
            Rectangle marquee = marqueeItem.Marquee;
            Vector2 position = new Vector2(marquee.X, marquee.Y);
            Vector2 size = marquee.Size();

            Color borderColor = marqueeItem.BorderColor;
            Color backgroundColor = marqueeItem.BackgroundColor;

            bool shouldDrawing = true;

            marqueeItem.PreDraw(ref shouldDrawing, marquee, backgroundColor, borderColor);

            if (shouldDrawing)
            {
                SDFRectangle.HasBorder(
                    position - new Vector2(2) - Main.screenPosition,
                    size + new Vector2(4),
                    new Vector4(2),
                    backgroundColor, 2f, borderColor, false);

                marqueeItem.PostDraw(marquee, backgroundColor, borderColor);
            }
        }
    }
}
