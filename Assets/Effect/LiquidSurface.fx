sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float4x4 uTransform;
float uXStart;
float uXEnd;
float uTime;
float uWaveScale;

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

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs((c.xxx + K.xyz - floor(c.xxx + K.xyz)) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float4 PixelShaderFunction(PSInput input) : COLOR0
{
    // 在纹理上的坐标，注意这个没有考虑帧的，是直接在贴图上反映
    float3 coord = input.Texcoord;

    // 将coord根据当前帧的起始和结尾坐标换算成0-1，用于柏林噪声
    float length = uXEnd - uXStart;
    float xInFrame = (coord.x - uXStart) / length;

    // 获取柏林颜色
    float berlinX = xInFrame * 0.1;
    float4 berlin = tex2D(uImage1, float2(berlinX, uTime));

    // 获取柏林颜色反映到y坐标上的值
    float berlinFactor = (1 - berlin.x) * uWaveScale;
    if (coord.y < berlinFactor)
        return float4(0, 0, 0, 0);

    // 插值x坐标，分成左右两半
    if (xInFrame < 0.5)
        xInFrame *= 1.95; // 有误差
    else
        xInFrame = (xInFrame - 0.5) * 2;
    coord.x = lerp(uXStart, uXEnd, xInFrame);

    float4 c = tex2D(uImage0, float2(coord.x, coord.y - berlinFactor));
    return c * input.Color;
}

PSInput VertexShaderFunction(VSInput input)
{
    PSInput output;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    output.Color = input.Color;
    output.Texcoord = input.Texcoord;
    return output;
}


technique Technique1
{
    pass ColorBar
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}