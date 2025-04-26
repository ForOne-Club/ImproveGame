using ImproveGame.Common.Conditions;
using ImproveGame.Content.Items.Globes.Core;
using ImproveGame.Content.Projectiles;
using ImproveGame.Packets.WorldFeatures;
using Terraria;

namespace ImproveGame.Content.Items.Globes;

public class PlanteraGlobe() : GlobePlentyTooltip(ItemRarityID.Pink, Item.sellPrice(silver: 30))
{
    public class PlanteraGlobeProj() : GlobeProjBase<PlanteraGlobe>(new(228, 131, 212))
    {
        public override bool RevealOperation(bool onlyJudging)=> GlobeRevealer.RevealPlantera(Projectile,GetModItemDummy(), onlyJudging);
    }

    protected override Recipe AddCraftingMaterials(Recipe recipe) =>
        recipe.AddIngredient(ItemID.Glass, 5)
            .AddIngredient(ItemID.MudBlock, 100)
            .AddIngredient(ItemID.RichMahogany, 30)
            .AddIngredient(ItemID.JungleSpores, 1)
            .AddTile(TileID.MythrilAnvil);
}