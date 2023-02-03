float4x4 uTransform;
float4 uBackgroundColor;
float uBorder;
float4 uBorderColor;
float uShadowSize;
float uInnerShrinkage;

struct VSInput
{
    float2 Pos : POSITION0;
    float3 Coord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float3 Coord : TEXCOORD0;
};

PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Coord = input.Coord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float sdRoundSquare(float2 pos, float2 sizeOver2, float rounded)
{
    float2 q = pos - sizeOver2 + rounded;
    return min(max(q.x, q.y), 0) + length(max(q, 0)) - rounded;
}

float4 HasBorder(float3 coords : TEXCOORD0) : COLOR0
{
    float Distance = min(max(coords.x, coords.y), 0) + length(max(coords.xy, 0)) - coords.z + uInnerShrinkage;
    return lerp(lerp(uBackgroundColor, uBorderColor, smoothstep(-1, 0.5, Distance + uBorder)), 0, smoothstep(-1, 0.5, Distance));
}

float4 NoBorder(float3 coords : TEXCOORD0) : COLOR0
{
    float Distance = min(max(coords.x, coords.y), 0) + length(max(coords.xy, 0)) - coords.z + uInnerShrinkage;
    return lerp(uBackgroundColor, 0, smoothstep(-1, 0.5, Distance));
}

float4 Shadow(float3 coords : TEXCOORD0) : COLOR0
{
    float Distance = min(max(coords.x, coords.y), 0) + length(max(coords.xy, 0)) - coords.z;
    return lerp(0, uBackgroundColor, pow(1 - smoothstep(-1 - uShadowSize, 0.5, Distance), 1.5));
}

technique T1
{
    pass HasBorder
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 HasBorder();
    }

    pass NoBorder
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 NoBorder();
    }

    pass Shadow
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 Shadow();
    }
}