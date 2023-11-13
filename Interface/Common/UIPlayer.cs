using ImproveGame.Common.Configs;
using ImproveGame.Common.ModPlayers;
using ImproveGame.Core;
using ImproveGame.Interface.GUI.BannerChest;
using ImproveGame.Interface.ExtremeStorage;
using ImproveGame.Interface.GUI;
using ImproveGame.Interface.PlayerProperty;
using ImproveGame.Interface.GUI.AutoTrash;
using ImproveGame.Interface.GUI.ItemSearcher;
using ImproveGame.Interface.GUI.OpenBag;
using ImproveGame.Interface.GUI.WorldFeature;
using System.Collections;
using ImproveGame.Interface.GUI.PlayerProperty;
using Terraria.ModLoader.IO;

namespace ImproveGame.Interface.Common;

// 仅在客户端加载
[Autoload(Side = ModSide.Client)]
public class UIPlayer : ModPlayer
{
    private static CoroutineRunner _uiSetupDelayRunner = new();
    internal static readonly Vector2 HugeInventoryDefPosition = new(150, 340);
    internal static Vector2 HugeInventoryUIPosition;
    internal static readonly Vector2 BuffTrackerDefPosition = new(630, 160);
    internal static Vector2 BuffTrackerPosition;
    internal static readonly Vector2 WorldFeatureDefPosition = new(250, 280);
    internal static Vector2 WorldFeaturePosition;
    internal static readonly Vector2 ItemSearcherDefPosition = new(620, 400);
    internal static Vector2 ItemSearcherPosition;
    internal static readonly Vector2 OpenBagDefPosition = new(410, 360);
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
        uiSystem.PlayerInfoGUI = new PlayerPropertyGUI();
        uiSystem.PlayerInfoTrigger.SetCarrier(uiSystem.PlayerInfoGUI);
        PlayerPropertyGUI.Visible = true;

        if (Main.LocalPlayer.TryGetModPlayer(out UIPlayerSetting playerSetting))
        {
            var proCats = PlayerPropertySystem.Instance.PropertyCategorys;

            foreach (var item in proCats)
            {
                if (playerSetting.ProCatsPos.TryGet(item.Key, out Vector2 pos))
                {
                    item.Value.UIPosition = pos;
                }

                if (playerSetting.ProCatsPos.TryGet(item.Key, out bool fav))
                {
                    item.Value.Favorite = fav;
                }

                if (playerSetting.ProCatsPos.TryGet(item.Key, out TagCompound tags))
                {
                    foreach (var pro in item.Value.BasePropertys)
                    {
                        if (tags.TryGet(pro.Name, out bool proFav))
                        {
                            pro.Favorite = proFav;
                        }
                    }
                }
            }
        }

        // 自动垃圾桶
        uiSystem.AutoTrashGUI = new GarbageListGUI();
        uiSystem.AutoTrashTrigger.SetCarrier(uiSystem.AutoTrashGUI);

        // 自动垃圾桶
        uiSystem.InventoryTrashGUI = new InventoryTrashGUI();
        uiSystem.InventoryTrashTrigger.SetCarrier(uiSystem.InventoryTrashGUI);

        // 旗帜盒
        uiSystem.PackageGUI = new PackageGUI();
        uiSystem.PackageTrigger.SetCarrier(uiSystem.PackageGUI);

        // 大背包
        uiSystem.BigBagGUI = new BigBagGUI();
        uiSystem.BigBagTrigger.SetCarrier(uiSystem.BigBagGUI);

        uiSystem.BigBagGUI.ItemGrid.SetInventory(dataPlayer.SuperVault);
        CheckPositionValid(ref HugeInventoryUIPosition, HugeInventoryDefPosition);
        uiSystem.BigBagGUI.MainPanel.SetPos(HugeInventoryUIPosition).Recalculate();

        // 增益追踪器
        uiSystem.BuffTrackerGUI = new BuffTrackerGUI();
        uiSystem.BuffTrackerTrigger.SetCarrier(uiSystem.BuffTrackerGUI);
        CheckPositionValid(ref BuffTrackerPosition, BuffTrackerDefPosition);
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
        CheckPositionValid(ref WorldFeaturePosition, WorldFeatureDefPosition);
        uiSystem.WorldFeatureGUI.MainPanel.SetPos(WorldFeaturePosition).Recalculate();

        // 物品搜索
        uiSystem.ItemSearcherGUI = new ItemSearcherGUI();
        uiSystem.ItemSearcherTrigger.SetCarrier(uiSystem.ItemSearcherGUI);
        CheckPositionValid(ref ItemSearcherPosition, ItemSearcherDefPosition);
        uiSystem.ItemSearcherGUI.MainPanel.SetPos(ItemSearcherPosition).Recalculate();

        // 快速开袋
        uiSystem.OpenBagGUI = new OpenBagGUI();
        uiSystem.OpenBagTrigger.SetCarrier(uiSystem.OpenBagGUI);
        CheckPositionValid(ref OpenBagPosition, OpenBagDefPosition);
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

    public static void CheckPositionValid(ref Vector2 position, Vector2 defaultPosition)
    {
        if (position.X <= 0 && position.X >= Main.screenWidth)
            position.X = defaultPosition.X;
        if (position.Y <= 0 && position.Y >= Main.screenHeight)
            position.Y = defaultPosition.Y;
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