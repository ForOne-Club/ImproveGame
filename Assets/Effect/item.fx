sampler image : register(s0);

float4 uColor;

float4 PixelShaderFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(image, coords);
    if (any(color))
    {
        color += uColor;
    }
    return color;
}


technique Technique1
{
    pass ColorPut
    {
        PixelShader = compile ps_2_0 PixelShaderFunc();
    }
}