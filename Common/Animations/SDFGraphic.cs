using ImproveGame.Assets;

namespace ImproveGame.Common.Animations
{
    internal static class SDFGraphic
    {
        private struct VertexPosCoord : IVertexType
        {
            private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[2]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            });

            public Vector2 Pos;
            public Vector2 Coord;

            public VertexPosCoord(Vector2 pos, Vector2 coord)
            {
                Pos = pos;
                Coord = coord;
            }

            public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        }

        private static void GetSDFVertexInfo(ref List<VertexPosCoord> vertices, Vector2 pos, Vector2 size)
        {
            vertices.Add(new VertexPosCoord(pos, new Vector2(0, 0)));
            vertices.Add(new VertexPosCoord(pos + new Vector2(size.X, 0), new Vector2(size.X, 0)));
            vertices.Add(new VertexPosCoord(pos + new Vector2(0, size.Y), new Vector2(0, size.Y)));
            vertices.Add(new VertexPosCoord(pos + new Vector2(0, size.Y), new Vector2(0, size.Y)));
            vertices.Add(new VertexPosCoord(pos + new Vector2(size.X, 0), new Vector2(size.X, 0)));
            vertices.Add(new VertexPosCoord(pos + size, size));
        }

        private static void BaseDraw(Vector2 pos, Vector2 size)
        {
            List<VertexPosCoord> vertices = new List<VertexPosCoord>();
            GetSDFVertexInfo(ref vertices, pos, size);
            VertexPosCoord[] triangles = vertices.ToArray();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangles, 0, triangles.Length / 3);
            ShaderAssets.SpriteEffectPass.Apply();
        }

        /// <summary>
        /// 绘制叉号
        /// </summary>
        public static void DrawCross(Vector2 pos, float size, float round, Color backgroundColor, float border,
            Color borderColor, bool ui = true)
        {
            Effect effect = ShaderAssets.Cross;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
            effect.Parameters["uBorder"].SetValue(border);
            effect.Parameters["uRound"].SetValue(round);
            effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
            effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();
            BaseDraw(pos, new Vector2(size));
        }

        public static void DrawRound(Vector2 pos, float size, Color background, bool ui = true)
        {
            Effect effect = ShaderAssets.Round;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["uSizeOver2"].SetValue(new Vector2(size) / 2f);
            effect.Parameters["uBackground"].SetValue(background.ToVector4());
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            BaseDraw(pos, new Vector2(size));
        }

        public static void DrawRound(Vector2 pos, float size, Color background, float border, Color borderColor,
            bool ui = true)
        {
            Effect effect = ShaderAssets.Round;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["size"].SetValue(size);
            effect.Parameters["background"].SetValue(background.ToVector4());
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor"].SetValue(borderColor.ToVector4());
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            BaseDraw(pos, new Vector2(size));
        }

        /// <summary>
        /// 绘制一条线，无边框
        /// </summary>
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color background, bool ui = true)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            start += new Vector2(width) - min;
            end += new Vector2(width) - min;
            Effect effect = ShaderAssets.Line;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["size"].SetValue(size);
            effect.Parameters["start"].SetValue(start);
            effect.Parameters["end"].SetValue(end);
            effect.Parameters["width"].SetValue(width);
            effect.Parameters["background"].SetValue(background.ToVector4());
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            BaseDraw(min - new Vector2(width), size);
        }

        /// <summary>
        /// 绘制一条线，有边框
        /// </summary>
        public static void DrawLine(Vector2 start, Vector2 end, float width, Color background, float border, Color borderColor, bool ui = true)
        {
            Vector2 min = Vector2.Min(start, end);
            Vector2 max = Vector2.Max(start, end);
            Vector2 size = max - min + new Vector2(width * 2);

            start += new Vector2(width) - min;
            end += new Vector2(width) - min;
            Effect effect = ShaderAssets.Line;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["size"].SetValue(size);
            effect.Parameters["start"].SetValue(start);
            effect.Parameters["end"].SetValue(end);
            effect.Parameters["width"].SetValue(width);
            effect.Parameters["border"].SetValue(border);
            effect.Parameters["borderColor"].SetValue(borderColor.ToVector4());
            effect.Parameters["background"].SetValue(background.ToVector4());
            effect.CurrentTechnique.Passes["HasBorder"].Apply();
            BaseDraw(min - new Vector2(width), size);
        }
    }
}
