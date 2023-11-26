sampler background : register(s0); // 背景图
sampler mask : register(s1); // 遮罩图
sampler picker : register(s2); // 取点图

// 对picker(取点图)上每个不是透明的点，使用mask(遮罩图)上的像素，否则使用background(背景图)上的像素
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 pickerColor = tex2D(picker, coords);
    if (any(pickerColor) && pickerColor.a > 0.2)
        return tex2D(mask, coords);
    else
        return tex2D(background, coords);
}

technique Technique1
{
    pass Mask
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}