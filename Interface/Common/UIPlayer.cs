using ImproveGame.Common.Configs;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Core;
using ImproveGame.Interface.GUI.BannerChest;
using ImproveGame.Interface.ExtremeStorage;
using ImproveGame.Interface.GUI;
using ImproveGame.Interface.PlayerInfo;
using ImproveGame.Interface.GUI.AutoTrash;
using ImproveGame.Interface.GUI.ItemSearcher;
using ImproveGame.Interface.GUI.OpenBag;
using ImproveGame.Interface.GUI.WorldFeature;
using System.Collections;

namespace ImproveGame.Interface.Common;

// 仅在客户端加载
[Autoload(Side = ModSide.Client)]
public class UIPlayer : ModPlayer
{
    private static CoroutineRunner _uiSetupDelayRunner = new();
    internal static Vector2 HugeInventoryUIPosition;
    internal static Vector2 BuffTrackerPosition;
    internal static Vector2 WorldFeaturePosition;
    internal static Vector2 ItemSearcherPosition;
    internal static Vector2 OpenBagPosition;
    internal static bool ShouldShowUI; // 防止进入世界时UI闪一下

    // 函数在玩家进入地图时候调用, 不会在服务器调用, 用来加载 UI, 可以避免一些因 HJson 未加载产生的问题.
    public override void OnEnterWorld()
    {
        // 协程延时执行可以防止进入世界时UI闪一下
        InitUI();
    }

    public override void Unload()
    {
        _uiSetupDelayRunner = null;
    }

    public static void InitUI()
    {
        _uiSetupDelayRunner.StopAll();
        _uiSetupDelayRunner.Run(2f, SetupUI());
    }

    static IEnumerator SetupUI()
    {
        ShouldShowUI = true;
        UISystem uiSystem = UISystem.Instance;
        DataPlayer dataPlayer = Main.LocalPlayer.GetModPlayer<DataPlayer>();

        // 玩家信息
        uiSystem.PlayerInfoGUI = new PlayerInfoGUI();
        uiSystem.PlayerInfoTrigger.SetCarrier(uiSystem.PlayerInfoGUI);
        PlayerInfoGUI.Visible = true;

        // 自动垃圾桶
        uiSystem.AutoTrashGUI = new AutoTrashGUI();
        uiSystem.AutoTrashTrigger.SetCarrier(uiSystem.AutoTrashGUI);

        // 旗帜盒
        uiSystem.PackageGUI = new PackageGUI();
        uiSystem.PackageTrigger.SetCarrier(uiSystem.PackageGUI);

        // 大背包
        uiSystem.BigBagGUI = new BigBagGUI();
        uiSystem.BigBagTrigger.SetCarrier(uiSystem.BigBagGUI);

        uiSystem.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
        CheckPositionValid(ref HugeInventoryUIPosition, 150, 340);
        uiSystem.BigBagGUI.MainPanel.SetPos(HugeInventoryUIPosition).Recalculate();
        
        // 增益追踪器
        uiSystem.BuffTrackerGUI = new BuffTrackerGUI();
        uiSystem.BuffTrackerTrigger.SetCarrier(uiSystem.BuffTrackerGUI);
        CheckPositionValid(ref BuffTrackerPosition, 630, 160);
        uiSystem.BuffTrackerGUI.MainPanel.SetPos(BuffTrackerPosition).Recalculate();
        UISystem.Instance.BuffTrackerGUI.BuffTrackerBattler.ResetDataForNewPlayer(Main.LocalPlayer.whoAmI);

        // 液体法杖
        uiSystem.LiquidWandGUI = new LiquidWandGUI();
        uiSystem.LiquidWandTrigger.SetCarrier(uiSystem.LiquidWandGUI);

        // 建筑法杖
        uiSystem.ArchitectureGUI = new ArchitectureGUI();
        uiSystem.ArchitectureTrigger.SetCarrier(uiSystem.ArchitectureGUI);

        // 构造法杖
        uiSystem.StructureGUI = new StructureGUI();
        uiSystem.StructureTrigger.SetCarrier(uiSystem.StructureGUI);

        // 世界特性
        uiSystem.WorldFeatureGUI = new WorldFeatureGUI();
        uiSystem.WorldFeatureTrigger.SetCarrier(uiSystem.WorldFeatureGUI);
        CheckPositionValid(ref WorldFeaturePosition, 250, 280);
        uiSystem.WorldFeatureGUI.MainPanel.SetPos(WorldFeaturePosition).Recalculate();

        // 物品搜索
        uiSystem.ItemSearcherGUI = new ItemSearcherGUI();
        uiSystem.ItemSearcherTrigger.SetCarrier(uiSystem.ItemSearcherGUI);
        CheckPositionValid(ref ItemSearcherPosition, 620, 400);
        uiSystem.ItemSearcherGUI.MainPanel.SetPos(ItemSearcherPosition).Recalculate();

        // 快速开袋
        uiSystem.OpenBagGUI = new OpenBagGUI();
        uiSystem.OpenBagTrigger.SetCarrier(uiSystem.OpenBagGUI);
        CheckPositionValid(ref OpenBagPosition, 410, 360);
        uiSystem.OpenBagGUI.MainPanel.SetPos(OpenBagPosition).Recalculate();

        // 生命体检测仪筛选
        uiSystem.LifeformAnalyzerGUI = new LifeformAnalyzerGUI();
        uiSystem.LifeformAnalyzerTrigger.SetCarrier(uiSystem.LifeformAnalyzerGUI);

        // 左侧栏
        uiSystem.ExtremeStorageGUI = new ExtremeStorageGUI();
        uiSystem.AutofisherGUI = new AutofisherGUI();
        uiSystem.PrefixRecallGUI = new PrefixRecallGUI();
        SidedEventTrigger.Clear();
        SidedEventTrigger.RegisterViewBody(uiSystem.ExtremeStorageGUI);
        SidedEventTrigger.RegisterViewBody(uiSystem.AutofisherGUI);
        SidedEventTrigger.RegisterViewBody(uiSystem.PrefixRecallGUI);
        yield return null;
    }

    private static void CheckPositionValid(ref Vector2 position, int defaultX, int defaultY)
    {
        if (position.X <= 0 && position.X >= Main.screenWidth)
            position.X = defaultX;
        if (position.Y <= 0 && position.Y >= Main.screenHeight)
            position.Y = defaultY;
    }

    public override void PreUpdate()
    {
        // 这里的判断其实是不需要的，因为SetupUI只会在进入世界的玩家的客户端运行
        // 但是为了防止其他情况下的报错，还是加上了
        if (Main.myPlayer != Player.whoAmI || Main.netMode is NetmodeID.Server)
            return;
        _uiSetupDelayRunner.Update(1);
    }
}