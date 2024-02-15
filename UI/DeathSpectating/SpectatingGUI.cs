using ImproveGame.UIFramework;
using ImproveGame.UIFramework.BaseViews;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UI.DeathSpectating;

[AutoCreateGUI(LayerName.Vanilla.RadialHotbars, "Spectating GUI", 50)]
public class SpectatingGUI : BaseBody
{
    public static SpectatingGUI Instance { get; private set; }
    public SpectatingGUI() => Instance = this;

    public override bool Enabled { get; set; }
    public override bool CanSetFocusTarget(UIElement target) => ActivePlayerList.IsMouseHovering;

    public SUIPanel Window { get; init; } = new SUIPanel(Color.White, Color.LightGray * 0.5f);
    public SUIScrollView2 ActivePlayerList { get; init; } = new SUIScrollView2(Orientation.Vertical);

    public override void OnInitialize()
    {
        Window.FinallyDrawBorder = true;
        Window.IsAdaptiveWidth = Window.IsAdaptiveHeight = true;
        Window.SetAlign(0.5f, 0.8f);
        Window.SetPadding(1.5f);
        Window.JoinParent(this);

        ActivePlayerList.SetSize(400f, 300f);
        ActivePlayerList.RelativeMode = RelativeMode.Horizontal;
        ActivePlayerList.PreventOverflow = true;
        ActivePlayerList.JoinParent(Window);

        var tail = ViewHelper.CreateTail(Color.Black * 0.5f, 45f, 12f);
        tail.JoinParent(Window);
    }

    public IReadOnlyList<Player> PreviousActivePlayers = [];

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        List<Player> current = Main.LocalPlayer.GetModPlayer<SpectatingPlayer>().ActivePlayers;
        if (!PreviousActivePlayers.SequenceEqual(current))
        {
            PreviousActivePlayers = new List<Player>(current);

            ActivePlayerList.ListView.RemoveAllChildren();
            foreach (var player in current)
            {
                var playerButton = new SUIText
                {
                    TextOrKey = player.name,
                    TextAlign = new Vector2(0.5f),
                    RelativeMode = RelativeMode.Vertical,
                    Spacing = new Vector2(4f),
                };
                playerButton.SetRoundedRectProperties(Color.LightGray * 0.5f, 2f, Color.White, 12f);
                playerButton.Width.Percent = 1f;
                playerButton.Height.Pixels = 45f;
                playerButton.OnLeftMouseDown += (_, _) =>
                {
                    Main.NewText($"视角切换至: {player.name}");

                    Main.LocalPlayer.GetModPlayer<SpectatingPlayer>().SpectatingTarget = player.whoAmI;
                    SpectatingPlayerPacket.SynsSpectatingTarget(player.whoAmI);
                };
                playerButton.OnUpdate += _ =>
                {
                    playerButton.BorderColor = Color.White * playerButton.HoverTimer.Lerp(0.5f, 1f);
                };
                playerButton.JoinParent(ActivePlayerList.ListView);
            }

            ActivePlayerList.ListView.Recalculate();
        }
    }
}
