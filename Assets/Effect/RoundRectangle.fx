sampler image : register(s0);

float2 size;
float round;
float border;
float4 borderColor;
float4 backgroundColor;

float4 RoundRectangle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 rt = size / 2;
    float2 pos = abs(coords * size - rt);
    float distanceR = distance(pos, clamp(pos, float2(0, 0), rt - round));
    return lerp(lerp(backgroundColor, borderColor, smoothstep(round - border, round - border + 1, distanceR)), float4(0, 0, 0, 0), smoothstep(round - 1, round, distanceR));
}

technique Technique1
{
    pass RoundRectangle
    {
        PixelShader = compile ps_3_0 RoundRectangle();
    }
}