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

float4 border;
float borderSize;
float4 background;
float2 imageSize;

float4 PixelShaderFunc2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixel = 1 / imageSize;
    float4 color = tex2D(image, coords);
    float2 center = float2(0.5, 0.5);
    float length1 = length(center - coords);
    if (length1 >= 0.5 - pixel.x * borderSize && length1 <= 0.5)
    {
        color = border;
    }
    else if (length1 < 0.5 - pixel.x * 2)
    {
        color = background;
    }
    else
    {
        color = float4(0, 0, 0, 0);
    }
    return color;
}

technique Technique1
{
    pass ColorPut
    {
        PixelShader = compile ps_2_0 PixelShaderFunc();
    }
    pass TestColor
    {
        PixelShader = compile ps_2_0 PixelShaderFunc2();
    }
}