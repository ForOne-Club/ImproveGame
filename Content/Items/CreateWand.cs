using ImproveGame.Common.Conditions;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Common.ModHooks;
using ImproveGame.Common.ModSystems;
using ImproveGame.UI;
using ImproveGame.UIFramework;
using ImproveGame.UserInterfaces.CreateWand;
using System.Collections.ObjectModel;

namespace ImproveGame.Content.Items;

public partial class CreateWand : ModItem, IItemOverrideHover, IItemMiddleClickable, IConditionItem
{
    public Condition UseCondition => ConfigCondition.AvailableCreateWandC;

    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults()
    {
        Item.SetBaseValues(42, 42, ItemRarityID.Red, Item.sellPrice(0, 1));
        Item.SetUseValues(ItemUseStyleID.Swing, SoundID.Item1, 16, 16);
    }
    public override bool AltFunctionUse(Player player) => true;

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.Wood, 24)
            .AddRecipeGroup(RecipeSystem.AnyIronBar, 8)
            .AddIngredient(ItemID.FallenStar, 6)
            .Register();
    }

    public override bool CanUseItem(Player player)
    {
        if (player.noBuilding) return false;

        if (player.altFunctionUse == 2 && !Main.dedServ && player.whoAmI == Main.myPlayer)
        {
            CreateWandController.Instance?.Toggle();

            if (!ArchitectureGUI.Visible)
                UISystem.Instance.ArchitectureGUI.Open(this);
            else
                UISystem.Instance.ArchitectureGUI.Close();
            return false;
        }

        return true;
    }

    bool IItemOverrideHover.OverrideHover(Item[] inventory, int context, int slot)
    {
        ((IItemMiddleClickable)this).HandleHover(inventory, context, slot);

        return false;
    }

    void IItemMiddleClickable.OnMiddleClicked(Item item)
    {
        if (!ArchitectureGUI.Visible)
            UISystem.Instance.ArchitectureGUI.Open(this);
        else
            UISystem.Instance.ArchitectureGUI.Close();
    }

    void IItemMiddleClickable.ManageHoverTooltips(Item item, List<TooltipLine> tooltips)
    {
        // 决定文本显示的是“开启”还是“关闭”
        string text = ArchitectureGUI.Visible ? "Off" : "On";
        TryGetKeybindString(KeybindSystem.ItemInteractKeybind, out string keybind);
        tooltips.Add(new TooltipLine(Mod, "CreateWand", GetTextWith($"Tips.CreateWand{text}", new { KeybindName = keybind }))
        { OverrideColor = Color.LightGreen });
    }

    public override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
    {
        if (ItemSlot.ShiftInUse)
            TagItem.DrawTagTooltips(lines, TagItem.GenerateDetailedTags(Mod, lines), x, y);
        return true;
    }

    // (TileSort)(index+1)即为枚举对应家具的消耗量
    // 23为墙壁
    private readonly int[] MaterialConsume = new int[24];
    // 计算消耗
    private void CalculateConsume()
    {
        Array.Fill(MaterialConsume, 0);

        if (!_colorsLoaded || _colors is null)
        {
            ImproveGame.Instance.Logger.Error("Create Wand Colors didn't load. Please report to mod developers.");
            return;
        }

        TileInfo tileInfo;
        for (int i = 0; i < Colors.Length; i++)
        {
            tileInfo = Color2TileInfo(Colors[i]);
            if (tileInfo.Sort is not TileSort.None)
                MaterialConsume[(int)tileInfo.Sort - 1]++;
            if (tileInfo.HasWall)
                MaterialConsume[23]++;
        }
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        ((IItemMiddleClickable)this).HandleTooltips(Item, tooltips);

        CalculateConsume();
        tooltips.Add(new(Mod, "MaterialConsume", $"[c/ffff00:{GetText("Architecture.MaterialsRequired")}]"));


        for (int n = 0; n < 24; n++)
        {
            int count = MaterialConsume[n];
            if (count <= 0) continue;

            string key = n == 23 ? "Wall" : ((TileSort)(n + 1)).ToString();


            GetStoredItemInstance(key, out var storedItem);
            int stack = 0;
            if (storedItem is not null && !storedItem.IsAir)
            {
                stack = storedItem.stack;
            }

            string neededText = $"[c/ffff00:{GetText($"Architecture.{key}")}: {count}]";
            string hasText =
                $"[c/00a7df:{GetTextWith("Architecture.StoredMaterials", new { MaterialCount = stack })}]";

            tooltips.Add(new(Mod, $"MaterialConsume.{key}", $"{neededText}   {hasText}"));
        }

        tooltips.Add(new TooltipLine(Mod, "TagDetailed.CreateWand", GetText("Tips.TagDetailed.CreateWand")) { OverrideColor = Color.SkyBlue });
        TagItem.AddShiftForMoreTooltip(tooltips);
    }

}