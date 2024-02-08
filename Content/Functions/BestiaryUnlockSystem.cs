using Terraria.GameContent.Bestiary;

namespace ImproveGame.Content.Functions
{
    public class BestiaryUnlockSystem : ModSystem
    {
        private delegate BestiaryUICollectionInfo GetEntryUICollectionInfoDelegate(IBestiaryUICollectionInfoProvider provider);

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
                MonoModHooks.Add(t.GetMethod("GetEntryUICollectionInfo"),
                    (GetEntryUICollectionInfoDelegate orig, IBestiaryUICollectionInfoProvider provider) =>
                    {
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