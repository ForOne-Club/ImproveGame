#define GLURRANGE 1
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
    
    float2 origin = size / 2 - float2(radius, radius);
    float2 corner = size / 2 - float2(border, border);
    float2 dxy = abs(position - size / 2);
    
    int2 insxy = step(dxy, origin);
    int2 outsxy = step(dxy, corner);
    int outside = (insxy.x + insxy.y) % 2;
    float length = distance(dxy, origin) * (1 - insxy.x) * (1 - insxy.y) + (dxy.x - (size.x / 2 - radius)) * outside * insxy.y + (dxy.y - (size.y / 2 - radius)) * outside * insxy.x;
    float4 color = lerp(lerp(background, borderColor, smoothstep(radius - border, radius - border + GLURRANGE, length)), float4(0, 0, 0, 0), clamp(length - radius + GLURRANGE, 0, 1));
    return color;
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_3_0 BoxFunc();
    }
}