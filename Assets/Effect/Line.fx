float2 size;
float4 background;
float2 start;
float2 end;
float width;

float4 DrawLine(float2 p1, float2 p2, float2 pos, float4 newColor, float4 oldColor)
{
    float2 ba = p2 - p1;
    float2 pa = pos - p1;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
    return lerp(newColor, oldColor, smoothstep(width - 0.5, width + 0.5, length(pa - h * ba)));
}

float4 Line(float2 coords : TEXCOORD0) : COLOR0
{
    return DrawLine(start, end, coords * size, background, 0);
}

technique Technique1
{
    pass Fork
    {
        PixelShader = compile ps_3_0 Line();
    }
}