using ImproveGame.Assets;

namespace ImproveGame.Common.Animations
{
    public class SDFRactangle
    {
        // 顶点着色器 —— 参数
        private struct VertexPositionTexture : IVertexType
        {
            private static readonly VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.Color, 0)
            });

            public Vector2 Position;
            public Vector2 Coord;
            public float Rounded;

            public VertexPositionTexture(Vector2 position, Vector2 coord, float rounded)
            {
                Position = position;
                Coord = coord;
                Rounded = rounded;
            }

            public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        }

        // 顶点信息 —— 矩形
        private static void GetVertexInfo(List<VertexPositionTexture> vertices, Vector2 pos, Vector2 size, float rounded)
        {
            Vector2 absSize = size.Abs();
            Vector2 offset = new Vector2(rounded) - absSize;
            Vector2 end = absSize + offset;
            vertices.Add(new(pos, offset, rounded));
            vertices.Add(new(pos + size.ToX(), new Vector2(end.X, offset.Y), rounded));
            vertices.Add(new(pos + size.ToY(), new Vector2(offset.X, end.Y), rounded));
            vertices.Add(new(pos + size.ToY(), new Vector2(offset.X, end.Y), rounded));
            vertices.Add(new(pos + size.ToX(), new Vector2(end.X, offset.Y), rounded));
            vertices.Add(new(pos + size, end, rounded));
        }

        private static void BaseDrawRectangle(Vector2 pos, Vector2 size, Vector4 rounded)
        {
            var oldCullMode = Main.graphics.GraphicsDevice.RasterizerState.CullMode;
            Main.graphics.GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            size /= 2f;
            pos += size;
            List<VertexPositionTexture> vertexPos = new List<VertexPositionTexture>();
            GetVertexInfo(vertexPos, pos, -size, rounded.X);
            GetVertexInfo(vertexPos, pos, new Vector2(-size.X, size.Y), rounded.Z);
            GetVertexInfo(vertexPos, pos, new Vector2(size.X, -size.Y), rounded.Y);
            GetVertexInfo(vertexPos, pos, size, rounded.W);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(0, vertexPos.ToArray(), 0, vertexPos.Count / 3);
            ShaderAssets.SpriteEffectPass.Apply();
            Main.graphics.GraphicsDevice.RasterizerState.CullMode = oldCullMode;
        }

        public static void HasBorder(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float border,
            Color borderColor, bool ui = true)
        {
            const float innerShrinkage = 1;
            pos -= new Vector2(innerShrinkage);
            size += new Vector2(innerShrinkage * 2);
            rounded += new Vector4(innerShrinkage);
            Effect effect = ShaderAssets.SDFRectangle;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
            effect.Parameters["uBorder"].SetValue(border);
            effect.Parameters["uBorderColor"].SetValue(borderColor.ToVector4());
            effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
            effect.CurrentTechnique.Passes["HasBorder"].Apply();
            BaseDrawRectangle(pos, size, rounded);
        }

        public static void NoBorder(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, bool ui = true)
        {
            const float innerShrinkage = 1;
            pos -= new Vector2(innerShrinkage);
            size += new Vector2(innerShrinkage * 2);
            rounded += new Vector4(innerShrinkage);
            Effect effect = ShaderAssets.SDFRectangle;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
            effect.Parameters["uInnerShrinkage"].SetValue(innerShrinkage);
            effect.CurrentTechnique.Passes["NoBorder"].Apply();
            BaseDrawRectangle(pos, size, rounded);
        }

        public static void Shadow(Vector2 pos, Vector2 size, Vector4 rounded, Color backgroundColor, float shadow, bool ui = true)
        {
            Effect effect = ShaderAssets.SDFRectangle;
            effect.Parameters["uTransform"].SetValue(GetMatrix(ui));
            effect.Parameters["uBackgroundColor"].SetValue(backgroundColor.ToVector4());
            effect.Parameters["uShadowSize"].SetValue(shadow);
            effect.CurrentTechnique.Passes["Shadow"].Apply();
            BaseDrawRectangle(pos, size, rounded + new Vector4(shadow));
        }
    }
}