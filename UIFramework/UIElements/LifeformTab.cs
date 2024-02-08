using ImproveGame.Content.Functions;
using ImproveGame.UIFramework.Common;
using ImproveGame.UIFramework.SUIElements;

namespace ImproveGame.UIFramework.UIElements
{
    public class LifeformTab : SUIPanel
    {
        private UIImage tickUI;
        private int _npcId;

        public LifeformTab(int npcId) : base(UIStyle.PanelBorderLight, UIStyle.PanelBg)
        {
            _npcId = npcId;
            this.SetSize(new Vector2(-20f, 36f), 1f);
            //IgnoresMouseInteraction = true;

            Append(new UITextPanel<string>($"{Lang.GetNPCNameValue(npcId)}")
            {
                IgnoresMouseInteraction = true,
                DrawPanel = false,
                Left = StyleDimension.FromPixels(-14f),
                VAlign = 0.5f
            });

            tickUI = new UIImage(TextureAssets.InventoryTickOn)
            {
                HAlign = 1f,
                VAlign = 0.5f
            };
            Append(tickUI);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (tickUI.IsMouseHovering)
            {
                bool hide = LifeAnalyzeCore.Blacklist.GetValueOrDefault(_npcId);
                Main.instance.MouseText(Language.GetTextValue($"LegacyInterface.{(hide ? "60" : "59")}"));
            }
        }

        public override void Update(GameTime gameTime)
        {
            bool hide = LifeAnalyzeCore.Blacklist.GetValueOrDefault(_npcId);
            tickUI.SetImage(hide ? TextureAssets.InventoryTickOff : TextureAssets.InventoryTickOn);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
            LifeAnalyzeCore.Blacklist[_npcId] = !LifeAnalyzeCore.Blacklist.GetValueOrDefault(_npcId);
        }
    }
}