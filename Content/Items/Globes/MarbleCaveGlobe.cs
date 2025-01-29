using ImproveGame.Common.Conditions;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes;

public class MarbleCaveGlobe () : GlobePlentyTooltip(ItemRarityID.Quest, Item.sellPrice(silver: 30))
{
    public class MarbleCaveGlobeProj : GlobeProjBase
    {
        public override ModItem GetModItemDummy() => ModContent.GetInstance<MarbleCaveGlobe>();
    }

    public override bool RevealOperation(Projectile projectile, bool onlyJudging)
    {
        if (StructureDatas.AllMarbleCavePositions.Count is 0)
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AddNotification(GetLocalizedText("NotFound").ToString(), Color.PaleVioletRed * 1.4f);
            return false;
        }

        if (StructureDatas.AllMarbleCavePositions.Count <= StructureDatas.MarbleCavePositions.Count)
        {
            if (!onlyJudging && projectile.owner == Main.myPlayer)
                AddNotification(this.GetLocalizedValue("NotFound"), Color.PaleVioletRed * 1.4f);
            return false;
        }

        if (onlyJudging)
            return true;

        StructureDatas.MarbleCavePositions.Add(StructureDatas.AllMarbleCavePositions
            .Except(StructureDatas.MarbleCavePositions)
            .MinBy(position => projectile.Center.Distance(position.ToVector2() * 16)));

        var text = Language.GetText("Mods.ImproveGame.Items.GlobeBase.Reveal")
            .WithFormatArgs(this.GetLocalizedValue("BiomeName"), Main.player[projectile.owner].name);
        AddNotification(text.Value, Color.Pink);
        return true;
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 8)
            .AddRecipeGroup(RecipeGroupID.IronBar, 4)
            .AddIngredient(ItemID.Ruby)
            .AddTile(TileID.WorkBenches)
            .AddCondition(ConfigCondition.EnableMinimapMarkC);
}