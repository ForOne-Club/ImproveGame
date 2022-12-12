sampler uImage0 : register(s0);
float2 size;
float2 mouse;

float4 F(float2 pos)
{
    float4 color;
    float count;
    for (float x = -3; x <= 3; x++)
    {
        for (float y = -3; y <= 3; y++)
        {
            color += tex2D(uImage0, (pos + float2(x, y)) / size);
            count++;
        }
    }
    return color / count;
}

float4 PSFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pos = coords * size;
    float4 color;
    float count;
    if (distance(mouse, pos) < 100)
        for (float x = -2; x <= 2; x++)
        {
            for (float y = -2; y <= 2; y++)
            {
                color += F(pos + float2(x, y));
                count++;
            }
        }
    return color / count;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PSFunction();
    }
}