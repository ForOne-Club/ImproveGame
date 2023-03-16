float4x4 uTransform;
float4 uBackgroundColor;
float uBorder;
float4 uBorderColor;
float uShadowSize;
float uInnerShrinkage;

float4 uTestColor;

struct PosCoordInput
{
    float2 Position : POSITION0;
    float2 Coord : TEXCOORD0;
};

struct PosCoordOut
{
    float4 Position : SV_POSITION;
    float2 Coord : TEXCOORD0;
};

PosCoordOut VSPosCoord(PosCoordInput input)
{
    PosCoordOut output;
    output.Coord = input.Coord;
    output.Position = mul(float4(input.Position, 0, 1), uTransform);
    return output;
}

struct VSInput
{
    float2 Position : POSITION0;
    float2 Coord : TEXCOORD0;
    float Rounded : COLOR0;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 Coord : TEXCOORD0;
    float Rounded : COLOR0;
};

PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Coord = input.Coord;
    output.Position = mul(float4(input.Position, 0, 1), uTransform);
    output.Rounded = input.Rounded;
    return output;
}

/*float sdRoundedRectangle(float2 pos, float2 sizeOver2, float rounded)
{
    float2 q = pos - sizeOver2 + rounded;
    return min(max(q.x, q.y), 0) + length(max(q, 0)) - rounded;
}*/

float4 HasBorder(float2 coords : TEXCOORD0, float rounded : COLOR0) : COLOR0
{
    float Distance = min(max(coords.x, coords.y), 0) + length(max(coords.xy, 0)) - rounded + uInnerShrinkage;
    return lerp(lerp(uBackgroundColor, uBorderColor, smoothstep(-1, 0.5, Distance + uBorder)), 0, smoothstep(-1, 0.5, Distance));
}

float4 NoBorder(float2 coords : TEXCOORD0, float rounded : COLOR0) : COLOR0
{
    float Distance = min(max(coords.x, coords.y), 0) + length(max(coords.xy, 0)) - rounded + uInnerShrinkage;
    return lerp(uBackgroundColor, 0, smoothstep(-1, 0.5, Distance));
}

float4 Shadow(float2 coords : TEXCOORD0, float rounded : COLOR0) : COLOR0
{
    float Distance = min(max(coords.x, coords.y), 0) + length(max(coords.xy, 0)) - rounded;
    return lerp(uBackgroundColor, 0, smoothstep(-1 - uShadowSize, 0.5, Distance));
}

float4 Round(float2 coord : TEXCOORD0, float rounded : COLOR0) : COLOR0
{
    // return float4(1, 1, 1, 1);
    float d = length(coord) - rounded;
    return lerp(lerp(uBackgroundColor, uBorderColor, clamp(d + lerp(2, 50, coord.y / 100), 0, 1)), 0, clamp(d, 0, 1));
}

technique T1
{
    pass Test
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 Round();
    }

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