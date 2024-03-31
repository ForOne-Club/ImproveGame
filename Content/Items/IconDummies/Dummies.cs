using ImproveGame.Common.ModHooks;
using ImproveGame.UI.RecipeSearch;
using System.Collections.ObjectModel;
using Terraria.DataStructures;
using Terraria.ModLoader.UI;

namespace ImproveGame.Content.Items.IconDummies;

public class AddChestIconDummy : ModItem;

public class UniversalAmmoIcon : ModItem, IHideExtraTooltips
{
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        foreach (var line in tooltips.Where(line => line.Name is not "ItemName" and not "Tooltip0" and not "Tooltip1"))
        {
            line.Visible = false;
        }
    }
}

public class SearchIcon : ModItem, IHideExtraTooltips
{
    public override void AddRecipes()
    {
        var recipe = CreateRecipe().Register();
        recipe.DisableRecipe();
        RecipeSearchSystem.DummyRecipeIndex = recipe.RecipeIndex;
    }

    public override void OnCreated(ItemCreationContext context)
    {
        if (context is not InitializationItemCreationContext)
            Item.SetDefaults();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        foreach (var line in tooltips.Where(line => line.Name is not "ItemName" and not "Tooltip0")) {
            line.Visible = false;
        }
    }
}