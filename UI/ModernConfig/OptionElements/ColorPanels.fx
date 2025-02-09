float4x4 uTransform;
struct VSInput
{
	float2 Pos : POSITION0;
	float3 Texcoord : TEXCOORD0;
};
struct PSInput
{
	float4 Pos : SV_POSITION;
	float3 Texcoord : TEXCOORD0;
};

PSInput VertexShaderFunction(VSInput input)
{
	PSInput output;
	output.Texcoord = input.Texcoord;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}
float3 uColor;
float4 RGBPanel(PSInput input) : COLOR0
{
	float2 coord = input.Texcoord;
	if (abs(coord.x - .5) < .3)
	{
		if (abs(coord.y - .4) < .3)
			return float4(uColor.x, (coord.x - .2) / .6, (coord.y - .2) / .6, 1.0);
		if (abs(coord.y - 0.85) < 0.05)
			return float4((coord.x - .2) / .6, uColor.y, uColor.z, 1.0);
	}
	return float4(0.0, 0.0, 0.0, 0.0);
}
float3 uHsl;
float2x2 uHueRotation;
float3 hueToRGB(float h)
{
	h -= floor(h);
	return saturate(float3(abs(h * 6.0 - 3.0) - 1.0, 2.0 - abs(h * 6.0 - 2.0), 2.0 - abs(h * 6.0 - 4.0)));
}
float3 hslToRGB(float h,float s,float l)
{
	float3 gray = float3(l,l,l);
	float3 c = lerp( gray, hueToRGB(h), s);
	if (l < .5)
		return 2 * l * c;
	else
		return lerp(c, float3(1.0, 1.0, 1.0), 2 * l - 1);
}

float4 HSLRing(PSInput input) : COLOR0
{
	float2 coord = input.Texcoord - .5;
	float ls = dot(coord,coord);
	//if (ls > 0.09 && ls < 0.16)
	//	return float4(hslToRGB(atan2(coord.y, coord.x) / 6.283, uHsl.y, uHsl.z), 1.0);
	if (ls > 0.0961 && ls < 0.1849) //.31^2ºÍ.43^2
		return float4(hslToRGB(atan2(coord.y, coord.x) / 6.283, uHsl.y, uHsl.z), 1.0) * saturate((0.74 * sqrt(ls) - ls - 0.1333) / 0.0011);
	coord = mul(uHueRotation, coord);
	ls = abs(coord.x) + abs(coord.y);
	if (ls < 0.31)
		return float4(hslToRGB(uHsl.x, coord.x / 0.62 + 0.5, coord.y / (0.31 - abs(coord.x)) * .5 + 0.5), 1.0) * saturate((0.0961 - ls * ls) / 0.0061);
	return float4(0.0, 0.0, 0.0, 0.0);
}

technique Technique1
{
	pass RGBPanel
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 RGBPanel();
	}
	pass HSLRing
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 HSLRing();
	}
}