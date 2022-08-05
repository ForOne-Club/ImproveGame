using ImproveGame.Common.Players;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace ImproveGame.Common.Systems
{
    public class BigBagRecipeSystem : ModSystem
    {
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
            c.Emit(OpCodes.Ldsfld, typeof(BigBagRecipeSystem).GetField(nameof(_loadedSuperVault), BindingFlags.Static | BindingFlags.NonPublic));
            c.Emit(OpCodes.Brfalse, label); // 如果没加载，跳出
            c.Emit(OpCodes.Pop); // 直接丢弃，因为sbyte不支持127以上
            c.Emit(OpCodes.Ldc_I4, 59 + 100); // 玩家背包 + 大背包
            c.MarkLabel(label); // pop和ldc_i4之后，直接跳到这里就没那两句
        }

        // 两边代码异常相似，所以我直接用上面的了
        private void ConsumeBigBagMaterial(ILContext il) => AllowBigBagAsMeterial(il);

        // 两边代码异常相似，所以我封装成一个方法了
        private Item[] GetWholeInventory(Item[] inventory)
        {
            _loadedSuperVault = false;
            if (Config.SuperVault && DataPlayer.TryGet(Main.LocalPlayer, out var modPlayer) && modPlayer.SuperVault is not null)
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
