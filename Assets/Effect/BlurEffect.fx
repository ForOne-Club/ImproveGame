sampler uImage0 : register(s0);
float2 uPixelSize;
float uBlurRadius;

// 高斯核权重（sigma=1.0，radius=5，11个权重）
static float gauss[11] =
{
    0.0093, 0.0280, 0.0659, 0.1217, 0.1757, 0.1986, 0.1757, 0.1217, 0.0659, 0.0280, 0.0093
};

// 高斯核归一化系数（sum of gauss[]）
static float gaussSum[6] =
{
    0.0, // 未使用（从radius=1开始）
    0.55, // radius=1 (sum of center + 2 sides: 0.1986 + 2*0.1757 + 2*0.1217)
    0.7934, // radius=2 (继续累加外围权重)
    0.9252, // radius=3
    0.9812, // radius=4
    0.9998 // radius=5 (全部11个权重的和)
};

// 定义水平模糊宏（BlurX）
#define BLUR_X(radius, sum) \
    float4 BlurX##radius(float2 coords : TEXCOORD0) : COLOR0 \
    { \
        float4 color = float4(gauss[5 + radius] * tex2D(uImage0, float2(coords.x + uPixelSize.x * radius * uBlurRadius, coords.y)).rgb, 1); \
        for (int i = -radius; i < radius; i++) \
        { \
            color.rgb += gauss[5 + i] * tex2D(uImage0, float2(coords.x + uPixelSize.x * i * uBlurRadius, coords.y)).rgb; \
        } \
        color.rgb = color.rgb / sum; \
        return color; \
    }

// 定义垂直模糊宏（BlurY）
#define BLUR_Y(radius, sum) \
    float4 BlurY##radius(float2 coords : TEXCOORD0) : COLOR0 \
    { \
        float4 color = float4(gauss[5 + radius] * tex2D(uImage0, float2(coords.x, coords.y + uPixelSize.y * radius * uBlurRadius)).rgb, 1); \
        for (int i = -radius; i < radius; i++) \
        { \
            color.rgb += gauss[5 + i] * tex2D(uImage0, float2(coords.x, coords.y + uPixelSize.y * i * uBlurRadius)).rgb; \
        } \
        color.rgb = color.rgb / sum; \
        return color; \
    }

// 生成不同半径的模糊函数
BLUR_X(1, gaussSum[1]) // radius=1, sum=2.3664
BLUR_Y(1, gaussSum[1])

BLUR_X(2, gaussSum[2]) // radius=2, sum=3.6706
BLUR_Y(2, gaussSum[2])

BLUR_X(3, gaussSum[3]) // radius=3, sum=4.3616
BLUR_Y(3, gaussSum[3])

BLUR_X(4, gaussSum[4]) // radius=4, sum=4.7726
BLUR_Y(4, gaussSum[4])

BLUR_X(5, gaussSum[5]) // radius=5, sum=4.9906
BLUR_Y(5, gaussSum[5])

technique Blur
{
    pass BlurX1
    {
        PixelShader = compile ps_3_0 BlurX1();
    }
    pass BlurY1
    {
        PixelShader = compile ps_3_0 BlurY1();
    }
    pass BlurX2
    {
        PixelShader = compile ps_3_0 BlurX2();
    }
    pass BlurY2
    {
        PixelShader = compile ps_3_0 BlurY2();
    }
    pass BlurX3
    {
        PixelShader = compile ps_3_0 BlurX3();
    }
    pass BlurY3
    {
        PixelShader = compile ps_3_0 BlurY3();
    }
    pass BlurX4
    {
        PixelShader = compile ps_3_0 BlurX4();
    }
    pass BlurY4
    {
        PixelShader = compile ps_3_0 BlurY4();
    }
    pass BlurX5
    {
        PixelShader = compile ps_3_0 BlurX5();
    }
    pass BlurY5
    {
        PixelShader = compile ps_3_0 BlurY5();
    }
}