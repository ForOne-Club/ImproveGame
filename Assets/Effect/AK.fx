float4x4 uTransform;

// 顶点着色器参数
struct VSInput
{
    float2 Pos : POSITION0;
    float3 Texcoord : TEXCOORD0;
};

// 像素着色器参数
struct PSInput
{
    float4 Pos : SV_POSITION;
    float3 Texcoord : TEXCOORD0;
};

float4 PSFunction(PSInput input) : COLOR0
{
    return float4(1, 0.5, 0.5, 1);
}

PSInput VSFunction(VSInput input)
{
    PSInput output;
    output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
    output.Texcoord = input.Texcoord;
    return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VSFunction();
        PixelShader = compile ps_2_0 PSFunction();
    }
}