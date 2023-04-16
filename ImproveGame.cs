using ImproveGame.Common;
using ImproveGame.Common.Systems;
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

    public override void Unload()
    {
        Instance = null;
        Config = null;
        GC.Collect();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);

    public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);
}