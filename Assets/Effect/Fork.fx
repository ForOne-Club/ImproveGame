float size;
float border;
float radius; // 半径
float4 borderColor;
float4 backgroundColor;

float4 ps_main(float2 coords : TEXCOORD0) : COLOR0
{
    float2 rt = size / 2;
    float2 pos = abs(coords * size - rt);
    float d = length(pos - min(pos.x + pos.y, rt.x - radius) * 0.5) - radius;
    return lerp(lerp(backgroundColor, borderColor, smoothstep(0 - border, 1 - border, d)), 0, smoothstep(0, 1, d));
}
technique Technique1
{
    pass Fork
    {
        PixelShader = compile ps_2_0 ps_main();
    }
}