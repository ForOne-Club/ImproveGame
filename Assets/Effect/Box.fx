sampler image : register(s0);

float2 size; // 尺寸
float radius; // 圆角
float border; // 边框
float4 borderColor1; // 边框颜色1
float4 borderColor2; // 边框颜色2
float4 background1; // 背景颜色1
float4 background2; // 背景颜色2

float4 BoxFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float2 position = coords * size;
    
    float4 borderColor = lerp(borderColor1, borderColor2, coords.x);
    float4 background = lerp(background1, background2, coords.x);
    
    float4 color = float4(0, 0, 0, 0);
    
    float length1 = distance(float2(radius, radius), position);
    float length2 = distance(float2(size.x - radius, radius), position);
    float length3 = distance(float2(radius, size.y - radius), position);
    float length4 = distance(float2(size.x - radius, size.y - radius), position);
    
    float r = radius - border;
    
    if (length1 < radius || length2 < radius || length3 < radius || length4 < radius)
    {
        if ((length1 < radius && length1 >= r) ||
            (length2 < radius && length2 >= r) ||
            (length3 < radius && length3 >= r) ||
            (length4 < radius && length4 >= r))
        {
            color = borderColor;
        }
        else
        {
            if (length1 < radius && length1 >= r - 1)
            {
                color = lerp(borderColor, background, r - length1);
            }
            else if (length2 < radius && length2 >= r - 1)
            {
                color = lerp(borderColor, background, r - length2);
            }
            else if (length3 < radius && length3 >= r - 1)
            {
                color = lerp(borderColor, background, r - length3);
            }
            else if (length4 < radius && length4 >= r - 1)
            {
                color = lerp(borderColor, background, r - length4);
            }
            else
            {
                color = background;
            }
        }
    }
    else if (length1 <= radius + 1)
    {
        color = lerp(borderColor, float4(0, 0, 0, 0), length1 - radius);
    }
    else if (length2 <= radius + 1)
    {
        color = lerp(borderColor, float4(0, 0, 0, 0), length2 - radius);
    }
    else if (length3 <= radius + 1)
    {
        color = lerp(borderColor, float4(0, 0, 0, 0), length3 - radius);
    }
    else if (length4 <= radius + 1)
    {
        color = lerp(borderColor, float4(0, 0, 0, 0), length4 - radius);
    }
    
    if ((position.x > radius && position.x < size.x - radius) ||
        (position.y > radius && position.y < size.y - radius))
    {
        if (position.x <= border || position.x >= size.x - border ||
            position.y <= border || position.y >= size.y - border)
        {
            color = borderColor;
        }
        else
        {
            color = background;
        }
    }
    
    return color;
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_3_0 BoxFunc();
    }
}