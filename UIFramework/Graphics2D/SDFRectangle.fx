sampler uImage0 : register(s0);

float4x4 uTransform;
float4 uBackgroundColor;
float uBorder;
float4 uBorderColor;
float uShadowSize;
float uInnerShrinkage;
float2 uTransition;

float3 uBarInfo;
float2 uSizeOver2;
float4 uRound;

struct VSInput
{
    float2 Position : POSITION0;
    float2 Coord : TEXCOORD0;
    float Rounded : TEXCOORD1;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 Coord : TEXCOORD0;
    float Rounded : TEXCOORD1;
};

PSInput VS_PCR(VSInput input)
{
    PSInput output;
    output.Coord = input.Coord;
    output.Position = mul(float4(input.Position, 0, 1), uTransform);
    output.Rounded = input.Rounded;
    return output;
}

/*float sdRoundedRectangle(float2 pos, float2 sizeOver2, float rounded)
{
    float2 q = pos - sizeOver2 + rounded;
    return min(max(q.x, q.y), 0) + length(max(q, 0)) - rounded;
}*/

float4 HasBorder(float2 q : TEXCOORD0, float rounded : TEXCOORD1) : COLOR0
{
    // float2 q = pos - sizeOver2 + rounded;
    float Distance = min(max(q.x, q.y), 0) + length(max(q.xy, 0)) - rounded + uInnerShrinkage;
    return lerp(lerp(uBackgroundColor, uBorderColor, smoothstep(uTransition.x, uTransition.y, Distance + uBorder)), 0, smoothstep(uTransition.x, uTransition.y, Distance));
}

float4 NoBorder(float2 q : TEXCOORD0, float rounded : TEXCOORD1) : COLOR0
{
    // float2 q = pos - sizeOver2 + rounded;
    float Distance = min(max(q.x, q.y), 0) + length(max(q.xy, 0)) - rounded + uInnerShrinkage;
    return lerp(uBackgroundColor, 0, smoothstep(uTransition.x, uTransition.y, Distance));
}

float4 Shadow(float2 q : TEXCOORD0, float rounded : TEXCOORD1) : COLOR0
{
    // float2 q = pos - sizeOver2 + rounded;
    float Distance = min(max(q.x, q.y), 0) + length(max(q.xy, 0)) - rounded;
    return lerp(uBackgroundColor, 0, smoothstep(uTransition.x - uShadowSize, uTransition.y, Distance));
}

float RoundedBox(float2 p) //圆角矩形-需要另外指定 uRound-四个圆角的半径
{
	p -= uSizeOver2;
	float4 r = uRound;
	float2 b = uSizeOver2;
	r.xy = (p.x > 0.0) ? r.xy : r.zw;
	r.x = (p.y > 0.0) ? r.x : r.y;
	float2 q = abs(p) - b + r.x;
	return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}

float4 BarColor(float2 q : TEXCOORD0, float rounded : COLOR0) : COLOR0
{
	float Distance = RoundedBox(q) + uInnerShrinkage;
	return lerp(color, 0, smoothstep(-1, 0.5, Distance));
}


technique T1
{
    pass HasBorder
    {
        VertexShader = compile vs_2_0 VS_PCR();
        PixelShader = compile ps_2_0 HasBorder();
    }

    pass NoBorder
    {
        VertexShader = compile vs_2_0 VS_PCR();
        PixelShader = compile ps_2_0 NoBorder();
    }

    pass Shadow
    {
        VertexShader = compile vs_2_0 VS_PCR();
        PixelShader = compile ps_2_0 Shadow();
    }
	pass BarColor
	{
		VertexShader = compile vs_2_0 VS_PCR();
		PixelShader = compile ps_2_0 BarColor();
	}
}