float2 size; // 尺寸
float round; // 圆角
float border; // 边框
float4 borderColor;
float4 backgroundColor;
float shadow;

// step 第一个参数 < 第二个参数返回 1，≥ 返回 0

float sdRoundRect(float2 p, float2 s)
{
    return distance(p, min(p, s - round));
}

float sdShadow(float2 p, float2 s)
{
    return distance(p, min(p, s - round - shadow));
}

float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float2 a = float2(-0.75, 0.25);
    // float2 a = smoothstep(s - round * 1.25, s - round, p);
    // a = float2(-0.5, 0.5) - lerp(0, 0.1, min(a.x, a.y));
    float Distance = sdRoundRect(p, s);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(a.x, a.y, Distance - round + border + 2)), 0, smoothstep(a.x, a.y, Distance - round + 2));
}

float4 HasBorderR4(float2 coords : TEXCOORD0) : COLOR0
{
    // 四个圆角单独设置大小
    float4 r4;
    r4.xy = step(coords.x, 0.5) * r4.xz + (1 - step(coords.x, 0.5)) * r4.yw;
    r4.x = step(coords.y, 0.5) * r4.x + (1 - step(coords.y, 0.5)) * r4.y;
    return 0;
}

float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float2 a = float2(-0.75, 0.25);
    // float2 a = smoothstep(s - round * 1.25, s - round, p);
    // a = float2(-0.5, 0.5) - lerp(0, 0.1, min(a.x, a.y));
    return lerp(backgroundColor, 0, smoothstep(a.x, a.y, sdRoundRect(p, s) - round + 2));
}

float4 Shadow(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    float2 a = float2(-0.75, 0.25);
    // float2 a = smoothstep(s - round * 1.25, s - round, p);
    // a = float2(-0.5, 0.5) - lerp(0, 0.25, min(a.x, a.y));
    return lerp(0, backgroundColor, pow(1 - smoothstep(a.x - shadow, a.y, sdShadow(p, s) - round - shadow), 2));
}

technique Technique1
{
    pass HasBorder
    {
        PixelShader = compile ps_3_0 HasBorder();
    }

    pass NoBorder
    {
        PixelShader = compile ps_3_0 NoBorder();
    }

    pass Shadow
    {
        PixelShader = compile ps_3_0 Shadow();
    }
}