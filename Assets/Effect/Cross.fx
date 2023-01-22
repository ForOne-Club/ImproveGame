float4x4 uTransform;
float size;
float border;
float round;
float4 borderColor;
float4 backgroundColor;

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

// -------------------------------
// ---------- 顶点着色器 ----------
// -------------------------------
PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Coord = input.Coord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

float4 ps_main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = abs(coords * size - s);
    float d = length(p - min(p.x + p.y, s) * 0.5) - round;
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-1, 0.5, d + border)), 0, smoothstep(-1, 0.5, d));
}
technique Technique1
{
    pass Cross
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 ps_main();
    }
}