using ImproveGame.Common.ModSystems;
using ImproveGame.Content.Functions;
using ImproveGame.Packets.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace ImproveGame.Content.Items;
public class ShellShipInBottle : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ShellShipInBottle_Shimmered>();
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.CombatBook);
        Item.useAnimation = Item.useTime = 0;
        Item.rare = ItemRarityID.Blue;
        Item.Size = new Vector2(42, 20);
        Item.value = Item.sellPrice(silver: 20);
        base.SetDefaults();
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ItemID.Seashell, 5)
            .AddIngredient(ItemID.Wood, 10)
            .AddIngredient(ItemID.FallenStar, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
public class ShellShipInBottle_Shimmered : ModItem
{

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.CombatBook);
        Item.Size = new Vector2(42, 20);
        Item.useStyle = ItemUseStyleID.RaiseLamp; // 用4的话物品会错位不在手上
        Item.value = Item.sellPrice(silver: 25);
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        if (QuickShimmerSystem.Unlocked)
        {
            if (player.itemAnimation == player.itemAnimationMax)
                AddNotification(GetText("UI.QuickShimmer.AlreadyUnlocked"), Color.Pink);
            return null;
        }
        QuickShimmerSystem.Unlocked = true;
        WorldGen.BroadcastText(NetworkText.FromKey("Mods.ImproveGame.UI.QuickShimmer.Unlocked"), Color.Pink);
        return true;
    }
    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }
    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        base.PostDrawInWorld(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
    }
}
public class QuickShimmerSystem : ModSystem 
{
    public static bool Unlocked;
    public static bool Enabled => Unlocked && Config.QuickShimmer;
    public override void SaveWorldData(TagCompound tag)
    {
        if (Unlocked) tag.Add("unlocked", true);
        base.SaveWorldData(tag);
    }
    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.ContainsKey("unlocked")) Unlocked = tag.GetBool("unlocked");

        base.LoadWorldData(tag);
    }
}
