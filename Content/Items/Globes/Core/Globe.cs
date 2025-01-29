namespace ImproveGame.Content.Items.Globes.Core;

/// <summary>
/// 球基类
/// </summary>
public abstract class Globe : ModItem
{
    /// <summary>
    /// 管理生态球的 ModItem.Type -> Projection.Type 的映射
    /// </summary>
    public static Dictionary<int, int> GlobeLookup = new();

    protected LocalizedText GetLocalizedText(string suffix) =>
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
        Item.DefaultToThrownWeapon(GlobeLookup.GetValueOrDefault(Type), 20, 8f, hasAutoReuse: true);
        Item.UseSound = SoundID.Item106;
        Item.width = 32;
        Item.height = 32;
        Item.noUseGraphic = true;
        Item.rare = rarity;
        Item.value = itemValue;
    }

    public abstract bool RevealOperation(Projectile projectile, bool onlyJudging);

    protected abstract Recipe AddCraftingMaterials(Recipe recipe);

    public override void AddRecipes()
    {
        var r = CreateRecipe();
        AddCraftingMaterials(r);
        r.Register();
    }
}