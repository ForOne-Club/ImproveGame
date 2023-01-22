float2 size; // 尺寸
float round; // 圆角
float4 round4;
float border; // 边框
float4 border4;
float4 borderColor;
float4 backgroundColor;
float4 bgColor1;
float4 bgColor2;
float shadow;
float progress;
float innerShrinkage;

float4 white = 1;
float4 black = float4(0, 0, 0, 1);
float4 blackHalf = float4(0, 0, 0, 0.5);
float4 transparent = float4(0, 0, 0, 0);

// PI * 2 double: 6.283185307179586
// PI     double: 3.141592653589793
// PI / 2 double: 1.5707963267948966
// PI / 4 double: 0.7853981633974483
float twoPi = 6.2831853;
float pi = 3.1415926;
float piOver2 = 1.5707963;
float piOver4 = 0.78539816;

// PI / 2 double: 0.5522847498307933
float h90 = 0.55228475;

float roundRadius;
float roundDiameter;
float roundedRadius;
float2 sizeOver2;
float interval;
float intervalOver2;
float extraCurve;
float extraLength;

float4x4 uTransform;

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

// -------------------------------
// ---------- 顶点着色器 ----------
// -------------------------------
PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

// 三阶贝塞尔参数方程
float2 Bezier3(float t, float2 A, float2 B, float2 C, float2 D)
{
    return A * pow(1 - t, 3) + B * 3 * pow(1 - t, 2) * t + C * 3 * (1 - t) * pow(t, 2) + D * pow(t, 3);
}

float h(float curve)
{
    return ((1 - cos(curve / 2)) / sin(curve / 2)) * 4 / 3;
}

float sdRoundSquare(float2 position, float2 sizeOver2, float roundedRadius)
{
    float2 q = abs(position) - sizeOver2 + roundedRadius;
    return min(max(q.x, q.y), 0) + length(max(q, 0)) - roundedRadius;
}

// 关于三元表达式: https://qa.1r1g.com/sf/ask/343798031/
// 关于HLSL: https://www.cnblogs.com/kekec/p/14762727.html
float4 DashedRound(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pos = coords * size - sizeOver2;
    float curve = atan2(pos.x, pos.y);
    curve += curve < 0 ? twoPi : 0;
    float4 color = (curve + extraCurve) % interval < intervalOver2 ? white : transparent;
    return lerp(lerp(transparent, color, length(pos) < roundRadius - 2 ? 0 : 1), transparent, length(pos) < roundRadius ? 0 : 1);
}

// 圆角矩形：有边框。
float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords * size - sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, round);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, Distance + border + innerShrinkage)), 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 圆角矩形：有边框。单独设置：圆角。
float4 HasBorderR4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yw, round4.xz, step(coords.x, 0.5));
    r4.x = lerp(r4.y, r4.x, step(coords.y, 0.5));
    float2 p = coords * size - sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, r4.x);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, Distance + border + innerShrinkage)), 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 圆角矩形：有边框。单独设置：圆角、边框。
float4 HasBorderR4B4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yw, round4.xz, step(coords.x, 0.5));
    r4.x = lerp(r4.y, r4.x, step(coords.y, 0.5));
    float2 p = coords * size - sizeOver2;
    float2 xy = float2(coords.x < 0.5 ? border4.x : border4.z, coords.y < 0.5 ? border4.y : border4.w);
    float rotation = smoothstep(pi * 1.5, pi * 2, 0);
    float b4 = lerp(xy.y, xy.x, rotation);
    float Distance = sdRoundSquare(p, sizeOver2, r4.x);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, Distance + ((p.x > p.y ? xy.y : xy.x)) + innerShrinkage)),
    0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 有边框，进度条，左右
float4 HasBorderProgressBarLeftRight(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords * size;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * size.x + border, p.x));
    p -= sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + innerShrinkage)), 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 有边框，进度条，左右，向内挤压
float4 HasBorderProgressBarLeftRight2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = abs(coords * size - sizeOver2);
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * sizeOver2.x + border, abs(p.x - sizeOver2.x)));
    float Distance = sdRoundSquare(p, sizeOver2, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + innerShrinkage)), 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 有边框，进度条，上下
float4 HasBorderProgressBarTopBottom(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords * size;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * size.y + border, p.y));
    p -= sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + innerShrinkage)), 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 有边框，进度条，上下，向内挤压
float4 HasBorderProgressBarTopBottom2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords * size - sizeOver2;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * sizeOver2.y + border, abs(p.y - sizeOver2.y)));
    float Distance = sdRoundSquare(p, sizeOver2, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + innerShrinkage)), 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 无边框圆角矩形
float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords * size - sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, round);
    return lerp(backgroundColor, 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 无边框圆角矩形，四个圆角单独设置大小
float4 NoBorderR4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yw, round4.xz, step(coords.x, 0.5));
    r4.x = lerp(r4.y, r4.x, step(coords.y, 0.5));
    float2 p = coords * size - sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, r4.x);
    return lerp(backgroundColor, 0, smoothstep(-1, 0.5, Distance + innerShrinkage));
}

// 圆角矩形的阴影
float4 Shadow(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = coords * size - sizeOver2;
    float Distance = sdRoundSquare(p, sizeOver2, round + shadow);
    return lerp(0, backgroundColor, pow(1 - smoothstep(-1 - shadow, 0.5, Distance), 2));
}

technique Technique1
{
    pass DashedRound
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 DashedRound();
    }

    pass HasBorder
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorder();
    }

    pass HasBorderR4
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorderR4();
    }

    pass HasBorderR4B4
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorderR4B4();
    }

    pass HasBorderProgressBarLeftRight
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorderProgressBarLeftRight();
    }

    pass HasBorderProgressBarLeftRight2
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorderProgressBarLeftRight2();
    }

    pass HasBorderProgressBarTopBottom
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorderProgressBarTopBottom();
    }

    pass HasBorderProgressBarTopBottom2
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 HasBorderProgressBarTopBottom2();
    }

    pass NoBorder
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 NoBorder();
    }

    pass NoBorderR4
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 NoBorderR4();
    }

    pass Shadow
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 Shadow();
    }
}