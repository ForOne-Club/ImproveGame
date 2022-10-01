using System.Reflection;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Core;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;

namespace ImproveGame.Common.Systems
{
    internal class BestiaryUnlockSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            if (Main.dedServ)
                return;

            // 获取所有继承IBestiaryUICollectionInfoProvider的类
            var providers = typeof(Main).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.ContainsGenericParameters)
                .Where(t => t.IsAssignableTo(typeof(IBestiaryUICollectionInfoProvider)))
                .OrderBy(type => type.FullName, StringComparer.InvariantCulture);

            // 实时挂On
            foreach (var t in providers)
            {
                HookEndpointManager.Add(t.GetMethod("GetEntryUICollectionInfo"), (Func<IBestiaryUICollectionInfoProvider, BestiaryUICollectionInfo> orig, IBestiaryUICollectionInfoProvider provider) => {
                    var info = orig.Invoke(provider);
                    if (Config.BestiaryQuickUnlock && info.UnlockState != BestiaryEntryUnlockState.NotKnownAtAll_0)
                    {
                        info.UnlockState = BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
                    }
                    return info;
                });
            }
        }
    }
}
