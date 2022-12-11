float2 size; // 尺寸
float round; // 圆角
float border; // 边框
float4 borderColor;
float4 backgroundColor;

float sdRoundRect(float2 p, float2 s)
{
    return distance(p, min(p, s - round));
}

float4 BoxFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-0.8, 0.2, sdRoundRect(p, s) - round + border)), 0, smoothstep(-0.8, 0.2, sdRoundRect(p, s) - round));
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_3_0 BoxFunc();
    }
}