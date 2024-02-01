using ImproveGame.Interface.GUI.PlayerStats;

namespace ImproveGame.GlobalGUI.FunctionList;

public class CCIManager : ModSystem
{
    public static CCIManager Instance { get; private set; }
    public CCIManager() => Instance = this;

    public IReadOnlyList<ControlCenterItem> ControlCenterItems => _controlCenterItems;
    public readonly List<ControlCenterItem> _controlCenterItems = [];

    public void Sort()
    {
        _controlCenterItems.Sort();
    }

    public ControlCenterItem Register(ControlCenterItem item)
    {
        if (!_controlCenterItems.Contains(item))
        {
            _controlCenterItems.Add(item);
        }

        return item;
    }

    public override void Load()
    {
        var bigBackpack = new ControlCenterItem("大背包").Register();

        bigBackpack.OnMouseDown += tv =>
        {
            if (BigBagGUI.Visible)
                BigBagGUI.Instance.Close();
            else
                BigBagGUI.Instance.Open();
        };

        var playerStats = new ControlCenterItem("属性面板")
        {
            Priority = 1,
        }.Register();

        playerStats.OnMouseDown += tv =>
        {
            var body = PlayerStatsGUI.Instance;

            if (body.HasChild(body.Window))
                body.RemoveChild(body.Window);
            else
            {
                Main.playerInventory = true;
                body.Append(body.Window);
            }
        };

        Sort();
    }
}
