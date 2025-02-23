float2 uCenter;
float2 uScreenSize;
float uTime;
float4 gridEffect(float2 texCoord : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
	float2 uv = texCoord * uScreenSize - uCenter;
	
	float l = length(uv) * .005;
	float w = 2.0;
	float t = uTime;
	l /= w;
	l -= uTime - 1.0;
	if (l * (1.0 - l) < 0.0)
		l = 0.0;
	else
		l = 1.0 - cos(6.28 * sqrt(1.0 - l));
	l *= .65;
	uv = uv - 16.0 * floor(uv / 16.0);
		if (uv.x < 2.0 || uv.y < 2.0)
		return color * l;
	return float4(0,0,0,0);
}
technique Technique1
{
	pass gridEffect
	{
		PixelShader = compile ps_3_0 gridEffect();
	}
}