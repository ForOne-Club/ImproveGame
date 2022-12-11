float2 size; // 尺寸
float round; // 圆角
float4 background;
float shadowSize;

float sdRoundRect(float2 p, float2 s)
{
    return distance(p, min(p, s - round));
}

float4 BoxFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    return lerp(background, 0, smoothstep(-0.7 - shadowSize, 0.7, sdRoundRect(p, s) - round));
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_2_0 BoxFunc();
    }
}