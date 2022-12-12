float2 size; // 尺寸
float round; // 圆角
float4 background;
float shadowWidth;

float sdRoundRect(float2 p, float2 s)
{
    return distance(p, min(p, s - round - shadowWidth));
}

float4 BoxFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    return lerp(0, background, pow(1 - smoothstep(-0.8 - shadowWidth, 0.2, sdRoundRect(p, s) - round - shadowWidth), 1.5));
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_2_0 BoxFunc();
    }
}