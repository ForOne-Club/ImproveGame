float4x4 uTransform;
float2 uSize; // 尺寸
float2 uSizeOver2; // 一半的大小
float4 uRounded;
float4 uBackgroundColor;
float uBorder; // 边框
float4 uBorderColor;
float uShadowSize;
float uInnerShrinkage;

struct VSInput
{
    float2 Pos : POSITION0;
    float2 Coord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float2 Coord : TEXCOORD0;
};

// -------------------------------
// ---------- 顶点着色器 ----------
// -------------------------------
PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Coord = input.Coord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float sdRoundSquare(float2 position, float2 sizeOver2, float roundedRadius)
{
    float2 q = abs(position) - sizeOver2 + roundedRadius;
    return min(max(q.x, q.y), 0) + length(max(q, 0)) - roundedRadius;
}

// 有边框 - 圆角矩形
float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 Rounded = coords.x < 0.5 ? uRounded.xz : uRounded.yw;
    Rounded.x = coords.y < 0.5 ? Rounded.x : Rounded.y;
    float2 p = coords * uSize - uSizeOver2;
    float Distance = sdRoundSquare(p, uSizeOver2, Rounded.x);
    return lerp(lerp(uBackgroundColor, uBorderColor, smoothstep(-1, 0.5, Distance + uBorder + uInnerShrinkage)), 0, smoothstep(-1, 0.5, Distance + uInnerShrinkage));
}

// 无边框 - 圆角矩形
float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 Rounded = coords.x < 0.5 ? uRounded.xz : uRounded.yw;
    Rounded.x = coords.y < 0.5 ? Rounded.x : Rounded.y;
    float2 p = coords * uSize - uSizeOver2;
    float Distance = sdRoundSquare(p, uSizeOver2, Rounded.x);
    return lerp(uBackgroundColor, 0, smoothstep(-1, 0.5, Distance + uInnerShrinkage));
}

// 圆角矩形 - 阴影
float4 Shadow(float2 coords : TEXCOORD0) : COLOR0
{
    float2 Rounded = coords.x < 0.5 ? uRounded.xz : uRounded.yw;
    Rounded.x = coords.y < 0.5 ? Rounded.x : Rounded.y;
    float2 p = coords * uSize - uSizeOver2;
    float Distance = sdRoundSquare(p, uSizeOver2, Rounded.x + uShadowSize);
    return lerp(uBackgroundColor, 0, smoothstep(-1 - uShadowSize, 0.5, Distance));
}

technique T1
{
    pass HasBorder
    {
        VertexShader = compile vs_3_0 VSFunction();
        PixelShader = compile ps_3_0 HasBorder();
    }

    pass NoBorder
    {
        VertexShader = compile vs_3_0 VSFunction();
        PixelShader = compile ps_3_0 NoBorder();
    }

    pass Shadow
    {
        VertexShader = compile vs_3_0 VSFunction();
        PixelShader = compile ps_3_0 Shadow();
    }
}