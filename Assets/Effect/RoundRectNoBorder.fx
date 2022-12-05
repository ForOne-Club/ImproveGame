float2 size;
float round;
float4 background;

float4 RoundRectangle(float2 coords : TEXCOORD0) : COLOR0
{
    float2 position = coords * size;
    float2 origin = size / 2 - round;
    float2 dxy = abs(position - size / 2);
    int2 insxy = step(dxy, origin);
    int outside = (insxy.x + insxy.y) % 2;
    float length = distance(dxy, origin) * (1 - insxy.x) * (1 - insxy.y)
        + (dxy.x - (size.x / 2 - round)) * outside * insxy.y
        + (dxy.y - (size.y / 2 - round)) * outside * insxy.x;
    return lerp(background, float4(0, 0, 0, 0), clamp((length - round + 1) / 1, 0, 1));
}

technique Technique1
{
    pass RoundRectangle
    {
        PixelShader = compile ps_3_0 RoundRectangle();
    }
}