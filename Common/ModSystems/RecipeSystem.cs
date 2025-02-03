using ImproveGame.Common.ModPlayers;
using ImproveGame.Content.Items;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace ImproveGame.Common.ModSystems;

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
    internal static RecipeGroup AnyGem;

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
        AnyCopperBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyCopperBar)}"), 20, 703);
        AnySilverBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnySilverBar)}"), 21, 705);
        AnyGoldBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyGoldBar)}"), 19, 706);
        AnyDemoniteBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyDemoniteBar)}"), 57, 1257);
        AnyShadowScale = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyShadowScale)}"), 86, 1329);
        AnyCobaltBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyCobaltBar)}"), 381, 1184);
        AnyMythrilBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyMythrilBar)}"), 382, 1191);
        AnyAdamantiteBar = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyAdamantiteBar)}"), 391, 1198);
        AnyGem = new RecipeGroup(() => GetText($"RecipeGroup.{nameof(AnyGem)}"), ItemID.Sapphire, ItemID.Ruby,
            ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst, ItemID.Diamond, ItemID.Amber);

        // 注册完成后用返回的int值来获取实际的RecipeGroup，处理跨Mod的问题
        AnyGoldBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("GoldBar", AnyGoldBar)];
        AnySilverBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("SilverBar", AnySilverBar)];
        AnyCopperBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("CopperBar", AnyCopperBar)];
        AnyShadowScale = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("ShadowScale", AnyShadowScale)];
        AnyDemoniteBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("DemoniteBar", AnyDemoniteBar)];
        AnyCobaltBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("CobaltBar", AnyCobaltBar)];
        AnyMythrilBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("MythrilBar", AnyMythrilBar)];
        AnyAdamantiteBar = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("AdamantiteBar", AnyAdamantiteBar)];
        AnyGem = RecipeGroup.recipeGroups[RecipeGroup.RegisterGroup("Gem", AnyGem)];
    }

    public override void AddRecipes()
    {
        /* Materials
         * 18 Bottled Water, 1 Obsidian, 1 Mushroom, 1 Cactus, 1Iron/Lead Ore, 2 Fallen Star, 1 Feather, 1 Gold/Platinum, 1 Lens, 1 Shark Fin, 1 Antlion Mandible, 10 Cobweb
         *
         * Flowers
         * 2 Fireblossom, 2 Waterleaf, 7 Daybloom, 6 Blinkroot, 4 Moonglow, 3 Deathweed 2 Shiverthorn
         *
         * Fish
         * 1 Crimson Tigerfish, 1 Armored Cavefish, 1 Prismite, 1 Flarefin Koi, 2 Obsidifish, 1 Hemopiranha, 1 Ebonkoi
         */
        var redPotionCondition = new Condition("Mods.ImproveGame.Configs.ImproveConfigs.InfiniteRedPotion.Condition",
            () => Config.InfiniteRedPotion);
        var ftwCondition = new Condition("Mods.ImproveGame.Configs.ImproveConfigs.RedPotionEverywhere.Condition",
            () => Config.RedPotionEverywhere || Condition.ForTheWorthyWorld.IsMet());

        Recipe.Create(ItemID.RedPotion)
            .AddTile(TileID.Bottles)
            .AddCondition(ftwCondition)
            .AddCondition(redPotionCondition)
            // Materials
            .AddIngredient(ItemID.BottledWater, 18)
            .AddIngredient(ItemID.Obsidian)
            .AddIngredient(ItemID.Mushroom)
            .AddIngredient(ItemID.Cactus)
            .AddIngredient(ItemID.Lens)
            .AddIngredient(ItemID.SharkFin)
            .AddIngredient(ItemID.AntlionMandible)
            .AddIngredient(ItemID.Feather)
            .AddIngredient(ItemID.Cobweb, 10)
            .AddIngredient(ItemID.FallenStar, 2)
            .AddIngredient(ItemID.IronBar)
            // Flowers
            .AddIngredient(ItemID.Fireblossom, 2)
            .AddIngredient(ItemID.Waterleaf, 2)
            .AddIngredient(ItemID.Daybloom, 7)
            .AddIngredient(ItemID.Blinkroot, 6)
            .AddIngredient(ItemID.Moonglow, 4)
            .AddIngredient(ItemID.Deathweed, 3)
            .AddIngredient(ItemID.Shiverthorn, 2)
            // Fish
            .AddIngredient(ItemID.CrimsonTigerfish)
            .AddIngredient(ItemID.ArmoredCavefish)
            .AddIngredient(ItemID.Prismite)
            .AddIngredient(ItemID.FlarefinKoi)
            .AddIngredient(ItemID.Obsidifish, 2)
            .AddIngredient(ItemID.Hemopiranha)
            .AddIngredient(ItemID.Ebonkoi)
            .Register();
    }

    public override void PreUpdateItems()
    {
        // 控制微光
        // 液体法杖月后可直接微光转换为终极液体法杖
        if (!AvailableConfig.AvailableLiquidWandAdvanced)
            return;
        int liquidWand = ModContent.ItemType<LiquidWand>();
        int liquidWandAdvanced = ModContent.ItemType<LiquidWandAdvanced>();
        ItemID.Sets.ShimmerTransformToItem[liquidWand] = NPC.downedMoonlord ? liquidWandAdvanced : -1;
    }
}