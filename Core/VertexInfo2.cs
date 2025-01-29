// 疑似国内每个Modder都在使用的顶点文件，赞美小裙子
namespace ImproveGame.Core
{
    public struct VertexInfo2 : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
        {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
        });
        /// <summary>
        /// 绘制位置(世界坐标)
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 绘制的颜色
        /// </summary>
        public Color Color;
        /// <summary>
        /// 前两个是纹理坐标，最后一个是自定义的
        /// </summary>
        public Vector3 TexCoord;
        public VertexInfo2(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
        public VertexDeclaration VertexDeclaration
        {
            get => _vertexDeclaration;
        }
    }
}
