using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using ImproveGame.UI.UIElements;

namespace ImproveGame.UI.ArchitectureUI
{
	public class ArchitectureGUI : UIState
	{
		public static bool Visible { get; private set; }
		private static float panelLeft;
		private static float panelWidth;
		private static float panelTop;
		private static float panelHeight;

		private const float InventoryScale = 0.85f;

		private bool Dragging;
		private Vector2 Offset;

		private UIPanel basePanel;
		private ModItemSlot itemSlot;

		public override void OnInitialize() {
			panelLeft = 300f;
			panelTop = 300f;
			panelHeight = 100f;
			panelWidth = 100f;

			basePanel = new UIPanel();
			basePanel.Left.Set(panelLeft, 0f);
			basePanel.Top.Set(panelTop, 0f);
			basePanel.Width.Set(panelWidth, 0f);
			basePanel.Height.Set(panelHeight, 0f);
			basePanel.OnMouseDown += DragStart;
			basePanel.OnMouseUp += DragEnd;
			Append(basePanel);

			itemSlot = new ModItemSlot(InventoryScale);
			itemSlot.Left.Set(10f, 0f);
			itemSlot.Top.Set(10f, 0f);
			itemSlot.Width.Set(40f, 0f);
			itemSlot.Height.Set(40f, 0f);
			basePanel.Append(itemSlot);
		}

		private void DragStart(UIMouseEvent evt, UIElement listeningElement) {
			var dimensions = listeningElement.GetDimensions().ToRectangle();
			Offset = new Vector2(evt.MousePosition.X - dimensions.Left, evt.MousePosition.Y - dimensions.Top);
			Dragging = true;
		}

		private void DragEnd(UIMouseEvent evt, UIElement listeningElement) {
			Vector2 end = evt.MousePosition;
			Dragging = false;

			listeningElement.Left.Set(end.X - Offset.X, 0f);
			listeningElement.Top.Set(end.Y - Offset.Y, 0f);

			listeningElement.Recalculate();
		}


		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (!Main.playerInventory && Visible) {
				Close();
			}

			if (Dragging) {
				basePanel.Left.Set(Main.mouseX - Offset.X, 0f);
				basePanel.Top.Set(Main.mouseY - Offset.Y, 0f);
				Recalculate();
			}
		}

		public override void Draw(SpriteBatch spriteBatch) {
			Player player = Main.LocalPlayer;

			//Initialize();

			//basePanel.Draw(spriteBatch);
			base.Draw(spriteBatch);

			if (basePanel.ContainsPoint(Main.MouseScreen)) {
				player.mouseInterface = true;
			}
		}

		public static void Open() {
			Main.playerInventory = true;
			Visible = true;
			SoundEngine.PlaySound(SoundID.MenuOpen);
			Recipe.FindRecipes();
		}

		public static void Close() {
			Visible = false;
			Main.blockInput = false;
			SoundEngine.PlaySound(SoundID.MenuClose);
			Recipe.FindRecipes();
		}
	}
}
