using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ImproveGame.Entitys
{
    // 自定义顶点数据结构，注意这个结构体里面的顺序需要和shader里面的数据相同
    // 来自从零群
    public struct VertexInfo : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
        {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        });
        public Vector2 Position;
        public Color Color;
        public Vector3 TexCoord;

        public VertexInfo(Vector2 position, Color color, Vector3 texCoord) {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }

        public VertexDeclaration VertexDeclaration {
            get {
                return _vertexDeclaration;
            }
        }
    }
}
