using ImproveGame.Common.Configs;
using ImproveGame.UIFramework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;

namespace ImproveGame.UI.RecipeSearch;

public class RecipeSearchSystem : ModSystem
{
    private static bool _oldRecBigList;
    
    public static bool UsingGuide => !Main.guideItem.IsAir && Main.guideItem.Name != "";

    public static bool SearchUnavailable(out RecipeSearchUI searchUI) =>
        !UISystem.TryGetBaseBody(out searchUI) || searchUI is null || string.IsNullOrWhiteSpace(searchUI.SearchContent)
        || UsingGuide || Main.screenWidth < 1180 || !UIConfigs.Instance.RecipeSearch;

    public static int DummyRecipeIndex;

    private static bool _addingSearchIcon;

    private static bool SearchConditionPredicate(int recipeIndex)
    {
        // 走后门
        if (_addingSearchIcon)
            return true;
        // 超界？怎么可能？但是还是判断一下
        if (!Main.recipe.IndexInRange(recipeIndex))
            return true;
        // 判断有没有在搜索
        if (SearchUnavailable(out var searchUI) || !Main.recBigList)
            return true;
        // 真正的判断
        return Main.recipe[recipeIndex].createItem.MatchWithString(searchUI.SearchContent, false);
    }

    public override void Load()
    {
        IL_Recipe.AddToAvailableRecipes += il =>
        {
            var c = new ILCursor(il);
            var label = c.DefineLabel(); // 记录位置
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Call, typeof(RecipeSearchSystem).GetMethod(nameof(SearchConditionPredicate),
                BindingFlags.Static | BindingFlags.NonPublic)!);
            c.Emit(OpCodes.Brtrue_S, label); // 为true就跳到下面
            c.Emit(OpCodes.Ret); // 为false直接return
            c.MarkLabel(label);
        };

        On_Recipe.TryRefocusingRecipe += (orig, recipe) =>
        {
            // 保证至少有一个配方显示，不然原版会直接关界面
            if (!SearchUnavailable(out _) && Main.numAvailableRecipes is 0 &&
                Main.recipe.IndexInRange(DummyRecipeIndex))
            {
                _addingSearchIcon = true;
                Recipe.AddToAvailableRecipes(DummyRecipeIndex);
                _addingSearchIcon = false;
            }

            orig.Invoke(recipe);
        };
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (_oldRecBigList != Main.recBigList && UIConfigs.Instance.RecipeSearch)
        {
            Recipe.FindRecipes();
            _oldRecBigList = Main.recBigList;
        }
    }
}