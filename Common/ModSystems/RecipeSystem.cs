using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.Players;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.ExtremeStorage;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace ImproveGame.Common.ModSystems
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

        public override void Load()
        {
            // 配方列表
            Terraria.IL_Recipe.FindRecipes += AllowBigBagAsMeterial;
            // 物品消耗
            Terraria.IL_Recipe.Create += ConsumeBigBagMaterial;
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

            c.Emit(OpCodes.Pop); // 直接丢弃，因为sbyte不支持127以上
            c.Emit(OpCodes.Ldc_I4, 58); // 玩家背包 + 大背包
            c.Emit(OpCodes.Call,
                this.GetType().GetMethod(nameof(GetAllExtraStorageLength), BindingFlags.Static | BindingFlags.Public));
            c.Emit(OpCodes.Add);
        }

        /// <summary>
        /// 为了方便直接写了一个方法，在IL里直接调用就行了
        /// 这里不使用 GetExtraItems().Count 是为了节省资源
        /// </summary>
        public static int GetAllExtraStorageLength()
        {
            int count = 0;

            if (Config.SuperVault && Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_HeCheng &&
                DataPlayer.TryGet(Main.LocalPlayer, out var modPlayer) && modPlayer.SuperVault is not null)
            {
                count += modPlayer.SuperVault.Length;
            }

            if (ExtremeStorageGUI.Visible && ExtremeStorageGUI.Storage.UseForCrafting &&
                Main.netMode is NetmodeID.SinglePlayer && ExtremeStorageGUI.CurrentGroup is not ItemGroup.Setting &&
                ExtremeStorageGUI.AllItemsCached is not null)
            {
                count += ExtremeStorageGUI.AllItemsCached.Length;
            }

            return count;
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
            c.Emit(OpCodes.Call,
                typeof(Item).GetMethod(nameof(Item.TurnToAir), BindingFlags.Instance | BindingFlags.Public));
        }

        // 两边代码异常相似，所以我封装成一个方法了
        private Item[] GetWholeInventory(Item[] inventory)
        {
            var itemList = new List<Item>(inventory);
            itemList[58] = new Item();
            itemList.AddRange(GetExtraItems());
            return itemList.ToArray();
        }

        public static IEnumerable<Item> GetExtraItems()
        {
            var finalItems = new List<Item>();

            if (Config.SuperVault && Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_HeCheng &&
                DataPlayer.TryGet(Main.LocalPlayer, out var modPlayer) && modPlayer.SuperVault is not null)
            {
                finalItems.AddRange(modPlayer.SuperVault);
            }

            if (ExtremeStorageGUI.Visible && ExtremeStorageGUI.Storage.UseForCrafting &&
                Main.netMode is NetmodeID.SinglePlayer && ExtremeStorageGUI.CurrentGroup is not ItemGroup.Setting &&
                ExtremeStorageGUI.AllItemsCached is not null)
            {
                finalItems.AddRange(ExtremeStorageGUI.AllItemsCached);
            }

            return finalItems;
        }
    }
}