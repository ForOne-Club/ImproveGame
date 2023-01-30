sampler uImage0 : register(s0);

float4 PS1(float2 coords : TEXCOORD0) : COLOR0
{
    return tex2D(uImage0, coords);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PS1();
    }
}