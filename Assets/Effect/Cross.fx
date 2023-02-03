float4x4 uTransform;
float2 uSizeOver2;
float uBorder;
float uRound;
float4 uBorderColor;
float4 uBackgroundColor;

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

float4 ps_main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 p = abs(coords - uSizeOver2);
    float d = length(p - min(p.x + p.y, uSizeOver2) * 0.5) - uRound;
    return lerp(lerp(uBackgroundColor, uBorderColor, smoothstep(-1, 0.5, d + uBorder)), 0, smoothstep(-1, 0.5, d));
}
technique Technique1
{
    pass Cross
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 ps_main();
    }
}