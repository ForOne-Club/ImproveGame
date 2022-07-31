using System.Reflection;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Core;
using MonoMod.RuntimeDetour;

namespace ImproveGame.Common.Systems
{
    internal class BestiaryUnlockSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // 获取所有继承IBestiaryUICollectionInfoProvider的类
            var providers = typeof(Main).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.ContainsGenericParameters)
                .Where(t => t.IsAssignableTo(typeof(IBestiaryUICollectionInfoProvider)))
                .OrderBy(type => type.FullName, StringComparer.InvariantCulture);

            // 实时挂On
            MonoModHooks.RequestNativeAccess();
            foreach (var t in providers)
            {
                // 默认构造自动生效，也就是new完就生效了，不需要处理
                _ = new Hook(t.GetMethod("GetEntryUICollectionInfo"), (Func<IBestiaryUICollectionInfoProvider, BestiaryUICollectionInfo> orig, IBestiaryUICollectionInfoProvider provider) => {
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
