using ImproveGame.Assets;
using ImproveGame.Common;
using ImproveGame.Common.ModSystems;
using Terraria.UI.Chat;

namespace ImproveGame.UIFramework.UIElements;

public class LiquidWandSlot : ModImageButton
{
    // 自定义顶点数据结构，注意这个结构体里面的顺序需要和shader里面的数据相同
    // 来自从零群
    public struct VertexInfo(Vector2 position, Color color, Vector3 texCoord) : IVertexType
    {
        public Vector2 Position = position;
        public Color Color = color;
        public Vector3 TexCoord = texCoord;
        
        private static readonly VertexDeclaration Declaration = new([
            new VertexElement (0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement (8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement (12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        ]);

        public VertexDeclaration VertexDeclaration => Declaration;
    }

    public readonly short LiquidId;
    public readonly short BucketId;
    public readonly short SpongeId;
    private readonly float _hoverColor;
    private readonly float _normalColor;
    private float _liquidAmount;
    public float CurrentColor;
    public int HoverTimer;

    // 悬停到别的按钮时这个的百分比文本也要虚化
    public bool IsAltHovering;
    public float AltHoverTimer;

    public LiquidWandSlot(short liquidId, short bucketId, short spongeId, float hoverColor, float normalColor) : base(
        TextureAssets.Liquid[0], Color.White, Color.White)
    {
        LiquidId = liquidId;
        BucketId = bucketId;
        SpongeId = spongeId;
        _hoverColor = hoverColor;
        _normalColor = normalColor;
        Width.Set(42f, 0f);
        Height.Set(42f, 0f);
        SetHoverImage(ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/LiquidSlot/Highlight"));
        SetBackgroundImage(ModContent.Request<Texture2D>("ImproveGame/Assets/Images/UI/LiquidSlot/Border"));
    }

    /// <summary>
    /// 用其他地方的的液体给当前槽存液体
    /// </summary>
    /// <param name="amount">存液体的数量，这个值会根据可存数量的多少减少</param>
    public void StoreLiquid(ref byte amount)
    {
        // 液体添加量
        float addAmount = LiquidAmountToFloat(amount);
        // 还可以存入的液体量
        float amountAvailable = 1f - _liquidAmount;
        // 取相对少的，毕竟如果我可以给你0.5%，你却只剩下0.25%，我只能填给你0.25%
        float amountAddition = Math.Min(addAmount, amountAvailable);

        // 应用上去
        amount -= (byte)LiquidAmountToInt(amountAddition);
        _liquidAmount += amountAddition;

        // 有海绵全吸走
        if (SpongeExists)
            amount = 0;
    }

    /// <summary>
    /// 把当前槽的液体放出去，供应到其他地方
    /// </summary>
    /// <param name="amount">拿液体的数量，这个值会根据可存数量的多少增加</param>
    public void TakeLiquid(ref byte amount)
    {
        // 液体需求量，应是最大值即255减去当前值
        float needAmount = LiquidAmountToFloat(255 - amount);
        // 取相对少的，毕竟你要是太多我也拿不下，太少我也补不完
        float amountReduction = Math.Min(needAmount, _liquidAmount);

        amount += (byte)LiquidAmountToInt(amountReduction);
        _liquidAmount -= amountReduction;

        // 有桶全放
        if (BucketExists)
            amount = 255;
    }

    /// <summary>
    /// 如果能无限，返回物品ID，否则返回-1
    /// </summary>
    public int Infinite => WandSystem.AbsorptionMode switch
    {
        true when SpongeExists => SpongeId,
        false when BucketExists => BucketId,
        _ => -1
    };

    public bool BucketExists => LocalPlayerHasItemFast(BucketId);

    public bool SpongeExists => LocalPlayerHasItemFast("", SpongeId, ItemID.UltraAbsorbantSponge);

    public void SetLiquidAmount(float amount) => _liquidAmount = amount;

    public float GetLiquidAmount() => _liquidAmount;

    public override void Update(GameTime gameTime)
    {
        if (IsMouseHovering)
            HoverTimer++;
        else
            HoverTimer--;

        if (IsAltHovering)
            AltHoverTimer -= 0.1f;
        else
            AltHoverTimer += 0.1f;

        CurrentColor = MathHelper.SmoothStep(_normalColor, _hoverColor, HoverTimer / 15f);

        HoverTimer = Math.Clamp(HoverTimer, 0, 15);
        AltHoverTimer = Math.Clamp(AltHoverTimer, 0.5f, 1f);

        base.Update(gameTime);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = GetDimensions();

        #region 液体绘制 代码好乱我下次来写就看不懂了

        // 考虑边框
        float scale = (dimensions.Width - 4f) / 16f;
        Vector2 drawPosition = dimensions.Position() + new Vector2(2f, 2f);

        // 原帧数，就是拼凑起来该有的
        var originalFrame = Texture.Frame(17, 1, (int)Main.wFrame, 0); // 根据风速的动态效果
        originalFrame.Width = 16;
        int waterStyle = Main.waterStyle; // 根据环境的水样式
        // 岩浆和蜂蜜是没有帧的
        if (LiquidId != LiquidID.Water)
        {
            originalFrame = new(0, 0, 16, 16);
            waterStyle = LiquidId == LiquidID.Lava ? 1 : 11;
        }

        // 水面高度效果，Y坐标添加是向上取整
        drawPosition.Y += (int)Math.Ceiling(originalFrame.Height * (1 - _liquidAmount) * scale);
        originalFrame.Height = (int)(originalFrame.Height * _liquidAmount);

        var mainColor = new Color(CurrentColor, CurrentColor, CurrentColor);
        if (DrawColor is not null)
        {
            mainColor = DrawColor.Invoke();
        }

        int frame = LiquidId != LiquidID.Water ? 0 : (int)Main.wFrame;
        float bottomY = dimensions.Position().Y + dimensions.Height;

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        float alpha = 1f;
        if (LiquidId == LiquidID.Water)
        {
            for (int j = 0; j < 13; j++)
            {
                if (Main.IsLiquidStyleWater(j) && Main.liquidAlpha[j] > 0f && j != waterStyle)
                {
                    DrawLiquid(drawPosition, originalFrame, mainColor * Main.liquidAlpha[j], frame, j, scale, bottomY);
                    alpha = Main.liquidAlpha[waterStyle];
                }
            }
        }

        DrawLiquid(drawPosition, originalFrame, mainColor * alpha, frame, waterStyle, scale, bottomY);

        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);

        #endregion

        // 边框
        if (BackgroundTexture is not null)
        {
            spriteBatch.Draw(BackgroundTexture.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);
        }

        // 选中时的高光
        if (WandSystem.LiquidMode == LiquidId && BorderTexture is not null)
        {
            spriteBatch.Draw(BorderTexture.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);
        }

        // 绘制一个文本，液体剩余多少
        // 转换为百分数，保留后一位，来自: https://www.jianshu.com/p/3f88338bde60
        string text = $"{_liquidAmount:p1}";

        int itemId = Infinite;
        if (itemId is not -1)
        {
            text = "∞"; // 无限使用
            Main.instance.LoadItem(itemId);
            var tex = TextureAssets.Item[itemId].Value;
            spriteBatch.Draw(tex, dimensions.Center(), null, Color.White * 0.85f, 0f, tex.Size() / 2f, 1f,
                SpriteEffects.None, 0f);
        }

        Vector2 origin = FontAssets.DeathText.Value.MeasureString(text) / 2f;

        Vector2 stringPosition = dimensions.Position();
        stringPosition.X += dimensions.Width / 2f + 2f;
        stringPosition.Y += dimensions.Height + 22f;

        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value,
            text, stringPosition, Color.White * AltHoverTimer, 0f, origin, new Vector2(0.4f), -1, 1);
    }

    private void DrawTriangleLiquid(List<VertexInfo> triangleList, Texture2D liquidTexture, bool isSurface,
        float xCoordStart, float xCoordEnd)
    {
        var screenCenter = Main.ScreenSize.ToVector2() / 2f;
        var screenSize = Main.ScreenSize.ToVector2();
        var screenPos = screenCenter - screenSize / 2f;

        var projection = Matrix.CreateOrthographicOffCenter(0, screenSize.X, screenSize.Y, 0, 0, 1);
        var model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0));

