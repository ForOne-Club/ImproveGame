using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace ImproveGame.Interface.UIElements
{
    public class JuButton : UIElement
    {
        private Asset<Texture2D> Background;
        private Asset<Texture2D> BackgroundBorder;

        public bool _playSound;

        public int[] data;
        public UIImage UIImage;
        public UIText UIText;

        public JuButton(Texture2D texture, string text) {
            _playSound = true;
            data = new int[5];
            Background = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/PanelGrayscale");
            BackgroundBorder = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelBorder");

            UIImage = new(texture) {
                VAlign = 0.5f
            };
            UIImage.Left.Pixels = 30f - UIImage.Width() / 2f;
            Append(UIImage);

            UIText = new(text) {
                VAlign = 0.5f
            };
            UIText.Left.Pixels = 50f;
            Append(UIText);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (IsMouseHovering) {
                if (_playSound) {
                    _playSound = false;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }
            else {
                _playSound = true;
            }
        }

        protected override void DrawSelf(SpriteBatch sb) {
            var rectangle = GetDimensions().ToRectangle();
            Utils.DrawSplicedPanel(sb, Background.Value, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 10, 10, 10, 10, Colors.InventoryDefaultColor);
            if (IsMouseHovering) {
                Utils.DrawSplicedPanel(sb, BackgroundBorder.Value, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 10, 10, 10, 10, Color.White);
            }
        }

        public void SetText(string text) {
            UIText.SetText(text);
        }

        public void SetImage(Texture2D texture) {
            UIImage.SetImage(texture);
            UIImage.Left.Pixels = 15 + UIImage.Width() / 2f;
        }
    }
}
