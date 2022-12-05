float2 size; // 尺寸
float round; // 圆角
float border; // 边框
float4 borderColor;
float4 backgroundColor;

float4 BoxFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - size / 2);
    float a = distance(p, min(p, s - round));
    float4 color = lerp(lerp(backgroundColor, borderColor, smoothstep(round - border - 0.7, round - border + 0.7, a)),
        0, smoothstep(round - 0.7, round + 0.7, a));
    return color;
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_2_0 BoxFunc();
    }
}