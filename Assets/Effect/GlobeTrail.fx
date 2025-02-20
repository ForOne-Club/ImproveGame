// 模糊处理
sampler uImage0 : register(s0);
sampler maskImage : register(s1);

float4x4 uTransform;
float uTime;

// 像素大小（纹理坐标的偏移量）
float2 pixelSize;

float gauss[3][3] = {
    0.075, 0.124, 0.075,
    0.124, 0.204, 0.124,
    0.075, 0.124, 0.075
};

struct VSInput
{
    float2 Pos : POSITION0;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float3 Texcoord : TEXCOORD0;
};

// 根据饱和度计算权重
float GetSaturationWeight(float3 rgb)
{
    float maxVal = max(max(rgb.r, rgb.g), rgb.b);
    float minVal = min(min(rgb.r, rgb.g), rgb.b);
    float delta = maxVal - minVal;
    return delta; // 饱和度作为权重
}

// 主函数：对当前像素及其周围像素进行加权平均
float4 WeightedSaturationBlur(float2 uv)
{
    // 当前像素的颜色
    float3 centerColor = tex2D(uImage0, uv).rgb;
    float centerWeight = GetSaturationWeight(centerColor);

    // 周围八个像素的UV偏移
    float2 offsets[8] = {
        float2(-pixelSize.x, -pixelSize.y), float2(0.0, -pixelSize.y), float2(pixelSize.x, -pixelSize.y),
        float2(-pixelSize.x, 0.0), float2(pixelSize.x, 0.0),
        float2(-pixelSize.x, pixelSize.y), float2(0.0, pixelSize.y), float2(pixelSize.x, pixelSize.y)};

    // 初始化加权颜色和总权重
    float3 weightedColor = centerColor * centerWeight;
    float totalWeight = centerWeight;

    // 遍历周围八个像素
    for (int i = 0; i < 8; i++)
    {
        float2 sampleUV = uv + offsets[i];
        float3 sampleColor = tex2D(uImage0, sampleUV).rgb;
        float sampleWeight = GetSaturationWeight(sampleColor);

        weightedColor += sampleColor * sampleWeight;
        totalWeight += sampleWeight;
    }

    // 归一化加权颜色
    weightedColor /= totalWeight;

    return float4(weightedColor, 1.0); // 返回加权平均后的颜色
}

// 旧的像素着色器函数，uImage0传入球本身贴图，但是因为贴图上颜色会突变，效果不，所以弃用了
// 原本还写了个根据饱和度混合。。emm还是自己指定颜色最好
float4 OldPSFunc(PSInput input) : COLOR0
{
    float3 coords = input.Texcoord;
    float3 mappedCoords = lerp(float3(0.3, 0.3, 0.3), float3(0.7, 0.7, 0.7), coords);
    float4 originalColor = WeightedSaturationBlur(mappedCoords) * input.Color;
    float maskX = -uTime + coords.x;
    if (maskX > 1.0)
        maskX -= 1.0;
    float4 maskColor = tex2D(maskImage, float2(maskX, coords.y));
    if (maskColor.r < 0.05)
        return float4(0, 0, 0, 0);
    return originalColor * maskColor.r * 2;
}

float4 PSFunc(PSInput input) : COLOR0
{
    float3 coords = input.Texcoord;
    float maskX = -uTime + coords.x;
    if (maskX > 1.0)
        maskX -= 1.0;
    float4 maskColor = tex2D(maskImage, float2(maskX, coords.y));
    if (maskColor.r < 0.05)
        return float4(0, 0, 0, 0);
    return input.Color * maskColor.r * 2;
}

PSInput VSFunc(VSInput input)
{
    PSInput output;
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    return output;
}

technique Technique1
{
    pass ColorBar
    {
        VertexShader = compile vs_3_0 VSFunc();
        PixelShader = compile ps_3_0 PSFunc();
    }
}