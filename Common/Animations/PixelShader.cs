using ImproveGame.Assets;

namespace ImproveGame.Common.Animations
{
    public class PixelShader
    {
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

        private struct VertexPos : IVertexType
        {
            private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[2]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
            });

            public Vector2 Position;
            public Vector3 Coord;

            public VertexPos(Vector2 position, Vector3 coord)
            {
                Position = position;
                Coord = coord;
            }

            public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        }

        private static void GetVertexPos(List<VertexPos> vertexPos, Vector2 pos, Vector2 size, float rounded)
        {
            Vector2 absSize = size.Abs();
            vertexPos.Add(new VertexPos(pos, new Vector3(0, 0, rounded)));
            vertexPos.Add(new VertexPos(pos + new Vector2(size.X, 0), new Vector3(absSize.X, 0, rounded)));
            vertexPos.Add(new VertexPos(pos + new Vector2(0, size.Y), new Vector3(0, absSize.Y, rounded)));
            vertexPos.Add(new VertexPos(pos + new Vector2(0, size.Y), new Vector3(0, absSize.Y, rounded)));
            vertexPos.Add(new VertexPos(pos + new Vector2(size.X, 0), new Vector3(absSize.X, 0, rounded)));
            vertexPos.Add(new VertexPos(pos + size, new Vector3(absSize, rounded)));
        }

        private static void BaseDraw(Vector2 pos, Vector2 size, bool ui, Action<Matrix> action)
        {
            action.Invoke(GetMatrix(ui));
            List<VertexPos> vertexPos = new List<VertexPos>();
            GetVertexPos(vertexPos, pos, size, 0f);
            VertexPos[] triangles = vertexPos.ToArray();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangles, 0, triangles.Length / 3);
            EffectAssets.SpriteEffectPass.Apply();
        }

        private static void GetRectangleVertexInfo(List<VertexPos> vertexPos, Vector2 pos, Vector2 size, float rounded)
        {
            Vector2 absSize = size.Abs();
            Vector2 offset = rounded.ToXY() - absSize;
            Vector2 end = absSize + offset;
            vertexPos.Add(new VertexPos(pos, new Vector3(offset, rounded)));
            vertexPos.Add(new VertexPos(pos + size.ToX(), new Vector3(new Vector2(end.X, offset.Y), rounded)));
            vertexPos.Add(new VertexPos(pos + size.ToY(), new Vector3(new Vector2(offset.X, end.Y), rounded)));
            vertexPos.Add(new VertexPos(pos + size.ToY(), new Vector3(new Vector2(offset.X, end.Y), rounded)));
            vertexPos.Add(new VertexPos(pos + size.ToX(), new Vector3(new Vector2(end.X, offset.Y), rounded)));
            vertexPos.Add(new VertexPos(pos + size, new Vector3(end, rounded)));
        }

        private static void BaseDrawRoundedRectangle(Vector2 pos, Vector2 size, Vector4 rounded, bool ui, Action<Matrix> action)
        {
            Main.graphics.GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            action.Invoke(GetMatrix(ui));
            size /= 2f;
            pos += size;
            List<VertexPos> vertexPos = new List<VertexPos>();
            GetRectangleVertexInfo(vertexPos, pos, -size, rounded.X);
            GetRectangleVertexInfo(vertexPos, pos, new Vector2(-size.X, size.Y), rounded.Z);
            GetRectangleVertexInfo(vertexPos, pos, new Vector2(size.X, -size.Y), rounded.Y);
            GetRectangleVertexInfo(vertexPos, pos, size, rounded.W);
            VertexPos[] triangles = vertexPos.ToArray();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangles, 0, triangles.Length / 3);
            EffectAssets.SpriteEffectPass.Apply();
        }

        /// <summary>
        /// 绘制叉号
        /// </summary>
        public static void DrawCross(Vector2 pos, float size, float round, Color backgroundColor, float border,
            Color borderColor, bool ui = true)
        {
            BaseDraw(pos, size.ToXY(), ui, matrix =>
            {
                Effect effect = EffectAssets.Cross;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
                effect.Parameters["uBorder"].SetValue(border);
                effect.Parameters["uRound"].SetValue(round);
                effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.CurrentTechnique.Passes[0].Apply();
            });
        }

        public static void RoundedRectangle(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float border,
            Color borderColor, bool ui = true)
        {
            float innerShrinkage = 1;
            pos -= new Vector2(innerShrinkage);
            size += new Vector2(innerShrinkage * 2);
            rounded += new Vector4(innerShrinkage);
            BaseDrawRoundedRectangle(pos, size, rounded, ui, matrix =>
            {
                Effect effect = EffectAssets.RoundedRectangle;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.Parameters["uBorder"].SetValue(border);
                effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
                effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
                effect.CurrentTechnique.Passes["HasBorder"].Apply();
            });
        }

        public static void RoundedRectangle(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, bool ui = true)
        {
            float innerShrinkage = 1;
            pos -= new Vector2(innerShrinkage);
            size += new Vector2(innerShrinkage * 2);
            rounded += new Vector4(innerShrinkage);
            BaseDrawRoundedRectangle(pos, size, rounded, ui, matrix =>
            {
                Effect effect = EffectAssets.RoundedRectangle;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
                effect.CurrentTechnique.Passes["NoBorder"].Apply();
            });
        }

        public static void DrawShadow(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float shadow, bool ui = true)
        {
            BaseDrawRoundedRectangle(pos, size, rounded + new Vector4(shadow), ui, matrix =>
            {
                Effect effect = EffectAssets.RoundedRectangle;
                effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
                effect.Parameters["uShadowSize"].SetValue(shadow);
                effect.CurrentTechnique.Passes["Shadow"].Apply();
            });
        }

        public static void DrawRound(Vector2 pos, float size, Color background, bool ui = true)
        {
            BaseDraw(pos, size.ToXY(), ui, matrix =>
            {
                Effect effect = EffectAssets.Round;
                effect.Parameters["uTransform"].SetValue(matrix);
                effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
                effect.Parameters["uBackground"].SetValue(background.ToVector4());
                effect.CurrentTechnique.Passes["NoBorder"].Apply();
            });
        }

        public static void DrawRound(Vector2 pos, float size, Color background, float border, Color borderColor,
            bool ui = true)
        {
            BaseDraw(pos, size.ToXY(), ui, matrix =>
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