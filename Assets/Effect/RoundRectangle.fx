sampler image : register(s0);

float2 size;
float round;
float border;
float4 borderColor;
float4 backgroundColor;

float sdRoundedBox(float2 p, float2 b, float r)
{
    return length(max(abs(p) - b, 0.0)) - r;
}

float4 RoundRectangle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 hsize = size / 2;
    float2 pos = abs(coords * size - hsize);
    float Length = sdRoundedBox(pos, hsize - round, round);
    return lerp(lerp(backgroundColor, borderColor, smoothstep(-border - 1, -border, Length)), 0, smoothstep(0, 1, Length));
}

technique Technique1
{
    pass RoundRectangle
    {
        PixelShader = compile ps_3_0 RoundRectangle();
    }
}