sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float4x4 uTransform; // 我不知道是啥（原理），但我大受震惊。
float uTime;

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs((c.xxx + K.xyz - floor(c.xxx + K.xyz)) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float4 PSFunc(PSInput input) : COLOR0
{
    return 1;
    float3 coord = input.Texcoord;
    float y = uTime + coord.x;
    float4 c1 = tex2D(uImage1, float2(coord.x, coord.y));
    float4 c3 = tex2D(uImage2, float2(y, coord.y));
    c1 *= c3;
    float4 c = tex2D(uImage0, float2(c1.r, 0));
    if (c.r < 0.1)
        return float4(0, 0, 0, 0);
    return 2 * c * coord.z;

}

PSInput VSFunc(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}


technique Technique1
{
    pass ColorBar
    {
        VertexShader = compile vs_2_0 VSFunc();
        PixelShader = compile ps_2_0 PSFunc();
    }
}