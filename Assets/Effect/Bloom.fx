sampler uImage0 : register(s0);

float2 uScreenResolution;
float uMinBrightness;
float uIntensity;
float uRange;
static float gauss[7] = { 0.05, 0.1, 0.24, 0.4, 0.24, 0.1, 0.05 };

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0, coords);
    float brightness = c.r * 0.4 + c.g * 0.4 + c.b * 0.2;
    if (brightness > uMinBrightness)
        return c;
    else
        return float4(0, 0, 0, 0);
}
float4 GlurH(float2 coords : TEXCOORD0) : COLOR0 //水平方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dx = uRange / uScreenResolution.x;
    color = float4(0, 0, 0, 1);
    for (int c = 0; c < 12; c++)
    {
        for (int i = -3; i <= 3; i++)
        {
            color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x + dx * i, coords.y)).rgb * uIntensity;
        }
    }
    return color;
}
float4 GlurV(float2 coords : TEXCOORD0) : COLOR0 //竖直方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dy = uRange / uScreenResolution.y;
    for (int c = 0; c < 6; c++)
    {
        for (int i = -3; i <= 3; i++)
        {
            color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x, coords.y + dy * i)).rgb * uIntensity;
        }
    }
    return color;
}
technique Technique1
{
    pass Bloom
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
    pass GlurH
    {
        PixelShader = compile ps_3_0 GlurH();
    }
    pass GlurV
    {
        PixelShader = compile ps_3_0 GlurV();
    }
}