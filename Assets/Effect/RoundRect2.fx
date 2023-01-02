float2 size; // 尺寸
float round; // 圆角
float4 round4;
float border; // 边框
float4 borderColor;
float4 backgroundColor;
float4 bgColor1;
float4 bgColor2;
float shadow;
float progress;

// step x ≤ y => 1 , x ＞ y => 0

// 有边框，圆角矩形
float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = distance(p, min(p, s - round));
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-0.75, 0.25, Distance - round + border + 1)), 0, smoothstep(-0.75, 0.25, Distance - round + 1));
}

// 有边框，进度条，左右
float4 HasBorderProgressBarLeftRight(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = coords * size;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * size.x + border, p.x));
    p = abs(p - s);
    float Distance = distance(p, min(p, s - round));
    return lerp(lerp(bgColor, borderColor, smoothstep(-0.75, 0.25, Distance - round + border + 1)), 0, smoothstep(-0.75, 0.25, Distance - round + 1));
}

// 有边框，进度条，左右，向内挤压
float4 HasBorderProgressBarLeftRight2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * s.x + border, abs(p.x - s.x)));
    float Distance = distance(p, min(p, s - round));
    return lerp(lerp(bgColor, borderColor, smoothstep(-0.75, 0.25, Distance - round + border + 1)), 0, smoothstep(-0.75, 0.25, Distance - round + 1));
}

// 有边框，进度条，上下
float4 HasBorderProgressBarTopBottom(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = coords * size;
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * size.y + border, p.y));
    p = abs(p - s);
    float Distance = distance(p, min(p, s - round));
    return lerp(lerp(bgColor, borderColor, smoothstep(-0.75, 0.25, Distance - round + border + 1)), 0, smoothstep(-0.75, 0.25, Distance - round + 1));
}

// 有边框，进度条，上下，向内挤压
float4 HasBorderProgressBarTopBottom2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float4 bgColor = lerp(bgColor1, bgColor2, step(progress * s.y + border, abs(p.y - s.y)));
    float Distance = distance(p, min(p, s - round));
    return lerp(lerp(bgColor, borderColor, smoothstep(-0.75, 0.25, Distance - round + border + 1)), 0, smoothstep(-0.75, 0.25, Distance - round + 1));
}

// 无边框圆角矩形
float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = distance(p, min(p, s - round));
    return lerp(backgroundColor, 0, smoothstep(-0.75, 0.25, Distance - round + 1));
}

// 无边框圆角矩形，四个圆角单独设置大小
float4 NoBorderR4(float2 coords : TEXCOORD0) : COLOR0
{
    float2 r4 = lerp(round4.yz, round4.xw, step(coords.x, 0.5));
    r4.x = lerp(r4.x, r4.y, step(coords.y, 0.5));
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float Distance = distance(p, min(p, s - r4.x));
    return lerp(backgroundColor, 0, smoothstep(-0.75, 0.25, Distance - r4.x + 1));
}

// 圆角矩形的阴影
float4 Shadow(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    float Distance = distance(p, min(p, s - round - shadow));
    return lerp(0, backgroundColor, pow(1 - smoothstep(-0.75 - shadow, 0.25, Distance - round - shadow), 2));
}

technique Technique1
{
    pass HasBorder
    {
        PixelShader = compile ps_3_0 HasBorder();
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