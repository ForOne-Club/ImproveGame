using Terraria.ModLoader;

namespace ImproveGame.Common.ModSystems
{
    public class WandSystem : ModPlayer
    {
        // 爆破法杖
        public static bool FixedMode = true;
        public static bool TileMode = true;
        public static bool WallMode = true;
        public static bool ChestMode = false;

        // 液体法杖
        public static bool AbsorptionMode = false;
        public static short LiquidMode = 0;

        // 自动钓鱼机
        public static bool SelectPoolMode = false;

        // 画画魔杖
        [Serializable]
        public enum PaintMode : byte { Tile, Wall, Remove };

        public static PaintMode PaintWandMode = PaintMode.Tile;

        // 构造法杖
        public enum Construct : byte { Save, Place, ExplodeAndPlace };

        public static Construct ConstructMode = Construct.Save;
        public static Construct ExplodeMode = Construct.ExplodeAndPlace;
        public static string ConstructFilePath;

        /// <summary>
        /// 切换吸收模式
        /// </summary>
        /// <returns>切换后当前模式</returns>
        public static bool ChangeAbsorptionMode() => AbsorptionMode = !AbsorptionMode;
    }
}