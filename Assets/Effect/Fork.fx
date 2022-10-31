sampler image : register(s0);
float4 color;
float2 size;
float round;
float4 Fork(float2 coords : TEXCOORD0) : COLOR0
{
    float2 rt = size / 2;
    float2 pos = abs(coords * size - rt);
    float2 posC = clamp(pos, 0, rt - round);
    float2 posStep = step(pos, rt - round);
    float distanceR = abs(pos.x - pos.y) / length(float2(1, 1)) * !(posStep.x && posStep.y);
    return lerp(color, 0, smoothstep(round - 1, round, distanceR));
}
technique Technique1
{
    pass Fork
    {
        PixelShader = compile ps_3_0 Fork();
    }
}