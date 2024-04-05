using ImproveGame.Common;
using ImproveGame.Common.ModSystems;
using ImproveGame.Core;
using Terraria.UI.Chat;

namespace ImproveGame;

public class ImproveGame : Mod
{
    private class DisplayNameUpdater : ModSystem
    {
        public override void OnLocalizationsLoaded()
        {
            Instance.DisplayName = GetText("ModName");
        }
    }
    
    public static ImproveGame Instance => ModContent.GetInstance<ImproveGame>();

    // 额外BUFF槽
    public override uint ExtraPlayerBuffSlots => (uint)Config.ExtraPlayerBuffSlots;

    public RenderTargetPool RenderTargetPool { get; init; } = new RenderTargetPool();

    public override void Load()
    {
        AddContent<NetModuleLoader>();
        ChatManager.Register<BgItemTagHandler>("bgitem");
        ChatManager.Register<CenteredItemTagHandler>("centeritem");
    }

    public override void Unload()
    {
        Config = null;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);

    public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);
}