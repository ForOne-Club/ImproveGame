using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace ImproveGame.Common.Systems
{
    public class RecipeSystem : ModSystem
    {
        internal static RecipeGroup AnyCopperBar;
        internal static RecipeGroup AnySilverBar;
        internal static RecipeGroup AnyGoldBar;
        internal static RecipeGroup AnyDemoniteBar;
        internal static RecipeGroup AnyShadowScale;
        internal static RecipeGroup AnyCobaltBar;
        internal static RecipeGroup AnyMythrilBar;
        internal static RecipeGroup AnyAdamantiteBar;

        public override void Unload()
        {
            AnyCopperBar = null;
            AnySilverBar = null;
            AnyGoldBar = null;
            AnyDemoniteBar = null;
            AnyShadowScale = null;
            AnyCobaltBar = null;
            AnyMythrilBar = null;
            AnyAdamantiteBar = null;
        }

        public override void AddRecipeGroups()
        {
            AnyCopperBar = new(() => GetText($"RecipeGroup.{nameof(AnyCopperBar)}"), 20, 703);
            AnySilverBar = new(() => GetText($"RecipeGroup.{nameof(AnySilverBar)}"), 21, 705);
            AnyGoldBar = new(() => GetText($"RecipeGroup.{nameof(AnyGoldBar)}"), 19, 706);
            AnyDemoniteBar = new(() => GetText($"RecipeGroup.{nameof(AnyDemoniteBar)}"), 57, 1257);
            AnyShadowScale = new(() => GetText($"RecipeGroup.{nameof(AnyShadowScale)}"), 86, 1329);
            AnyCobaltBar = new(() => GetText($"RecipeGroup.{nameof(AnyCobaltBar)}"), 381, 1184);
            AnyMythrilBar = new(() => GetText($"RecipeGroup.{nameof(AnyMythrilBar)}"), 382, 1191);
            AnyAdamantiteBar = new(() => GetText($"RecipeGroup.{nameof(AnyAdamantiteBar)}"), 391, 1198);

            RecipeGroup.RegisterGroup("ImproveGame:AnyGoldBar", AnyGoldBar);
            RecipeGroup.RegisterGroup("ImproveGame:AnySilverBar", AnySilverBar);
            RecipeGroup.RegisterGroup("ImproveGame:AnyCopperBar", AnyCopperBar);
            RecipeGroup.RegisterGroup("ImproveGame:AnyShadowScale", AnyShadowScale);
            RecipeGroup.RegisterGroup("ImproveGame:AnyDemoniteBar", AnyDemoniteBar);
            RecipeGroup.RegisterGroup("ImproveGame:AnyCobaltBar", AnyCobaltBar);
            RecipeGroup.RegisterGroup("ImproveGame:AnyMythrilBar", AnyMythrilBar);
            RecipeGroup.RegisterGroup("ImproveGame:AnyAdamantiteBar", AnyAdamantiteBar);
        }

        private static bool _loadedSuperVault; // 本次运行是否加载了大背包，全局字段避免出问题

        public override void Load()
        {
            // 配方列表
            IL.Terraria.Recipe.FindRecipes += AllowBigBagAsMeterial;
            // 物品消耗
            IL.Terraria.Recipe.Create += ConsumeBigBagMaterial;
        }

        private void AllowBigBagAsMeterial(ILContext il)
        {
            var c = new ILCursor(il);

            /* IL_0268: ldsfld    class Terraria.Player[] Terraria.Main::player
             * IL_026D: ldsfld    int32 Terraria.Main::myPlayer
             * IL_0272: ldelem.ref
             * IL_0273: ldfld     class Terraria.Item[] Terraria.Player::inventory
             * IL_0278: stloc.s   'array'
             */
            if (!c.TryGotoNext(
                MoveType.After,
                i => i.MatchLdsfld<Main>(nameof(Main.player)),
                i => i.MatchLdsfld<Main>(nameof(Main.myPlayer)),
                i => i.Match(OpCodes.Ldelem_Ref),
                i => i.MatchLdfld<Player>(nameof(Player.inventory))
            ))
                return;

            c.EmitDelegate(GetWholeInventory);

            /* IL_02E4: ldloc.s   l
             * IL_02E6: ldc.i4.s  58
             * IL_02E8: blt.s     IL_027F
             */
            if (!c.TryGotoNext(
                MoveType.After,
                i => i.Match(OpCodes.Ldloc_S),
                i => i.Match(OpCodes.Ldc_I4_S, (sbyte)58)
                //i => i.Match(OpCodes.Blt_S) // FindRecipes是Blt_S, Create是Blt, 加上这句就不方便偷懒了
            ))
                return;

            var label = c.DefineLabel(); // 记录位置
            c.Emit(OpCodes.Ldsfld, typeof(RecipeSystem).GetField(nameof(_loadedSuperVault), BindingFlags.Static | BindingFlags.NonPublic));
            c.Emit(OpCodes.Brfalse, label); // 如果没加载，跳出
            c.Emit(OpCodes.Pop); // 直接丢弃，因为sbyte不支持127以上
            c.Emit(OpCodes.Ldc_I4, 59 + 100); // 玩家背包 + 大背包
            c.MarkLabel(label); // pop和ldc_i4之后，直接跳到这里就没那两句
        }

        private void ConsumeBigBagMaterial(ILContext il)
        {
            AllowBigBagAsMeterial(il);

            var c = new ILCursor(il);

            // 将 item.Clone() 加入到 ConsumedItems 列表，而不是原来的直接加入 item
            /* IL_01e9: ldsfld       class [System.Collections]System.Collections.Generic.List`1<class Terraria.Item> Terraria.ModLoader.RecipeLoader::ConsumedItems
             * IL_01ee: ldloc.1      // item
             * IL_01ef: callvirt     instance void class [System.Collections]System.Collections.Generic.List`1<class Terraria.Item>::Add(!0 class Terraria.Item)
             */
            if (!c.TryGotoNext(
                MoveType.Before,
                i => i.MatchLdsfld(typeof(RecipeLoader).GetField("ConsumedItems", BindingFlags.Static | BindingFlags.NonPublic)),
                i => i.Match(OpCodes.Ldloc_1),
                i => i.MatchCallvirt(typeof(List<Item>).GetMethod(nameof(List<Item>.Add), BindingFlags.Public | BindingFlags.Instance))
            ))
                return;

            c.GotoNext(MoveType.After, i => i.Match(OpCodes.Ldloc_1));
            c.Emit<Item>(OpCodes.Callvirt, "Clone");

            // 使用 item.TurnToAir 代替原来的 item = new Item()
            /* IL_01A8: ldloc.0
             * IL_01A9: ldloc.s   k
             * IL_01AB: newobj    instance void Terraria.Item::.ctor()
             * IL_01B0: stelem.ref
             */
            if (!c.TryGotoNext(
                MoveType.After,
                i => i.Match(OpCodes.Ldloc_0),
                i => i.Match(OpCodes.Ldloc_S),
                i => i.Match(OpCodes.Newobj),
                i => i.Match(OpCodes.Stelem_Ref)
            ))
                return;

            c.Emit(OpCodes.Ldloc_1);
            c.Emit(OpCodes.Call, typeof(Item).GetMethod(nameof(Item.TurnToAir), BindingFlags.Instance | BindingFlags.Public));
        }

        // 两边代码异常相似，所以我封装成一个方法了
        private Item[] GetWholeInventory(Item[] inventory)
        {
            _loadedSuperVault = false;
            if (Config.SuperVault && Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_HeCheng && DataPlayer.TryGet(Main.LocalPlayer, out var modPlayer) && modPlayer.SuperVault is not null)
            {
                _loadedSuperVault = true;
                var superVault = modPlayer.SuperVault;
                var inv = new Item[inventory.Length + superVault.Length];
                for (int i = 0; i < inventory.Length - 1; i++) // 原版没包括58，我们也不包括
                {
                    inv[i] = inventory[i];
                }
                inv[inventory.Length - 1] = new();
                for (int i = 0; i < superVault.Length; i++)
                {
                    if (superVault[i] is null)
                    {
                        inv[i + inventory.Length] = new();
                        continue;
                    }
                    inv[i + inventory.Length] = superVault[i];
                }
                return inv;
            }
            return inventory;
        }
    }
}
