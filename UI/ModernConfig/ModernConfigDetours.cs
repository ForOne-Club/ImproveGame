using System.Reflection;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ImproveGame.UI.ModernConfig;

public class ModernConfigDetours : ILoadable
{
    public void Load(Mod mod)
    {
        var populateConfigsMethod = typeof(UIModConfigList).GetMethod(nameof(UIModConfigList.PopulateConfigs),
            BindingFlags.NonPublic | BindingFlags.Instance);
        MonoModHooks.Add(populateConfigsMethod, PopulateConfigsDetour);

        var drawMenuMethod = typeof(MenuLoader).GetMethod(nameof(MenuLoader.UpdateAndDrawModMenu),
            BindingFlags.NonPublic | BindingFlags.Static);
        MonoModHooks.Add(drawMenuMethod, DrawMenuDetour);

        On_Main.CanPauseGame += orig =>
            orig.Invoke() || (Main.netMode is NetmodeID.SinglePlayer && Main.InGameUI.CurrentState is ModernConfigUI);

        On_IngameFancyUI.Close += orig =>
        {
            // 现记录，因为执行完原版之后CurrentState就是null了
            bool isInGameModernConfig = !Main.gameMenu && Main.InGameUI.CurrentState is ModernConfigUI;

            orig.Invoke();

            if (!isInGameModernConfig)
                return;

            OperateInventory(false);
            ModernConfigUI.Instance.Enabled = false;
        };
    }

    private static void PopulateConfigsDetour(Action<UIModConfigList> orig, UIModConfigList self)
    {
        orig.Invoke(self);

        // 确保是自家的
        if (self.selectedMod?.Name is not "ImproveGame")
            return;
        // 确保里面有东西了
        if (self.configList.Count == 0)
            return;

        var configPanel = new UIButton<LocalizedText>(Language.GetText("Mods.ImproveGame.ModernConfig.Name"))
        {
            MaxWidth = { Percent = 0.95f },
            HAlign = 0.5f,
            ScalePanel = true,
            UseInnerDimensions = true,
            ClickSound = SoundID.MenuOpen,
        };
        configPanel.OnUpdate += delegate
        {
            configPanel.TextColor = Main.DiscoColor;
        };
        configPanel.OnLeftClick += delegate
        {
            ModernConfigUI.Instance.Open();
        };

        self.configList.Add(configPanel);
    }

    private static void DrawMenuDetour(Action<SpriteBatch, GameTime, Color, float, float> orig, SpriteBatch spriteBatch,
        GameTime gameTime, Color color, float logoRotation, float logoScale)
    {
        if (ModernConfigUI.Instance?.Enabled is true)
            logoScale = 0f;
        orig.Invoke(spriteBatch, gameTime, color, logoRotation, logoScale);
    }

    public void Unload()
    {
    }
}