using MonoMod.RuntimeDetour.HookGen;
using Terraria.GameContent.Bestiary;

namespace ImproveGame.Common.Systems
{
    public class BestiaryUnlockSystem : ModSystem
    {
        public static IOrderedEnumerable<Type> Providers;
        public delegate BestiaryUICollectionInfo GetEntryUICollectionInfoDelegate(IBestiaryUICollectionInfoProvider provider);

        public BestiaryUICollectionInfo GetEntryUICollectionInfoHook(GetEntryUICollectionInfoDelegate orig, IBestiaryUICollectionInfoProvider provider)
        {
            var info = orig.Invoke(provider);
            if (Config.BestiaryQuickUnlock && info.UnlockState != BestiaryEntryUnlockState.NotKnownAtAll_0)
            {
                info.UnlockState = BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
            }
            return info;
        }

        public override void PostSetupContent()
        {
            if (Main.dedServ)
                return;

            // 获取所有继承IBestiaryUICollectionInfoProvider的类
            Providers = typeof(Main).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.ContainsGenericParameters)
                .Where(t => t.IsAssignableTo(typeof(IBestiaryUICollectionInfoProvider)))
                .OrderBy(type => type.FullName, StringComparer.InvariantCulture);
                
            // 实时挂On
            foreach (var t in Providers)
                HookEndpointManager.Add(t.GetMethod("GetEntryUICollectionInfo"), GetEntryUICollectionInfoHook);
        }

        public override void Unload()
        {
            if (Providers is null)
                return;

            foreach (var t in Providers)
                HookEndpointManager.Remove(t.GetMethod("GetEntryUICollectionInfo"), GetEntryUICollectionInfoHook);

            Providers = null;
        }
    }
}