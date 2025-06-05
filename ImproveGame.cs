using ImproveGame.Common;
using ImproveGame.Common.ModSystems;
using System.Reflection;
using Terraria.ModLoader.Core;
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

    public override void Load()
    {
        AddContent<NetModuleLoader>();
        ChatManager.Register<BgItemTagHandler>("bgitem");
        ChatManager.Register<CenteredItemTagHandler>("centeritem");
        ChatManager.Register<QotGlyphTagHandler>("qotglyph");

        LocalizationFix();
    }

    public override void Unload()
    {
        Config = null;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) => NetModule.ReceiveModule(reader, whoAmI);

    public override object Call(params object[] args) => ModIntegrationsSystem.Call(args);


    #region 本地化热更新修复

    static string SourceFolderFix(Func<Mod, string> orig, Mod self)
    {
        var result = orig.Invoke(self);
        if (result.Length == 0)
            result = Path.Combine(ModCompile.ModSourcePath, self.Name);
        return result;
    }
    static List<(string key, string value)> LocalizationLoadFix(Func<Mod, GameCulture, List<(string key, string value)>> orig, Mod mod, GameCulture culture)
        => orig.Invoke(mod, culture);

    static void LocalizationFix()
    {
        MonoModHooks.Add(typeof(Mod).GetMethod("get_SourceFolder"), SourceFolderFix);
        MonoModHooks.Add(typeof(LocalizationLoader).GetMethod("LoadTranslations", BindingFlags.Static | BindingFlags.NonPublic), LocalizationLoadFix);
    }
    #endregion
}