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

float sdRoundSquare(float2 p, float2 s, float r)
{
    float2 q = abs(p) - s + r;
    return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r;
}

// 圆角矩形：有边框。
float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = sdRoundSquare(p, s, round);
    // distance(p, min(p, s - round));
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, Distance + border + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 圆角矩形：有边框。单独设置：圆角。
float4 HasBorderR4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yw, round4.xz, step(coords.x, 0.5));
    r4.x = lerp(r4.y, r4.x, step(coords.y, 0.5));
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = sdRoundSquare(p, s, r4.x);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, Distance + border + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 圆角矩形：有边框。单独设置：圆角、边框。
float4 HasBorderR4B4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yw, round4.xz, step(coords.x, 0.5));
    r4.x = lerp(r4.y, r4.x, step(coords.y, 0.5));
    float2 s = size / 2;
    float2 p = coords * size - s;
    float Distance = sdRoundSquare(p, s, r4.x);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, Distance + b4 + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 有边框，进度条，左右
float4 HasBorderProgressBarLeftRight(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = coords * size;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * size.x + border, p.x));
    p = abs(p - s);
    float Distance = sdRoundSquare(p, s, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 有边框，进度条，左右，向内挤压
float4 HasBorderProgressBarLeftRight2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * s.x + border, abs(p.x - s.x)));
    float Distance = sdRoundSquare(p, s, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 有边框，进度条，上下
float4 HasBorderProgressBarTopBottom(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = coords * size;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * size.y + border, p.y));
    p = abs(p - s);
    float Distance = sdRoundSquare(p, s, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 有边框，进度条，上下，向内挤压
float4 HasBorderProgressBarTopBottom2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * s.y + border, abs(p.y - s.y)));
    float Distance = sdRoundSquare(p, s, round);
    return lerp(lerp(bgColor, borderColor, smoothstep(-1, 0.5, Distance + border + 1)), 0, smoothstep(-1, 0.5, Distance + 1));
}

// 无边框圆角矩形
float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = sdRoundSquare(p, s, round);
    return lerp(backgroundColor, 0, smoothstep(-1, 0.5, Distance + 1));
}

// 无边框圆角矩形，四个圆角单独设置大小
float4 NoBorderR4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yw, round4.xz, step(coords.x, 0.5));
    r4.x = lerp(r4.y, r4.x, step(coords.y, 0.5));
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = sdRoundSquare(p, s, r4.x);
    return lerp(backgroundColor, 0, smoothstep(-1, 0.5, Distance + 1));
}

// 圆角矩形的阴影
float4 Shadow(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    float Distance = distance(p, min(p, s - round - shadow));
    return lerp(0, backgroundColor, pow(1 - smoothstep(-1 - shadow, 0.5, Distance - shadow), 2));
}

technique Technique1
{
    pass HasBorder
    {
        PixelShader = compile ps_3_0 HasBorder();
    }

    pass HasBorderR4
    {
        PixelShader = compile ps_3_0 HasBorderR4();
    }

    pass HasBorderR4B4
    {
        PixelShader = compile ps_3_0 HasBorderR4B4();
    }

    pass HasBorderProgressBarLeftRight
    {
        PixelShader = compile ps_3_0 HasBorderProgressBarLeftRight();
    }

    pass HasBorderProgressBarLeftRight2
    {
        PixelShader = compile ps_3_0 HasBorderProgressBarLeftRight2();
    }

    pass HasBorderProgressBarTopBottom
    {
        PixelShader = compile ps_3_0 HasBorderProgressBarTopBottom();
    }

    pass HasBorderProgressBarTopBottom2
    {
        PixelShader = compile ps_3_0 HasBorderProgressBarTopBottom2();
    }

    pass NoBorder
    {
        PixelShader = compile ps_3_0 NoBorder();
    }

    pass NoBorderR4
    {
        PixelShader = compile ps_3_0 NoBorderR4();
    }

    pass Shadow
    {
        PixelShader = compile ps_3_0 Shadow();
    }
}