        // 把变换和所需信息丢给shader
        float speed = LiquidId == LiquidID.Water ? 0.5f : 0.3f;
        int milliSeconds = (int)Main.gameTimeCache.TotalGameTime.TotalMilliseconds;
        float uTime = milliSeconds % 10000f / 10000f * speed;
        if (milliSeconds % 20000f > 10000f)
        {
            // 10-20s时反过来，这样就连续了（
            uTime = (1 - milliSeconds % 10000f / 10000f) * speed;
        }

        uTime += LiquidId * 0.3f; // 让每个槽看起来不一样

        float uWaveScale = _liquidAmount > 0.9f ? (1.0f - _liquidAmount) * 4f : 0.4f;
        ModAsset.LiquidSurface.Value.Parameters["uTransform"].SetValue(model * projection);
        ModAsset.LiquidSurface.Value.Parameters["uXStart"].SetValue(xCoordStart);
        ModAsset.LiquidSurface.Value.Parameters["uXEnd"].SetValue(xCoordEnd);
        ModAsset.LiquidSurface.Value.Parameters["uTime"].SetValue(uTime);
        ModAsset.LiquidSurface.Value.Parameters["uWaveScale"].SetValue(uWaveScale);

        Main.instance.GraphicsDevice.Textures[0] = liquidTexture;
        // 用柏林噪声实现水波纹，如果不是水面，就放张全白的MagicPixel给你
        Main.instance.GraphicsDevice.Textures[1] =
            isSurface ? ShaderAssets.Perlin.Value : TextureAssets.MagicPixel.Value;
        ModAsset.LiquidSurface.Value.CurrentTechnique.Passes[0].Apply();

        Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0,
            triangleList.Count / 3);
    }

    private List<VertexInfo> PrepareTriangleList(Rectangle source, Color mainColor, int frame, float onePixelX,
        float minY, float maxY, bool isSurface, out float xCoordStart, out float xCoordEnd)
    {
        List<VertexInfo> bars = new();

        // 切换帧数
        float oneFrameLength = onePixelX * 18f; // 每帧长度
        float oneFrameActual = onePixelX * 16f; // 每帧实际上有贴图的部分长度
        xCoordStart = oneFrameLength * frame; // 根据当前帧加上帧长度
        xCoordEnd = xCoordStart + oneFrameActual;

        // 矩形描出框
        Vector2 topPos = new(source.X, source.Y);
        if (isSurface)
        {
            // 防止液体出框或者没液体出水波，还要判断_liquidAmount，不是表面也没有偏移
            if (_liquidAmount > 0.9f)
                topPos.Y -= MathHelper.Lerp(0f, 5f, (1.0f - _liquidAmount) * 10f);
            else if (_liquidAmount < 0.1f)
                topPos.Y -= MathHelper.Lerp(0f, 5f, _liquidAmount * 10f);
            else
                topPos.Y -= 5f;
        }

        Vector2 bottomPos = new(source.X, source.Y + source.Height);
        bars.Add(new VertexInfo(topPos, mainColor, new(xCoordStart, minY, 0)));
        bars.Add(new VertexInfo(bottomPos, mainColor, new(xCoordStart, maxY, 0)));

        topPos.X += source.Width;
        bottomPos.X += source.Width;
        bars.Add(new VertexInfo(topPos, mainColor, new(xCoordEnd, minY, 0)));
        bars.Add(new VertexInfo(bottomPos, mainColor, new(xCoordEnd, maxY, 0)));

        List<VertexInfo> triangleList = new();

        if (bars.Count > 2)
        {
            // 按照顺序连接三角形
            for (int i = 0; i < bars.Count - 2; i += 2)
            {
                // 这是一个四边形 [i] [i+2] [i+1] [i+3]
                triangleList.Add(bars[i]);
                triangleList.Add(bars[i + 2]);
                triangleList.Add(bars[i + 1]);

                triangleList.Add(bars[i + 1]);
                triangleList.Add(bars[i + 2]);
                triangleList.Add(bars[i + 3]);
            }
        }

        return triangleList;
    }

    public void DrawLiquid(Vector2 drawPosition, Rectangle originalFrame, Color mainColor, int frame, int waterStyle,
        float scale, float bottomY)
    {
        // -------------------------- 水面部分 --------------------------
        // 水面绘制矩形（矩形涉及的部分就是水面被绘制上去的部分）
        float surfaceDrawHeight = Math.Min(bottomY - drawPosition.Y, 10);
        Rectangle surfaceRectangle = new((int)drawPosition.X,
            (int)drawPosition.Y,
            (int)(originalFrame.Width * scale) + 1,
            (int)surfaceDrawHeight);

        float onePixelX = 1f / TextureAssets.Liquid[waterStyle].Width();
        float onePixelY = 1f / TextureAssets.Liquid[waterStyle].Height();
        var surfaceTriangle = PrepareTriangleList(surfaceRectangle, mainColor, frame, onePixelX, 0f,
            onePixelY * surfaceDrawHeight, true, out float xCoordStart, out float xCoordEnd);
        DrawTriangleLiquid(surfaceTriangle, TextureAssets.Liquid[waterStyle].Value, true, xCoordStart, xCoordEnd);

        // -------------------------- 水面部分 --------------------------
        // 水下绘制矩形（矩形涉及的部分就是水下被绘制上去的部分）(有一部分重叠起来防止空隙）
        Rectangle deepRectangle = new((int)drawPosition.X,
            (int)drawPosition.Y + surfaceRectangle.Height - 2,
            (int)(originalFrame.Width * scale), // 有一部分重叠起来防止空隙
            (int)(originalFrame.Height * scale) - surfaceRectangle.Height + 4);
        if (deepRectangle.Height > 0)
        {
            var deepTriangle = PrepareTriangleList(deepRectangle, mainColor, frame, onePixelX, onePixelY * 10f, 1f,
                false, out float xStart, out float xEnd);
            DrawTriangleLiquid(deepTriangle, TextureAssets.Liquid[waterStyle].Value, false, xStart, xEnd);
        }
    }
}