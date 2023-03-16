using ImproveGame.Assets;
using ImproveGame.VertexTypes;

namespace ImproveGame.Common.Animations
{
    public class SDFRectangle
    {
        public static void TestDraw()
        {
            return;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;


            Vector2 pos = Main.MouseScreen;
            Vector2 size = new Vector2(100);

            List<VertexPosCoordRounded> vertices = new List<VertexPosCoordRounded>();
            AddRectangle(vertices, pos, size, size, Vector2.Zero, size.X - 1f);
            AddRectangle(vertices, pos + new Vector2(size.X, 0), size, new Vector2(0, size.Y), new Vector2(size.X, 0), size.X - 1f);
            AddRectangle(vertices, pos + new Vector2(0, size.Y), size, new Vector2(size.X, 0), new Vector2(0, size.Y), size.X - 1f);
            AddRectangle(vertices, pos + size, size, Vector2.Zero, size, size.X - 1f);

            Effect effect = ShaderAssets.SDFRectangle;
            effect.Parameters["uTransform"].SetValue(GetMatrix(true));
            effect.Parameters["uBorder"].SetValue(2);
            effect.Parameters["uBorderColor"].SetValue(Color.Red.ToVector4());
            effect.Parameters["uBackgroundColor"].SetValue(Color.Transparent.ToVector4());
            effect.CurrentTechnique.Passes["Test"].Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
        }

        public static void AddRectangle(List<VertexPosCoordRounded> vertices, Vector2 pos, Vector2 size, Vector2 coordBegin, Vector2 coordEnd, float rounded)
        {
            vertices.Add(new(pos, coordBegin, rounded));
            vertices.Add(new(pos.X + size.X, pos.Y, coordEnd.X, coordBegin.Y, rounded));
            vertices.Add(new(pos.X, pos.Y + size.Y, coordBegin.X, coordEnd.Y, rounded));
            vertices.Add(new(pos.X, pos.Y + size.Y, coordBegin.X, coordEnd.Y, rounded));
            vertices.Add(new(pos.X + size.X, pos.Y, coordEnd.X, coordBegin.Y, rounded));
            vertices.Add(new(pos + size, coordEnd, rounded));
        }

        public static void ModifyXXX(ref List<VertexPosCoord> vertices)
        {
            vertices.Add(new VertexPosCoord());
        }

        // 顶点信息 —— 矩形
        private static void GetVertexInfo(List<VertexPosCoordRounded> vertices, Vector2 pos, Vector2 size, float rounded)
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
            List<VertexPosCoordRounded> vertexPos = new List<VertexPosCoordRounded>();
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