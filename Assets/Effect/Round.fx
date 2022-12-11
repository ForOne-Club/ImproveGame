float size; // 尺寸
float4 background; // 背景颜色

float4 RoundFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    return lerp(background, 0, smoothstep(-0.8, 0.2, distance(p, 0) - s.x));
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_3_0 RoundFunction();
    }
}