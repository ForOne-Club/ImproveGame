namespace ImproveGame.Common.Systems
{
    public class ModRecipeGroup : ModSystem
    {
        public static RecipeGroup GoldGroup;
        public static RecipeGroup SilverGroup;
        public static RecipeGroup IronGroup;
        public static RecipeGroup CopperGroup;
        public static RecipeGroup DemoniteGroup;
        public static RecipeGroup ShadowGroup;

        public override void Unload()
        {
            GoldGroup = null;
            SilverGroup = null;
            IronGroup = null;
            CopperGroup = null;
            DemoniteGroup = null;
            ShadowGroup = null;
        }

        public override void AddRecipeGroups()
        {
            GoldGroup = new(() => GetText("RecipeGroup.GoldGroup"), 19, 706);
            SilverGroup = new(() => GetText("RecipeGroup.SilverGroup"), 21, 705);
            IronGroup = new(() => GetText("RecipeGroup.IronGroup"), 22, 704);
            CopperGroup = new(() => GetText("RecipeGroup.CopperGroup"), 20, 703);
            ShadowGroup = new(() => GetText("RecipeGroup.ShadowGroup"), 86, 1329);
            DemoniteGroup = new(() => GetText("RecipeGroup.DemoniteGroup"), 57, 1257);

            RecipeGroup.RegisterGroup("ImproveGame:GoldGroup", GoldGroup);
            RecipeGroup.RegisterGroup("ImproveGame:SilverGroup", SilverGroup);
            RecipeGroup.RegisterGroup("ImproveGame:IronGroup", IronGroup);
            RecipeGroup.RegisterGroup("ImproveGame:CopperGroup", CopperGroup);
            RecipeGroup.RegisterGroup("ImproveGame:ShadowGroup", ShadowGroup);
            RecipeGroup.RegisterGroup("ImproveGame:DemoniteGroup", DemoniteGroup);
        }
    }
}
