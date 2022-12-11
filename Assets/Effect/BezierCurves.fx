float2 size;
float2 Pn[4];

// 四个点

float4 DrawPoint(float2 pos, float2 pointPos, float4 newColor, float4 oldColor)
{
    return lerp(newColor, oldColor, smoothstep(4, 5, distance(pointPos, pos)));
}

float4 DrawLine(float2 p1, float2 p2, float2 pos, float4 newColor, float4 oldColor)
{
    float2 ba = p2 - p1;
    float2 pa = pos - p1;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
    return lerp(newColor, oldColor, smoothstep(1, 2, length(pa - h * ba)));
}

float4 BezierCurves(float2 coords : TEXCOORD0) : COLOR0
{
    float2 pos = coords * size;
    float4 color = 0;
    color = DrawLine(Pn[0], Pn[3], pos, float4(1, 0.5, 0.5, 1), color);
    color = DrawLine(Pn[1], Pn[2], pos, float4(1, 0.5, 0.5, 1), color);
    color = DrawPoint(pos, Pn[0], 1, color);
    color = DrawPoint(pos, Pn[1], 1, color);
    color = DrawPoint(pos, Pn[2], 1, color);
    color = DrawPoint(pos, Pn[3], 1, color);
    color = DrawPoint(pos, 50, 1, color);
    return color;
}

technique Technique1
{
    pass Fork
    {
        PixelShader = compile ps_3_0 BezierCurves();
    }
}