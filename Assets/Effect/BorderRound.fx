sampler image : register(s0);

float2 imageSize;
float border;
float4 borderColor;
float4 background;

float2 ToFloat2(float f)
{
    return float2(cos(f), sin(f));
}

float ToRotation(float2 f)
{
    return atan2(f.y, f.x);
}

float4 PixelShaderFunc2(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixel = 1 / imageSize;
    float4 color = tex2D(image, coords);
    float2 center = float2(0.5, 0.5);
    float dis = distance(center, coords);
    if (dis >= 0.5 - pixel.x * border && dis <= 0.5)
    {
        color = borderColor;
    }
    else if (dis < 0.5 - pixel.x * border)
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
    pass TestColor
    {
        PixelShader = compile ps_2_0 PixelShaderFunc2();
    }
}