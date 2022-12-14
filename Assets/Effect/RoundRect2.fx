float2 size; // 尺寸
float round; // 圆角
float border; // 边框
float4 borderColor;
float4 backgroundColor;
float shadow;

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
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-0.8, 0.2, sdRoundRect(p, s) - round + border)), 0, smoothstep(-0.8, 0.2, sdRoundRect(p, s) - round));
}

float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    return lerp(backgroundColor, 0, smoothstep(-0.8, 0.2, sdRoundRect(p, s) - round));
}

float4 Shadow(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    return lerp(0, backgroundColor, pow(1 - smoothstep(-0.8 - shadow, 0.2, sdShadow(p, s) - round - shadow), 1.5));
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