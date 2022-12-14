float2 size;
float4 background;
float2 start;
float2 end;
float width;
float border;
float4 borderColor;

float4 NoBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 ba = end - start;
    float2 pa = coords * size - start;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
    return lerp(background, 0, smoothstep(width - 0.5, width + 0.5, length(pa - h * ba)));
}

float4 HasBorder(float2 coords : TEXCOORD0) : COLOR0
{
    float2 ba = end - start;
    float2 pa = coords * size - start;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
    return lerp(lerp(background, borderColor, smoothstep(width - border - 0.5, width - border + 0.5, length(pa - h * ba))), 0, smoothstep(width - 0.5, width + 0.5, length(pa - h * ba)));
}

technique Technique1
{
    pass NoBorder
    {
        PixelShader = compile ps_3_0 NoBorder();
    }

    pass HasBorder
    {
        PixelShader = compile ps_3_0 HasBorder();
    }
}