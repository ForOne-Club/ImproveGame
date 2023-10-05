using ImproveGame.Content.NPCs.Dummy;

namespace ImproveGame.Content.Items;

public class Dummy : ModItem
{
    public override void SetDefaults()
    {
        Item.SetBaseValues(34, 36, ItemRarityID.Red, Item.sellPrice(0, 1), 1);
        Item.SetUseValues(ItemUseStyleID.Swing, SoundID.Item1, 15, 15, true);
    }

    public override bool? UseItem(Player player)
    {
        return base.UseItem(player);
    }

    public override bool CanUseItem(Player player)
    {
        bool hasDummy = false;

        foreach (var npc in Main.npc)
        {
            if (npc.active && npc.type == ModContent.NPCType<DummyNPC>() && npc.HasPlayerTarget && npc.target == player.whoAmI)
            {
                hasDummy = true;
                npc.active = false;

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, Scale: 2f);
                    Main.dust[dust].velocity.Y = -1f;
                    Main.dust[dust].noGravity = true;
                }

                break;
            }
        }

        if (!hasDummy)
        {
            NPC npc = NPC.NewNPCDirect(null, 0, 0, ModContent.NPCType<DummyNPC>(), target: player.whoAmI);
            npc.Center = Main.MouseWorld;

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, Scale: 2f);
                Main.dust[dust].velocity.Y = -1f;
                Main.dust[dust].noGravity = true;
            }
        }

        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().Register();
    }
}
