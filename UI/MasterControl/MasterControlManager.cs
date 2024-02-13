using ImproveGame.UI.DeathSpectating;
using ImproveGame.UI.PlayerStats;

namespace ImproveGame.UI.MasterControl;

public class MasterControlManager : ModSystem
{
    public static MasterControlManager Instance => ModContent.GetInstance<MasterControlManager>();

    /// <summary>
    /// 原功能列表 (O, Strat!)
    /// </summary>
    public IReadOnlyList<MasterControlFunction> OriginalMCFuncions => _originalMCFunctions;
    private readonly List<MasterControlFunction> _originalMCFunctions = [];

    public MasterControlFunction Register(MasterControlFunction item)
    {
        if (!_originalMCFunctions.Contains(item))
        {
            _originalMCFunctions.Add(item);
        }

        return item;
    }

    /// <summary>
    /// 排序之后的可用功能列表
    /// </summary>
    public IReadOnlyList<MasterControlFunction> AvailableOrderedMCFunctions => _availableOrderedMCFunctions;
    private readonly List<MasterControlFunction> _availableOrderedMCFunctions = [];

    /// <summary>
    /// 排序之后的不可用功能列表
    /// </summary>
    public IReadOnlyList<MasterControlFunction> UnavailableOrderedMCFunctions => _unavailableOrderedMCFunctions;
    private readonly List<MasterControlFunction> _unavailableOrderedMCFunctions = [];
    public void OrderMCFunctions()
    {
        _availableOrderedMCFunctions.Clear();
        _unavailableOrderedMCFunctions.Clear();

        foreach (var item in _originalMCFunctions.OrderBy(i => i))
        {
            if (item.IsAvailable)
            {
                _availableOrderedMCFunctions.Add(item);
            }
            else
            {
                _unavailableOrderedMCFunctions.Add(item);
            }
        }
    }

    public override void PostSetupContent()
    {
        // 大背包
        var bigBackpack = new MasterControlFunction("SuperVault")
        {
            Icon = ModAsset.BigBackpack.Value,
        }.Register();

        bigBackpack.Available += () => Config.SuperVault;
        bigBackpack.OnMouseDown += tv =>
        {
            if (!Config.SuperVault)
                return;

            if (BigBagGUI.Visible)
                BigBagGUI.Instance.Close();
            else
                BigBagGUI.Instance.Open();
        };

        // 属性面板
        var playerStats = new MasterControlFunction("PlayerStats")
        {
            Icon = ModAsset.PlayerStats.Value,
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

        // 观战
        var spectating = new MasterControlFunction("Spectating")
        {
            Icon = ModAsset.Spectating.Value,
        }.Register();

        spectating.OnMouseDown += tv =>
        {
            var body = SpectatingGUI.Instance;

            body.Enabled = !body.Enabled;
        };

        OrderMCFunctions();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        foreach (var function in OriginalMCFuncions)
        {
            function.UpdateAlways();
        }

        OrderMCFunctions();
    }
}
