using ImproveGame.QolUISystem.UI;

namespace ImproveGame.QolUISystem;

internal class MainSystem : ModSystem
{
    public readonly UIController Trigger = new UIController();

    public override void Load()
    {
        if (Main.dedServ) return;
        Trigger.UIManager = new MainManager();
        Trigger.UIManager.Initialize();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        Trigger.UpdateUI(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(match => match.Name is "Vanilla: Inventory");
        if (index >= 0)
        {
            index++;
            // 测试使用
            // layers.Insert(index, new ModLayer("Main", Trigger.TwoUIManager));
        }
    }
}
