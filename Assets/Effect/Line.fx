float4x4 uTransform;
float2 size;
float4 background;
float2 start;
float2 end;
float width;
float border;
float4 borderColor;

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

float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 ba = end - start;
    float2 pa = coords * size - start;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
    return lerp(background, 0, smoothstep(-0.5, +0.5, length(pa - h * ba) - width));
}

float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 ba = end - start;
    float2 pa = coords * size - start;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
    return lerp(lerp(background, borderColor, smoothstep(-0.5, 0.5, length(pa - h * ba) - width + border)), 0, smoothstep(-0.5, 0.5, length(pa - h * ba) - width));
}

technique Technique1
{
    pass NoBorder
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_3_0 NoBorder();
    }

    pass HasBorder
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_3_0 HasBorder();
    }
}