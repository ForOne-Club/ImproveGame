float size; // 尺寸
float4 background; // 背景颜色
float border;
float4 borderColor;

float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    return lerp(background, 0, smoothstep(-0.8, 0.2, distance(p, 0) - s.x));
}

float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    return lerp(lerp(background, borderColor, smoothstep(-0.5, 0.5, distance(p, 0) - s.x + border)), 0, smoothstep(-0.5, 0.5, distance(p, 0) - s.x));
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
}