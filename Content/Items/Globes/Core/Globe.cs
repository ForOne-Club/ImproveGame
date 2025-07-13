using ImproveGame.Common.Conditions;
using ImproveGame.Common.GlobalItems;
using ImproveGame.Packets.Notifications;
using ImproveGame.Packets.WorldFeatures;
using Terraria.Chat;
using Terraria.DataStructures;

namespace ImproveGame.Content.Items.Globes.Core;

/// <summary>
/// 球基类
/// </summary>
public abstract class Globe : ModItem, IConditionItem
{
    public Condition UseCondition => ConfigCondition.EnableMinimapMarkC;

    public static Color hintTextColor = Color.PaleVioletRed * 1.4f;
    public static Color foundColor = Color.Pink;

    /// <summary>
    /// 管理生态球的 ModItem.Type -> Projection.Type 的映射
    /// </summary>
    public static Dictionary<int, int> GlobeLookup = [];

    public LocalizedText GetLocalizedText(string suffix) =>
        Language.GetText($"Mods.ImproveGame.Items.GlobeBase.{suffix}")
            .WithFormatArgs(this.GetLocalizedValue("BiomeName"));

    public override LocalizedText DisplayName => GetLocalizedText(nameof(DisplayName));

    public override LocalizedText Tooltip => GetLocalizedText(nameof(Tooltip));

    private readonly int rarity;
    private readonly int itemValue;

    public Globe() { }

    public Globe(int rare, int itemValue)
    {
        this.rarity = rare;
        this.itemValue = itemValue;
    }

    public override void SetDefaults()
    {
        Item.DefaultToThrownWeapon(GlobeLookup.GetValueOrDefault(Type), 20, 12f, hasAutoReuse: true);
        Item.DamageType = DamageClass.Default;
        Item.UseSound = SoundID.Item106;
        Item.width = 32;
        Item.height = 32;
        Item.noUseGraphic = true;
        Item.rare = rarity;
        Item.value = itemValue;
    }
    protected abstract Recipe AddCraftingMaterials(Recipe recipe);

    public override void AddRecipes()
    {
        var r = CreateRecipe();
        AddCraftingMaterials(r);
        r.Register();
    }
}
/// <summary>
/// 球基类，但是Tooltip会说明一个世界有多个结构，使用一次显示一个
/// </summary>
public abstract class GlobePlentyTooltip(int rare, int itemValue) : Globe(rare, itemValue)
{
    public override LocalizedText Tooltip => GetLocalizedText(nameof(Tooltip) + "_Multi");
}
