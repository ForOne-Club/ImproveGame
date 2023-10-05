using ImproveGame.Common;
using ImproveGame.Common.ModSystems;
using ImproveGame.Interface.GUI;
using Terraria.UI.Chat;

namespace ImproveGame;

public class ImproveGame : Mod
{
    // 额外BUFF槽
    public override uint ExtraPlayerBuffSlots => (uint)Config.ExtraPlayerBuffSlots;
    public static ImproveGame Instance { get; private set; }

    // 成员变量不需要自己设 null
    public readonly RenderTarget2DPool RenderTarget2DPool = new RenderTarget2DPool();

    public override void Load()
    {
        Instance = this;
        AddContent<NetModuleLoader>();
        ChatManager.Register<BgItemTagHandler>("bgitem");
        ChatManager.Register<CenteredItemTagHandler>("centeritem");
    }
    public override void PostSetupContent()
    {
        #region 初始化 AccessoryAttribute
        // 初始化
        AccessoryAttribute.FishingAddition = new float[ItemLoader.ItemCount];
        AccessoryAttribute.FishingPower = new int[ItemLoader.ItemCount];
        AccessoryAttribute.TackleBox = new bool[ItemLoader.ItemCount];
        AccessoryAttribute.LavaFishing = new bool[ItemLoader.ItemCount];
        // 钓具箱
        AccessoryAttribute.FishingAddition[ItemID.TackleBox] = 1f;
        AccessoryAttribute.FishingPower[ItemID.TackleBox] = 0;
        AccessoryAttribute.TackleBox[ItemID.TackleBox] = true;
        AccessoryAttribute.LavaFishing[ItemID.TackleBox] = false;
        // 渔夫耳环
        AccessoryAttribute.FishingAddition[ItemID.AnglerEarring] = 2f;
        AccessoryAttribute.FishingPower[ItemID.AnglerEarring] = 10;
        AccessoryAttribute.TackleBox[ItemID.AnglerEarring] = false;
        AccessoryAttribute.LavaFishing[ItemID.AnglerEarring] = false;
        // 渔夫渔具袋
        AccessoryAttribute.FishingAddition[ItemID.AnglerTackleBag] = 3f;
        AccessoryAttribute.FishingPower[ItemID.AnglerTackleBag] = 10;
        AccessoryAttribute.TackleBox[ItemID.AnglerTackleBag] = true;
        AccessoryAttribute.LavaFishing[ItemID.AnglerTackleBag] = false;
        // 防熔岩钓钩
        AccessoryAttribute.FishingAddition[ItemID.LavaFishingHook] = 1f;
        AccessoryAttribute.FishingPower[ItemID.LavaFishingHook] = 0;
        AccessoryAttribute.TackleBox[ItemID.LavaFishingHook] = false;
        AccessoryAttribute.LavaFishing[ItemID.LavaFishingHook] = true;
        // 防熔岩渔具袋
        AccessoryAttribute.FishingAddition[ItemID.LavaproofTackleBag] = 5f;
        AccessoryAttribute.FishingPower[ItemID.LavaproofTackleBag] = 10;
        AccessoryAttribute.TackleBox[ItemID.LavaproofTackleBag] = true;
        AccessoryAttribute.LavaFishing[ItemID.LavaproofTackleBag] = true;
        #endregion
    }

    public override void Unload()
    {
        Instance = null;
        Config = null;
        GC.Collect();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);

    public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);
}