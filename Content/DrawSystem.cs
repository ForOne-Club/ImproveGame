using ImproveGame.Entitys;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ImproveGame.Content
{
    /// <summary>
    /// 绘制方框
    /// </summary>
    public class DrawSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Ruler"));
            if (MouseTextIndex != -1)
            {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
                    "ImproveGame: BorderRect",
                    delegate
                    {
                        DrawBorderRect();
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }

        public static readonly Box[] boxs = new Box[10];

        private static void DrawBorderRect()
        {
            for (int i = 0; i < boxs.Length; i++)
            {
                if (boxs[i] != null)
                {
                    Box box = boxs[i];
                    if (box.PreView is not null)
                    {
                        Main.spriteBatch.Draw(box.PreView, new Vector2(box.X, box.Y) * 16f - Main.screenPosition, null, Color.White * 0.5f, 0, Vector2.Zero, 1f, 0, 0);
                    }

                    box.Draw();

                    string text = "";
                    if (box.ShowWidth)
                        text = box.Width.ToString();
                    if (box.ShowHeight)
                        text = box.ShowWidth ? $"{box.Width}×{box.Height}" : box.Height.ToString();
                    Vector2 size = FontAssets.MouseText.Value.MeasureString(box.Width.ToString()) * 1.2f;
                    Vector2 position = Main.MouseScreen + new Vector2(16, -size.Y + 6);
                    Utils.DrawBorderString(Main.spriteBatch, text, position, box.borderColor, 1.2f);

                    boxs[i] = null;
                }
            }
        }
    }
}
