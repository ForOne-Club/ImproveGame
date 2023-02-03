float4x4 uTransform;
float uSizeOver2; // 尺寸
float4 uBackground; // 背景颜色
float uBorder;
float4 uBorderColor;

struct VSInput
{
    float2 Pos : POSITION0;
    float2 Coord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float2 Coord : TEXCOORD0;
};

PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Coord = input.Coord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = abs(coords - uSizeOver2);
    return lerp(uBackground, 0, smoothstep(-1, 0.5, distance(p, 0) - uSizeOver2.x));
}

float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = abs(coords - uSizeOver2);
    return lerp(lerp(uBackground, uBorderColor, smoothstep(-1, 0.5, distance(p, 0) - uSizeOver2.x + uBorder)), 0, smoothstep(-1, 0.5, distance(p, 0) - uSizeOver2.x));
}

technique Technique1
{
    pass HasBorder
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_3_0 HasBorder();
    }

    pass NoBorder
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_3_0 NoBorder();
    }
}