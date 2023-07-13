using ImproveGame.Common.ModPlayers;
using ImproveGame.Interface.Common;
using ImproveGame.Interface.ExtremeStorage;
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
        AnyCopperBar = new(() => GetText($"RecipeGroup.{nameof(AnyCopperBar)}"), 20, 703);
        AnySilverBar = new(() => GetText($"RecipeGroup.{nameof(AnySilverBar)}"), 21, 705);
        AnyGoldBar = new(() => GetText($"RecipeGroup.{nameof(AnyGoldBar)}"), 19, 706);
        AnyDemoniteBar = new(() => GetText($"RecipeGroup.{nameof(AnyDemoniteBar)}"), 57, 1257);
        AnyShadowScale = new(() => GetText($"RecipeGroup.{nameof(AnyShadowScale)}"), 86, 1329);
        AnyCobaltBar = new(() => GetText($"RecipeGroup.{nameof(AnyCobaltBar)}"), 381, 1184);
        AnyMythrilBar = new(() => GetText($"RecipeGroup.{nameof(AnyMythrilBar)}"), 382, 1191);
        AnyAdamantiteBar = new(() => GetText($"RecipeGroup.{nameof(AnyAdamantiteBar)}"), 391, 1198);

        RecipeGroup.RegisterGroup("ImproveGame:AnyGoldBar", AnyGoldBar);
        RecipeGroup.RegisterGroup("ImproveGame:AnySilverBar", AnySilverBar);
        RecipeGroup.RegisterGroup("ImproveGame:AnyCopperBar", AnyCopperBar);
        RecipeGroup.RegisterGroup("ImproveGame:AnyShadowScale", AnyShadowScale);
        RecipeGroup.RegisterGroup("ImproveGame:AnyDemoniteBar", AnyDemoniteBar);
        RecipeGroup.RegisterGroup("ImproveGame:AnyCobaltBar", AnyCobaltBar);
        RecipeGroup.RegisterGroup("ImproveGame:AnyMythrilBar", AnyMythrilBar);
        RecipeGroup.RegisterGroup("ImproveGame:AnyAdamantiteBar", AnyAdamantiteBar);
    }
}

public class MaterialConsumer : ModPlayer
{
    public static IEnumerable<Item> ExtendedCraftingMaterials
    {
        get
        {
            var finalItems = new List<Item>();

            if (Config.SuperVault && Main.LocalPlayer.GetModPlayer<UIPlayerSetting>().SuperVault_HeCheng &&
                DataPlayer.TryGet(Main.LocalPlayer, out var modPlayer) && modPlayer.SuperVault is not null)
            {
                finalItems.AddRange(modPlayer.SuperVault);
            }

            if (ExtremeStorageGUI.Visible && ExtremeStorageGUI.Storage.UseForCrafting &&
                Main.netMode is NetmodeID.SinglePlayer && ExtremeStorageGUI.CurrentGroup is not ItemGroup.Setting &&
                ExtremeStorageGUI.AllItemsCached is not null)
            {
                finalItems.AddRange(ExtremeStorageGUI.AllItemsCached);
            }

            return finalItems;
        }
    }
    
    public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback)
    {
        itemConsumedCallback = null;
        return ExtendedCraftingMaterials;
    }
}