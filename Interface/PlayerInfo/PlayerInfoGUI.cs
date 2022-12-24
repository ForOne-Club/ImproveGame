using ImproveGame.Interface.Common;
using ImproveGame.Interface.PlayerInfo.UIElements;
using ImproveGame.Interface.SUIElements;
using Terraria.GameInput;

namespace ImproveGame.Interface.PlayerInfo
{
    public class PlayerInfoGUI : UIState
    {
        public static bool Visible;
        public SUIPanel mainPanel;
        public override void OnInitialize()
        {
            Append(mainPanel = new SUIPanel(UIColor.PanelBorder, UIColor.PanelBackground)
            {
                Draggable = true
            });
            var damage = Main.LocalPlayer.GetDamage(DamageClass.Melee);
            mainPanel.Width.Pixels = mainPanel.HPadding() + PlayerTip.w * 2 + 10f;
            mainPanel.Height.Pixels = mainPanel.VPadding() + PlayerTip.h * 5 + 10f * 4;
            mainPanel.Append(new PlayerTip(() => $"幸运值: {Main.LocalPlayer.luck}"));
            mainPanel.Append(new PlayerTip(() => $"仇恨值: {Main.LocalPlayer.aggro}"));
            mainPanel.Append(new PlayerTip(() => $"近战暴击率: {Main.LocalPlayer.GetTotalCritChance(DamageClass.Melee)}"));
            mainPanel.Append(new PlayerTip(() => $"远程暴击率: {Main.LocalPlayer.GetTotalCritChance(DamageClass.Ranged)}"));
            mainPanel.Append(new PlayerTip(() => $"魔法暴击率: {Main.LocalPlayer.GetTotalCritChance(DamageClass.Magic)}"));
            mainPanel.Append(new PlayerTip(() => $"仇恨值: {Main.LocalPlayer.aggro}"));
            mainPanel.Append(new PlayerTip(() => $"飞行时间: {Main.LocalPlayer.wingTime}"));
            mainPanel.Append(new PlayerTip(() => $"最大飞行时间: {Main.LocalPlayer.wingTimeMax}"));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (mainPanel.IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ImproveGame: PlayerInfo GUI");
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
