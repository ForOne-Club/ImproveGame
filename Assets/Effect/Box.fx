#define GLURRANGE 1

float2 size; // 尺寸
float round; // 圆角
float border; // 边框
float4 borderColor;
float4 background;

float sdRoundSquare(float2 p, float2 s, float round)
{
    float2 q = abs(p) - s + round;
    return min(max(q.x, q.y), 0) + length(max(q, 0)) - round;
}

float4 BoxFunc(float2 coords : TEXCOORD0) : COLOR0
{
    float2 s = size / 2;
    float2 p = coords * size - size / 2;
    // float2 origin = size / 2 - round;
    // float2 corner = size / 2 - border;
    // float2 dxy = abs(position - size / 2);
    // int2 insxy = step(dxy, origin);
    // int2 outsxy = step(dxy, corner);
    // int outside = (insxy.x + insxy.y) % 2;
    // float length = distance(dxy, origin) * (1 - insxy.x) * (1 - insxy.y)
    //     + (dxy.x - (size.x / 2 - round)) * outside * insxy.y
    //     + (dxy.y - (size.y / 2 - round)) * outside * insxy.x;
    float length = sdRoundSquare(p, s, round);
    float4 color = lerp(lerp(background, borderColor, smoothstep(-border, -border + GLURRANGE, length)),
        float4(0, 0, 0, 0), clamp((length + GLURRANGE) / GLURRANGE, 0, 1));
    return color;
}

technique Technique1
{
    pass Box
    {
        PixelShader = compile ps_2_0 BoxFunc();
    }
}