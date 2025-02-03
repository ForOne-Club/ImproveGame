using ImproveGame.Content.NPCs.Dummy;
using ImproveGame.UI;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ID;

namespace ImproveGame.Content.Items;

public class Dummy : ModItem
{
    public override void SetDefaults()
    {
        Item.SetBaseValues(34, 36, ItemRarityID.Red, Item.sellPrice(silver: 40), 1);
        Item.SetUseValues(ItemUseStyleID.Swing, SoundID.Item1, 15, 15);
    }

    public override bool? UseItem(Player player)
    {
        return base.UseItem(player);
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override bool CanUseItem(Player player)
    {
        /*if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            AddNotification(GetText("Items.Dummy.CannotUse"), Color.PaleVioletRed * 1.4f);
            return false;
        }*/
        if (Main.myPlayer != player.whoAmI) return false;
        if (player.altFunctionUse == 2)
        {
            foreach (var npc in Main.npc)
            {

                if (npc.active && npc.ModNPC is DummyNPC dummy && dummy.Owner == player.whoAmI && npc.HasPlayerTarget && npc.target == player.whoAmI && npc.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                {
                    //dummy.Disappear();
                    RemoveDummyModule.Get(npc.whoAmI, player.whoAmI).Send(runLocally: true);
                    break;
                }
            }
        }
        else
        {
            //for(int n =0;n < 50;n++)
            // 确保不要完全和已有的重叠
            var spawnPosition = Main.MouseWorld;
            for (var i = 0; i < Main.npc.Length; i++)
            {
                var npc = Main.npc[i];
                if (!npc.active || npc.ModNPC is not DummyNPC dummy || npc.Center != spawnPosition)
                    continue;

                spawnPosition += new Vector2(4);
                // 重新搜，因为可能会和index位于它之前的重叠
                i = -1;
            }
            SyncDummyModule.Get(spawnPosition, player.whoAmI, DummyNPC.LocalConfig).Send(runLocally: true);
        }

        return true;
    }
    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        if (PlayerInput.MouseInfo.MiddleButton == ButtonState.Pressed && player.itemAnimation == 0)
        {
            if (!DummyConfigurationUI.Instance.Enabled)
                DummyConfigurationUI.Instance.Open();
            else
                DummyConfigurationUI.Instance.Close();
            player.itemAnimation = player.itemAnimationMax = 30;
        }
        base.HoldStyle(player, heldItemFrame);
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.Wood, 100)
            .AddIngredient(ItemID.Hay, 50)
            .AddTile(TileID.WorkBenches).Register();
    }
}
