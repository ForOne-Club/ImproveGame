sampler uImage0 : register(s0);

float2 uScreenResolution;
float uIntensity;
float uRange;
static float gauss[7] = { 0.05, 0.1, 0.24, 0.4, 0.24, 0.1, 0.05 };

float4 BlurX(float2 coords : TEXCOORD0) : COLOR0 // 水平方向模糊
{
    float4 color = float4(0, 0, 0, 1);
    float dx = uRange / uScreenResolution.x;
    color = float4(0, 0, 0, 1);
    for (int c = 0; c < 9; c++)
    {
        for (int i = -3; i <= 3; i++)
        {
            color.rgb += gauss[i + 3] * tex2D(uImage0, float2(coords.x + dx * i, coords.y)).rgb * uIntensity;
        }
    }
    return color;
}
float4 BlurY(float2 coords : TEXCOORD0) : COLOR0 // 竖直方向模糊
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
    pass BlurX
    {
        PixelShader = compile ps_3_0 BlurX();
    }
    pass BlurY
    {
        PixelShader = compile ps_3_0 BlurY();
    }
}