using ImproveGame.Assets;
using Mono.Cecil;

namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
        private struct VertexPos : IVertexType
        {
            private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[2]
            {
                // Position
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                // Coord
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            });

            public Vector2 Position;
            public Vector2 Coord;

            public VertexPos(Vector2 position, Vector2 coord)
            {
                Position = position;
                Coord = coord;
            }

            public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        }

        private static VertexPos[] GetVertexPos(Vector2 pos, Vector2 size)
        {
            return new VertexPos[6]
            {
                new VertexPos(pos, new Vector2(0, 0)),
                new VertexPos(pos + new Vector2(size.X, 0),  new Vector2(1, 0)),
                new VertexPos(pos + new Vector2(0, size.Y),  new Vector2(0, 1)),
                new VertexPos(pos + new Vector2(0, size.Y),  new Vector2(0, 1)),
                new VertexPos(pos + new Vector2(size.X, 0),  new Vector2(1, 0)),
                new VertexPos(pos + size, new Vector2(1, 1))
            };
        }

        public static Matrix GetMatrix(bool ui)
        {
            if (ui)
            {
                return Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            }
            else
            {
                Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                Vector2 offset = screenSize * (Vector2.One - Vector2.One / Main.GameViewMatrix.Zoom) / 2;
                return Matrix.CreateOrthographicOffCenter(offset.X, Main.screenWidth - offset.X, Main.screenHeight - offset.Y, offset.Y, 0, 1);
            }
        }

        private static void BaseDraw(Vector2 pos, Vector2 size, bool ui, Action<Matrix> action)
        {
            VertexPos[] triangles = GetVertexPos(pos, size);
            action.Invoke(GetMatrix(ui));
            Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangles, 0, triangles.Length / 3);
            EffectAssets.SpriteEffectPass.Apply();
        }

        /// <summary>
        /// 绘制叉号
        /// </summary>
        public static void DrawCross(Vector2 pos, float size, float round, Color backgroundColor, float border,
            Color borderColor, bool ui = true)
        {
            BaseDraw(pos, size.ToVector2(), ui, matrix =>
            {
                Effect effect = EffectAssets.Cross;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["size"].SetValue(size);
                effect.Parameters["border"].SetValue(border);
                effect.Parameters["round"].SetValue(round);
                effect.Parameters["borderColor"].SetValue(borderColor.ToVector4());
                effect.Parameters["backgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();
            });
        }

        public static void RoundedRectangle(Vector2 pos, Vector2 size, Vector4 round4, Color backgroundColor, bool ui = true)
        {
            float innerShrinkage = 1;
            pos -= new Vector2(innerShrinkage);
            size += new Vector2(innerShrinkage * 2);
            round4 += new Vector4(innerShrinkage);
            BaseDraw(pos, size, ui, matrix =>
            {
                Effect effect = EffectAssets.RoundedRectangle;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["uSize"].SetValue(size);
                effect.Parameters["uSizeOver2"].SetValue(size / 2);
                effect.Parameters["uRounded"].SetValue(round4);
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
                effect.CurrentTechnique.Passes["NoBorder"].Apply();
            });
        }

        public static void RoundedRectangle(Vector2 pos, Vector2 size, Vector4 round4, Color backgroundColor, float border,
            Color borderColor, bool ui = true)
        {
            float innerShrinkage = 1;
            pos -= new Vector2(innerShrinkage);
            size += new Vector2(innerShrinkage * 2);
            round4 += new Vector4(innerShrinkage);
            BaseDraw(pos, size, ui, matrix =>
            {
                Effect effect = EffectAssets.RoundedRectangle;
                GraphicsDevice GraphicsDevice = Main.graphics.GraphicsDevice;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["uSize"].SetValue(size);
                effect.Parameters["uSizeOver2"].SetValue(size / 2);
                effect.Parameters["uRounded"].SetValue(round4);
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.Parameters["uBorder"].SetValue(border);
                effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
                effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
                effect.CurrentTechnique.Passes["HasBorder"].Apply();
            });
        }

        public static void DrawShadow(Vector2 pos, Vector2 size, Vector4 round, Color backgroundColor, float shadow, bool ui = true)
        {
            BaseDraw(pos, size, ui, matrix =>
            {
                Effect effect = EffectAssets.RoundedRectangle;
                effect.Parameters["uSize"].SetValue(size);
                effect.Parameters["uSizeOver2"].SetValue(size / 2);
                effect.Parameters["uRounded"].SetValue(round + new Vector4(shadow));
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.Parameters["uShadowSize"].SetValue(shadow);
                effect.CurrentTechnique.Passes["Shadow"].Apply();
            });
        }

        public static void DrawRound(Vector2 pos, float size, Color background, bool ui = true)
        {
            BaseDraw(pos, size.ToVector2(), ui, matrix =>
            {
                Effect effect = EffectAssets.Round;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["size"].SetValue(size);
                effect.Parameters["background"].SetValue(background.ToVector4());
                effect.CurrentTechnique.Passes["NoBorder"].Apply();
            });
        }

        public static void DrawRound(Vector2 pos, float size, Color background, float border, Color borderColor,
            bool ui = true)
        {
            BaseDraw(pos, size.ToVector2(), ui, matrix =>
            {
                Effect effect = EffectAssets.Round;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["size"].SetValue(size);
                effect.Parameters["background"].SetValue(background.ToVector4());
                effect.Parameters["border"].SetValue(border);
                effect.Parameters["borderColor"].SetValue(borderColor.ToVector4());
                effect.CurrentTechnique.Passes["NoBorder"].Apply();
            });
        }

        /// <summary>
        /// 绘制一条线，无边框
        /// </summary>
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color background, bool ui = true)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            BaseDraw(min - new Vector2(width), size, ui, matrix =>
            {
                start += new Vector2(width) - min;
                end += new Vector2(width) - min;
                Effect effect = EffectAssets.Line;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["size"].SetValue(size);
                effect.Parameters["start"].SetValue(start);
                effect.Parameters["end"].SetValue(end);
                effect.Parameters["width"].SetValue(width);
                effect.Parameters["background"].SetValue(background.ToVector4());
                effect.CurrentTechnique.Passes["NoBorder"].Apply();
            });
        }

        /// <summary>
        /// 绘制一条线，有边框
        /// </summary>
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, bool ui = true)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            BaseDraw(min - new Vector2(width), size, ui, matrix =>
            {
                start += new Vector2(width) - min;
                end += new Vector2(width) - min;
                Effect effect = EffectAssets.Line;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["size"].SetValue(size);
                effect.Parameters["start"].SetValue(start);
                effect.Parameters["end"].SetValue(end);
                effect.Parameters["width"].SetValue(width);
                effect.Parameters["border"].SetValue(border);
                effect.Parameters["borderColor"].SetValue(borderColor.ToVector4());
                effect.Parameters["background"].SetValue(background.ToVector4());
                effect.CurrentTechnique.Passes["HasBorder"].Apply();
            });
        }
    }
}