using ImproveGame.UI.ModernConfig.Categories;
using ImproveGame.UI.ModernConfig.FakeCategories;
using ImproveGame.UI.ModernConfig;
using ImproveGame.UIFramework.SUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace ImproveGame.UI.ModernConfig_Reflect;
public sealed partial class ModernReflectConfigUI : UIState
{
    //使用类中类，防止他人使用的时候混淆
    private class CategorySidePanelForModConfig : SUIPanel
    {

        private SUIScrollView2 Categories { get; set; }


        public CategorySidePanelForModConfig(Color color) : base(color, color)
        {
            Categories = new SUIScrollView2(Orientation.Vertical);
            Categories.SetPadding(0f, 0f);
            Categories.SetSize(0f, 0f, 1f, 1f);
            Categories.JoinParent(this);


        }
        public void SwitchMod(Mod mod)
        {
            Categories.RemoveAllChildren();
            if (mod == null || !ConfigManager.Configs.TryGetValue(mod, out var configs))
                return;
            var sortedConfigs = configs.OrderBy(x => Utils.CleanChatTags(x.DisplayName.Value)).ToList();
            foreach (var config in sortedConfigs)
            {
                new CategoryCard(config).JoinParent(Categories);
            }
        }
    }
}

