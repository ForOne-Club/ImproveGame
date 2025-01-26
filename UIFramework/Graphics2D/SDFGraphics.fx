//#define CreateShapeBordered(shape,k) float4 HasBorder(shape)(float2 coords:TEXCOORD0):COLOR0{return GetBorderedColor(coords,k);}
//#define CreateShapeNoBorder(shape,k) float4 NoBorder(shape)(float2 coords:TEXCOORD0):COLOR0{return GetNoBorderColor(coords,k);}
//#define ShapePass(shape) pass HasBorder shape { VertexShader = compile vs_3_0 VSFunction(); PixelShader = compile ps_3_0 HasBordershape(); } pass NoBordershape { VertexShader = compile vs_3_0 VSFunction(); PixelShader = compile ps_3_0 NoBordershape(); }
//↑错误的宏，非常的低级
sampler uImage0 : register(s0);

#define DEFINE_SINGLEPASS(type,suffix) \
pass type##suffix { \
    VertexShader = compile vs_2_0 VSFunction(); \
    PixelShader = compile ps_3_0 type##suffix(); \
}

#define DEFINE_PASS(suffix) \
DEFINE_SINGLEPASS(HasBorder,suffix)\
DEFINE_SINGLEPASS(NoBorder,suffix)\
DEFINE_SINGLEPASS(Bar,suffix)


#define DEFINE_SINGLEFUNCTION(type,suffix) \
float4 type##suffix(float2 coords : TEXCOORD0):COLOR0{return Get##type##Color(##suffix(coords));}

#define DEFINE_FUNCTION(suffix) \
DEFINE_SINGLEFUNCTION(NoBorder,suffix)\
DEFINE_SINGLEFUNCTION(HasBorder,suffix)\
DEFINE_SINGLEFUNCTION(Bar,suffix)

#define MAXPOLYGONPOINT 24
#define MAXBEZIERPOINT 9

//SDF全部改自iq佬的文章↓
//https://iquilezles.org/articles/distfunctions2d/
float4x4 uTransform;
float2 uSizeOver2;
float uBorder;
float4 uRound; //对于四个角的，一个维度一个角，否则取x为角
float4 uBorderColor;
float4 uBackgroundColor;
float2 uStart;
float2 uEnd;
float2 uAnother;
float uLineWidth;
float2 uTransition;
float uBottomScaler;
float uInnerShrinkage;

int uCurrentPointCount;
float2 uVectors[MAXPOLYGONPOINT];
float2 uBVectors[MAXBEZIERPOINT];

float uTime;
float uValueScaler;
//bool uHasBorder;
//int uStyle;
struct VSInput
{
	float2 Pos : POSITION0;
	float2 Coord : TEXCOORD0;
};

struct PSInput
{
	float4 Pos : SV_POSITION;
	float2 Coord : TEXCOORD0;
};

PSInput VSFunction(VSInput input)
{
	PSInput output;
	output.Coord = input.Coord;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}
float ndot(float2 a, float2 b)
{
	return a.x * b.x - a.y * b.y;
}
float dot2(float2 v)
{
	return dot(v, v);
}
float mod(float x, float y)
{
	return x - y * floor(x / y);
}
float bezier(float2 p,float2 A, float2 B, float2 C)
{
	float2 a = B - A;
	float2 b = A - 2.0 * B + C;
	float2 c = a * 2.0;
	float2 d = A - p;
	float kk = 1.0 / dot(b, b);
	float kx = kk * dot(a, b);
	float ky = kk * (2.0 * dot(a, a) + dot(d, b)) / 3.0;
	float kz = kk * dot(d, a);
	float res = 0.0;
	float u = ky - kx * kx;
	float u3 = u * u * u;
	float q = kx * (2.0 * kx * kx - 3.0 * ky) + kz;
	float h = q * q + 4.0 * u3;
	if (h >= 0.0)
	{
		h = sqrt(h);
		float2 x = (float2(h, -h) - q) / 2.0;
		float2 uv = sign(x) * pow(abs(x), float2(1.0 / 3.0, 1.0 / 3.0));
		float t = saturate(uv.x + uv.y - kx);
		res = dot2(d + (c + b * t) * t);
	}
	else
	{
		float z = sqrt(-u);
		float v = acos(q / (u * z * 2.0)) / 3.0;
		float m = cos(v);
		float n = sin(v) * 1.732050808;
		float3 t = saturate(float3(m + m, -n - m, n - m) * z - kx);
		res = min(dot2(d + (c + b * t.x) * t.x),
                   dot2(d + (c + b * t.y) * t.y));
        // the third root cannot be the closest
        // res = min(res,dot2(d+(c+b*t.z)*t.z));
	}
	return sqrt(res);

}
float Gtlp(float from, float to, float value)
{
	return (value - from) / (to - from);
}
float Cross(float2 A, float2 B)
{
	return A.x * B.y - A.y * B.x;
}

int LRoot(float2 P, float2 A, float2 B)
{
	float v = Gtlp(A.y, B.y, P.y);
	if (v * (1.0 - v) > 0.0 && sign(A.y - B.y) * Cross(P - A, B - A) > 0.0)
		return 1;
	return 0;
}
int QRoot(float2 p, float2 A, float2 B, float2 C)
{
	float2 a = A - 2.0 * B + C;
	float2 b = A - B;
	if (abs(a.y) <= 0.00000001)
	{
		b = 2.0 * (B - A);
		float m = (p.y - A.y) / b.y;
		if (m * (1 - m) > 0 && a.x * m * m + b.x * m + A.x > p.x)
			return 1;
		return 0;
        
	}
	else
	{
		b.y /= a.y;
		float d = b.y * b.y - (A.y - p.y) / a.y;
        //d*=4.0;
		if (d < 0.0)
			return 0;
		d = sqrt(d);
		
		float m = b.y - d;
		float m2 = m * m;
		int r = 0;
		if (m > m2 && a.x * m2 - 2.0 * b.x * m + A.x > p.x)
			r++;
		m = b.y + d;
		m2 = m * m;
		if (m > m2 && a.x * m2 - 2.0 * b.x * m + A.x > p.x)
			r++;
		return r;
	}

}
//下面这些函数负责形形
float Circle(float2 p) //圆
{
	return length(p - uSizeOver2) - uSizeOver2.x + uInnerShrinkage;
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
float Box(float2 p) //矩形
{
	p -= uSizeOver2;
	float2 b = uSizeOver2;
	float2 d = abs(p) - b;
	return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}
float OrientedBox(float2 p) //倾斜矩形-需要指定 uStart|uEnd-起终点 以及 uLineWidth-矩形宽
{
	float2 a = uStart;
	float2 b = uEnd;
	float th = uLineWidth;
	float l = length(b - a);
	float2 d = (b - a) / l;
	float2 q = (p - (a + b) * 0.5);
	q = mul(float2x2(d.x, d.y, -d.y, d.x), q);
	q = abs(q) - float2(l, th) * 0.5;
	return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
}
float Segment(float2 p) //线段-需要指定 uStart|uEnd-起终点 以及 uLineWidth-线宽
{
	float2 a = uStart;
	float2 b = uEnd;
	float2 pa = p - a, ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba * h) - uLineWidth;
}
float Rhombus(float2 p) //棱形
{
	p = abs(p - uSizeOver2);
	float2 b = uSizeOver2;
	float h = clamp(ndot(b - 2.0 * p, b) / dot(b, b), -1.0, 1.0);
	float d = length(p - 0.5 * b * float2(1.0 - h, 1.0 + h));
	return d * sign(p.x * b.y + p.y * b.x - b.x * b.y);
}
float Trapezoid(float2 p) //梯形 需要指定uBottomScaler-下底与上底的比值,如果是0.5，下底是上底一半，如果是2，上底是下底一半
{
	p -= uSizeOver2;
	float he = uSizeOver2.y;
	float r1 = uSizeOver2.x;
	float r2 = r1;
	if (uBottomScaler > 1)
		r1 /= uBottomScaler;
	else
		r2 *= uBottomScaler;
	float2 k1 = float2(r2, he);
	float2 k2 = float2(r2 - r1, 2.0 * he);
	p.x = abs(p.x);
	float2 ca = float2(p.x - min(p.x, (p.y < 0.0) ? r1 : r2), abs(p.y) - he);
	float2 cb = p - k1 + k2 * clamp(dot(k1 - p, k2) / dot2(k2), 0.0, 1.0);
	float s = (cb.x < 0.0 && ca.y < 0.0) ? -1.0 : 1.0;
	return s * sqrt(min(dot2(ca), dot2(cb)));
}
float Parallelogram(float2 p) //平行四边形 需要指定uBottomScaler-倾斜偏移占总宽的比例，取正为向右倾斜
{
	p -= uSizeOver2;
	float he = uSizeOver2.y;
	float wi = uSizeOver2.x * (1 - abs(uBottomScaler));
	float sk = uSizeOver2.x * uBottomScaler;
	
	float2 e = float2(sk, he);
	p = (p.y < 0.0) ? -p : p;
	float2 w = p - e;
	w.x -= clamp(w.x, -wi, wi);
	float2 d = float2(dot(w, w), -w.y);
	float s = p.x * e.y - p.y * e.x;
	p = (s < 0.0) ? -p : p;
	float2 v = p - float2(wi, 0);
	v -= e * clamp(dot(v, e) / dot(e, e), -1.0, 1.0);
	d = min(d, float2(dot(v, v), wi * he - abs(s)));
	return sqrt(d.x) * sign(-d.y);
}
float EquilateralTriangle(float2 p) //等边三角形
{
	p -= uSizeOver2 * float2(1.0, 0.6666667);
	float r = uSizeOver2.x;
	const float k = sqrt(3.0);
	p.x = abs(p.x) - r;
	p.y = p.y + r / k;
	if (p.x + k * p.y > 0.0)
		p = float2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
	p.x -= clamp(p.x, -2.0 * r, 0.0);
	return -length(p) * sign(p.y);
}
float TriangleIsosceles(float2 p) //等腰三角形
{
	float2 q = uSizeOver2 * float2(1.0, 2.0);
	p -= q;
	p.x = abs(p.x);
	p.y *= -1;
	float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0.0, 1.0);
	float2 b = p - q * float2(clamp(p.x / q.x, 0.0, 1.0), 1.0);
	float s = -sign(q.y);
	float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)),
                  float2(dot(b, b), s * (p.y - q.y)));
	return -sqrt(d.x) * sign(d.y);
}
float Triangle(float2 p) //任意三角形 需要指定 uStart|uEnd|uAnother-三角形三个顶点
{
	float2 p0 = uStart;
	float2 p1 = uEnd;
	float2 p2 = uAnother;
	float2 e0 = p1 - p0, e1 = p2 - p1, e2 = p0 - p2;
	float2 v0 = p - p0, v1 = p - p1, v2 = p - p2;
	float2 pq0 = v0 - e0 * clamp(dot(v0, e0) / dot(e0, e0), 0.0, 1.0);
	float2 pq1 = v1 - e1 * clamp(dot(v1, e1) / dot(e1, e1), 0.0, 1.0);
	float2 pq2 = v2 - e2 * clamp(dot(v2, e2) / dot(e2, e2), 0.0, 1.0);
	float s = sign(e0.x * e2.y - e0.y * e2.x);
	float2 d = min(min(float2(dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x)),
                     float2(dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x))),
                     float2(dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x)));
	return -sqrt(d.x) * sign(d.y);
}
float UnevenCapsule(float2 p) //水滴，小圆切线连大圆 需要指定 uRound.xy-两个圆的半径
{
	p -= uSizeOver2;
	p.y -= uRound.y;
	p.y *= -1;
	p.x = abs(p.x);
	float r1 = uRound.x;
	float r2 = uRound.y;
	float h = uSizeOver2.y;
	float b = (r1 - r2) / h;
	float a = sqrt(1.0 - b * b);
	float k = dot(p, float2(-b, a));
	if (k < 0.0)
		return length(p) - r1;
	if (k > a * h)
		return length(p - float2(0.0, h)) - r2;
	return dot(p, float2(a, b)) - r1;
}
float Pentagon(float2 p) //正五边形
{
	const float3 k = float3(0.809016994, 0.587785252, 0.726542528);
	//float r = uRound.x; //需要指定 uRound.x-等份的等腰三角形的高
	float r = uSizeOver2.y;
	p -= float2(uSizeOver2.x, uSizeOver2.y / k.x);
	p.x = abs(p.x);
	p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
	p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
	p -= float2(clamp(p.x, -r * k.z, r * k.z), r);
	return length(p) * sign(p.y);
}
float Hexagon(float2 p) //正六边形
{
	const float3 k = float3(-0.866025404, 0.5, 0.577350269);
	//float r = uRound.x; //需要指定 uRound.x-等份的等腰三角形的高
	float r = uSizeOver2.y;
	p -= float2(uSizeOver2.x, r);
	p = abs(p);
	p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
	p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
	return length(p) * sign(p.y);
}
float Octogon(float2 p) //正八边形
{
	const float3 k = float3(-0.9238795325, 0.3826834323, 0.4142135623);
	//float r = uRound.x; //需要指定 uRound.x-等份的等腰三角形的高
	float r = uSizeOver2.y;
	p -= float2(uSizeOver2.x, r);
	p = abs(p);
	p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
	p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
	p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
	return length(p) * sign(p.y);
}
float Ngon(float2 p) //正N边形，需要指定 uRound.x-边数，边需要大于等于三
{
	//float r = uRound.x;
	
	float an = 6.2831853 / uRound.x;
	
	float r = uSizeOver2.y;
	p -= uSizeOver2;
	
	float he = r * tan(0.5 * an);
    
    // rotate to first sector
	p = -p.yx; // if you want the corner to be up
	float bn = an * floor((atan2(p.y, p.x) + 0.5 * an) / an);
	float2 cs = float2(cos(bn), sin(bn));
	p = mul(float2x2(cs.x, cs.y, -cs.y, cs.x), p);

    // side of polygon
	return length(p - float2(r, clamp(p.y, -he, he))) * sign(p.x - r);
}
float Hexagram(float2 p) //六芒星
{
	const float4 k = float4(-0.5, 0.86602540378, 0.57735026919, 1.73205080757);
	
	float r = uSizeOver2.y * 0.5;
	p -= uSizeOver2;
    
	p = abs(p);
	p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
	p -= 2.0 * min(dot(k.yx, p), 0.0) * k.yx;
	p -= float2(clamp(p.x, r * k.z, r * k.w), r);
	return length(p) * sign(p.y);
}
float Star5(float2 p) //五角星，需要指定 uRound.x-内接圆的半径占外接圆的比例
{
	const float2 k1 = float2(0.809016994375, -0.587785252292);
	const float2 k2 = float2(-k1.x, k1.y);
	float r = uSizeOver2.y; // * 2.0 / (1.0 + k1.x)
	float rf = uRound.x;
	
	p -= float2(uSizeOver2.x, uSizeOver2.y * k1.x);
	
	p.x = abs(p.x);
	p -= 2.0 * max(dot(k1, p), 0.0) * k1;
	p -= 2.0 * max(dot(k2, p), 0.0) * k2;
	p.x = abs(p.x);
	p.y -= r;
	float2 ba = rf * float2(-k1.y, k1.x) - float2(0, 1);
	float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, r);
	return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}
float StarX(float2 p) //X角星，StarX是致敬东方幕华祭曲师及程序 需要指定 uRound.xy-边数 以及N边形时等腰三角形高的系数(0到1)
{
	float n = uRound.x;
	float w = uRound.y;
	
	float m = n + w * (2.0 - n);
	float an = 3.1415927 / n;
	float en = 3.1415927 / m;
	bool flag = (int) n % 2 == 0;
	float r = uSizeOver2.y * (flag ? 1.0 : 2 / (1 + cos(an)));

	// these 5 lines can be precomputed for a given shape
    //float m = n*(1.0-w) + w*2.0;
    

	float2 racs = r * float2(cos(an), sin(an));
	float2 ecs = float2(cos(en), sin(en)); // ecs=float2(0,1) and simplify, for regular polygon,
	p.x -= uSizeOver2.x;
	p.y -= flag ? r : racs.x;
    // symmetry (optional)
	p.x = abs(p.x);
    
    // reduce to first sector
	float bn = mod(atan2(p.x, p.y), 2.0 * an) - an;
	p = length(p) * float2(cos(bn), abs(sin(bn)));

    // line sdf
	p -= racs;
	p += ecs * clamp(-dot(p, ecs), 0.0, racs.y / ecs.y);
	return length(p) * sign(p.x);
}
float Pie(float2 p) //饼图 需要指定uRound.xyz-半圆心角的正余弦值及半径
{
	float r = uRound.z;
	float2 c = uRound.xy;
	if (c.y < 0)
		p.y += c.y * r;
	p.x -= uSizeOver2.x;
	p.x = abs(p.x);
	float l = length(p) - r;
	float m = length(p - c * clamp(dot(p, c), 0.0, r)); // c=sin/cos of aperture
	return max(l, m * sign(c.y * p.x - c.x * p.y));
}
float CutDisk(float2 p) //弓形 需要指定uRound.xy-从-1到1从无到整圆以及圆半径
{
	float r = uRound.y;
	float h = uRound.x;
	float w = sqrt(1 - h * h) * r; // constant for any given shape
	h *= -r;
	p.x -= uSizeOver2.x;
	p.y += h;
	
	p.x = abs(p.x);
	
	float s = max((h - r) * p.x * p.x + w * w * (h + r - 2.0 * p.y), h * p.x - w * p.y);
	return (s < 0.0) ? length(p) - r :
           (p.x < w) ? h - p.y :
                     length(p - float2(w, h));
}
float Arc(float2 p) //圆弧 需要指定uRound-圆心角正余弦值 圆弧半径 线条半径
{
	// sc is the sin/cos of the arc's aperture
	float2 sc = uRound.xy;
	float ra = uRound.z;
	float rb = uRound.w;
	
	p.x -= uSizeOver2.x;
	p.y -= rb - ra * uRound.y;
	
	p.x = abs(p.x);
	return ((sc.y * p.x > sc.x * p.y) ? length(p - sc * ra) : abs(length(p) - ra)) - rb;
}
float Ring(float2 p) //圆环 需要指定uRound-半径 宽度 圆心角余正弦值
{
	//TODO 实现中心偏移
	float r = uRound.x;
	float th = uRound.y;
	float2 n = uRound.zw;
	p.y += n.x * (r - th * .5 * sign(n.x));
	p.x -= uSizeOver2.x;
	p.x = abs(p.x);
   
	p = mul(float2x2(n.x, -n.y, n.y, n.x), p);

	return max(abs(length(p) - r) - th * 0.5, length(float2(p.x, max(0.0, abs(r - p.y) - th * 0.5))) * sign(p.x));
}
float Horseshoe(float2 p) //马蹄铁 需要指定uRound-半径 延长量 圆心角余正弦值 uLineWidth-线宽
{
	float r = uRound.x;
	float2 w = float2(uRound.y, uLineWidth);
	float2 c = uRound.zw;
	
	p -= uSizeOver2; //这里传入的时候已经偏转到中心了
	
	p.x = abs(p.x);
	float l = length(p);
	p = mul(float2x2(-c.x, c.y, c.y, c.x), p);
	p = float2((p.y > 0.0 || p.x > 0.0) ? p.x : l * sign(-c.x),
             (p.x > 0.0) ? p.y : l);
	p = float2(p.x, abs(p.y - r)) - w;
	return length(max(p, 0.0)) + min(0.0, max(p.x, p.y));
}
float Vesica(float2 p)//鱼鳔形(两圆交集) 需要指定uRound.xy-两圆半径以及坐标
{
	float r = uRound.x;
	float d = uRound.y;
	p -= uSizeOver2;
	p = abs(p);
	float b = sqrt(r * r - d * d); // can delay this sqrt by rewriting the comparison
	return ((p.y - b) * d > p.x * b) ? length(p - float2(0.0, b)) * sign(d)
                               : length(p - float2(-d, 0.0)) - r;
}
float OrientedVesica(float2 p)//正交鱼鳔 需要指定uStart|uEnd-两个端点坐标 uRound.x-宽度   //uStart|uEnd-两圆心坐标 uRound.x-两圆半径
{
	float2 a = uStart;
	float2 b = uEnd;
	float w = uRound.x;
	float r = 0.5 * length(b - a);
	float d = 0.5 * (r * r - w * w) / w;
	float2 v = (b - a) / r;
	float2 c = (b + a) * 0.5;
	float2 q = 0.5 * abs(mul(float2x2(v.y, -v.x, v.x, v.y), p - c));
	float3 h = (r * q.x < d * (q.y - r)) ? float3(0.0, r, 0.0) : float3(-d, 0.0, d + w);
	return length(q - h.xy) - h.z;
}
float Moon(float2 p)//月牙 需要指定uRound.xyz-大小圆半径以及横坐标
{
	float ra = uRound.x;
	float rb = uRound.y;
	float d = uRound.z;
	p -= uSizeOver2;
	p.y = abs(p.y);
	float a = (ra * ra - rb * rb + d * d) / (2.0 * d);
	float b = sqrt(max(ra * ra - a * a, 0.0));
	if (d * (p.x * b - p.y * a) > d * d * max(b - p.y, 0.0))
		return length(p - float2(a, b));
	return max((length(p) - ra),
               -(length(p - float2(d, 0)) - rb));
}
float CircleCross(float2 p)//曲边四芒星 需要指定uRound.xyz-半径、宽高比、原始宽
{
	float s = uRound.z; //uSizeOver2 - uRound.xx;
	p -= uSizeOver2;
	p /= s;
	
	float h = uRound.y;
	float k = 0.5 * (h + 1.0 / h);
	p = abs(p);
	float result = (p.x < 1.0 && p.y < p.x * (k - h) + h) ?
             k - sqrt(dot2(p - float2(1, k))) :
           sqrt(min(dot2(p - float2(0, h)),
                    dot2(p - float2(1, 0))));
	return result * s - uRound.x;
}
float Egg(float2 p)//蛋 需要指定uRound.xyz-大小圆半径以及距离
{
	float ra = uRound.x;
	float rb = uRound.y;
	float he = uRound.z;
	p -= uSizeOver2;
	
	float ce = 0.5 * (he * he - (ra - rb) * (ra - rb)) / (ra - rb);

	p.x = abs(p.x);

	if (p.y < 0.0)
		return length(p) - ra;
	if (p.y * ce - p.x * he > he * ce)
		return length(float2(p.x, p.y - he)) - rb;
	return length(float2(p.x + ce, p.y)) - (ce + ra);
}
float Heart(float2 p)//心 
{
	p -= uSizeOver2;
	float k = uSizeOver2 / 0.60355; //(1 + sqrt(2))/4
	p /= k;
	p.x = abs(p.x);
	if (p.y + p.x > 1.0)
		return (sqrt(dot2(p - float2(0.25, 0.75))) - sqrt(2.0) / 4.0) * k;
	return sqrt(min(dot2(p - float2(0.00, 1.00)), dot2(p - 0.5 * max(p.x + p.y, 0.0)))) * sign(p.x - p.y) * k;
}
float Plus(float2 p)//十字 需要指定uRound.xyz-正方形顶点坐标 圆心坐标 半径(y=x直线上)
{
	float2 b = uRound.xy;
	float r = uRound.z;
	p -= uSizeOver2;
	
	p = abs(p);
	if (p.y > p.x)
		p = p.yx;
	float2 q = p - b;
	float k = max(q.y, q.x);
	float2 w = (k > 0.0) ? q : float2(b.y - p.x, -k);
	float d = length(max(w, 0.0));
	return ((k > 0.0) ? d : -d) + r;
	
	/*p = abs(p);
	p = (p.y > p.x) ? p.yx : p.xy;
	float2 q = p - b;
	float k = max(q.y, q.x);
	float2 w = (k > 0.0) ? q : float2(b.y - p.x, -k);
	return sign(k) * length(max(w, 0.0)) + r;*/
}
float Cross(float2 p) //叉叉，需要指定 uRound.xy-半径以及宽度
{
	p = abs(p - uSizeOver2);
	float d = length(p - min(p.x + p.y, uRound.y) * 0.5) - uRound.x + uInnerShrinkage;
	return d;
}
float Polygon(float2 p) //多边形，需要指定uCurrentPointCount-当前顶点数 uVectors-顶点坐标 最多支持20边形
{
	int N = uCurrentPointCount;
	float2 v[MAXPOLYGONPOINT] = uVectors;
	float d = dot2(p - v[0]);
	float s = 1.0;
	for (int i = 0, j = N - 1; i < N; j = i, i++)
	{
		float2 e = v[j] - v[i];
		float2 w = p - v[i];
		float2 b = w - e * saturate(dot(w, e) / dot2(e));
		d = min(d, dot2(b));
		bool3 c = bool3(p.y >= v[i].y, p.y < v[j].y, e.x * w.y > e.y * w.x);
		if (all(c) || all(!c))
			s = -s;
	}
	return s * sqrt(d);
}
float Ellipse(float2 p) //椭圆
{
	float2 ab = uSizeOver2;
	p -= uSizeOver2;
	p = abs(p);
	if (p.x > p.y)
	{
		p = p.yx;
		ab = ab.yx;
	}
	float l = ab.y * ab.y - ab.x * ab.x;
	float m = ab.x * p.x / l;
	float m2 = m * m;
	float n = ab.y * p.y / l;
	float n2 = n * n;
	float c = (m2 + n2 - 1.0) / 3.0;
	float c3 = c * c * c;
	float q = c3 + m2 * n2 * 2.0;
	float d = c3 + m2 * n2;
	float g = m + m * n2;
	float co;
	if (d < 0.0)
	{
		float h = acos(q / c3) / 3.0;
		float s = cos(h);
		float t = sin(h) * sqrt(3.0);
		float rx = sqrt(-c * (s + t + 2.0) + m2);
		float ry = sqrt(-c * (s - t + 2.0) + m2);
		co = (ry + sign(l) * rx + abs(g) / (rx * ry) - m) / 2.0;
	}
	else
	{
		float h = 2.0 * m * n * sqrt(d);
		float s = sign(q + h) * pow(abs(q + h), 1.0 / 3.0);
		float u = sign(q - h) * pow(abs(q - h), 1.0 / 3.0);
		float rx = -s - u - c * 4.0 + 2.0 * m2;
		float ry = (s - u) * sqrt(3.0);
		float rm = sqrt(rx * rx + ry * ry);
		co = (ry / sqrt(rm - rx) + 2.0 * g / rm - m) / 2.0;
	}
	float2 r = ab * float2(co, sqrt(1.0 - co * co));
	return length(r - p) * sign(p.y - r.y);
}
float Parabola(float2 p)
{
	float k = uRound.x;
	p -= uSizeOver2;
	p.x = abs(p.x);
	float ik = 1.0 / k;
	float t = ik * (p.y - 0.5 * ik) / 3.0;
	float q = 0.25 * ik * ik * p.x;
	float h = q * q - t * t * t;
	float r = sqrt(abs(h));
	float x = (h > 0.0) ?
        pow(q + r, 1.0 / 3.0) - pow(abs(q - r), 1.0 / 3.0) * sign(r - q) :
        2.0 * cos(atan2(r, q) / 3.0) * sqrt(t);
	return length(p - float2(x, k * x * x)) * sign(p.x - x);
}
float ParabolaSegment(float2 p)
{
	float wi = uRound.x;
	float he = uRound.y;
	p -= uSizeOver2;
	p.x = abs(p.x);
	float ik = wi * wi / he;
	float k = ik * (he - p.y - 0.5 * ik) / 3.0;
	float q = p.x * ik * ik * 0.25;
	float h = q * q - k * k * k;
	float r = sqrt(abs(h));
	float x = (h > 0.0) ?
        pow(q + r, 1.0 / 3.0) - pow(abs(q - r), 1.0 / 3.0) * sign(r - q) :
        2.0 * cos(atan(r / q) / 3.0) * sqrt(k);
	x = min(x, wi);
	return length(p - float2(x, he - x * x / ik)) *
           sign(ik * (p.y - he) + p.x * p.x);
}
float QuadraticBezier(float2 p)
{
	return bezier(p, uStart, uAnother, uEnd) - uLineWidth;
}
float BlobbyCross(float2 p)//需要指定uRound.xyz-基准大小 控制点系数 半径
{
	float s = uRound.x;
	float he = uRound.y;
	float r = uRound.z;
	
	p -= uSizeOver2;
	p /= s;
	
	p = abs(p);
	p = float2(abs(p.x - p.y), 1.0 - p.x - p.y) / sqrt(2.0);

	float t = (he - p.y - 0.25 / he) / (6.0 * he);
	float q = p.x / (he * he * 16.0);
	float h = q * q - t * t * t;
    
	float x;
	if (h > 0.0)
	{
		float r = sqrt(h);
		x = pow(q + r, 1.0 / 3.0) - pow(abs(q - r), 1.0 / 3.0) * sign(r - q);
	}
	else
	{
		float r = sqrt(t);
		x = 2.0 * r * cos(acos(q / (t * r)) / 3.0);
	}
	x = min(x, sqrt(2.0) / 2.0);
    
	float2 z = float2(x, he * (1.0 - 2.0 * x * x)) - p;
	return length(z) * sign(z.y) * s - r;
}
float Tunnel(float2 p)
{
	float2 wh = uSizeOver2;
	p -= wh;
	p.x = abs(p.x);
	p.y = -p.y;
	float2 q = p - wh;

	float d1 = dot2(float2(max(q.x, 0.0), q.y));
	q.x = (p.y > 0.0) ? q.x : length(p) - wh.x;
	float d2 = dot2(float2(q.x, max(q.y, 0.0)));
	float d = sqrt(min(d1, d2));
    
	return (max(q.x, q.y) < 0.0) ? -d : d;
}
float Stairs(float2 p)
{
	int n = uRound.x;
	float2 wh = uSizeOver2;
	float2 ba = wh * n;
	float d = min(dot2(p - float2(clamp(p.x, 0.0, ba.x), 0.0)),
                  dot2(p - float2(ba.x, clamp(p.y, 0.0, ba.y))));
	float s = sign(max(-p.y, p.x - ba.x));

	float dia = length(wh);
	p = mul(float2x2(wh.x, wh.y, -wh.y, wh.x), p) / dia;
	float id = clamp(round(p.x / dia), 0.0, n - 1.0);
	p.x = p.x - id * dia;
	p = mul(float2x2(wh.x, -wh.y, wh.y, wh.x), p) / dia;

	float hh = wh.y / 2.0;
	p.y -= hh;
	if (p.y > hh * sign(p.x))
		s = 1.0;
	if (id >= 0.5 && p.x <= 0.0)
		p = -p;
	//p = (id < 0.5 || p.x > 0.0) ? p : -p;
	d = min(d, dot2(p - float2(0.0, clamp(p.y, -hh, hh))));
	d = min(d, dot2(p - float2(clamp(p.x, 0.0, wh.x), hh)));
    
	return sqrt(d) * s;
}
float QuadraticCircle(float2 p)
{
	float r = uSizeOver2.x;
	p -= uSizeOver2;
	p /= r;
	p = abs(p);
	if (p.y > p.x)
		p = p.yx;

	float a = p.x - p.y;
	float b = p.x + p.y;
	float c = (2.0 * b - 1.0) / 3.0;
	float h = a * a + c * c * c;
	float t;
	if (h >= 0.0)
	{
		h = sqrt(h);
		t = sign(h - a) * pow(abs(h - a), 1.0 / 3.0) - pow(h + a, 1.0 / 3.0);
	}
	else
	{
		float z = sqrt(-c);
		float v = acos(a / (c * z)) / 3.0;
		t = -z * (cos(v) + sin(v) * 1.732050808);
	}
	t *= 0.5;
	float2 w = float2(-t, t) + 0.75 - t * t - p;
	return length(w) * sign(a * a * 0.5 + b - 1.5) * r;
}
float Hyperbola(float2 p) //等轴双曲线，需要指定uRound.x=k,内接圆半径为sqrt(2 * k)
{
	float k = uRound.x;
	float he = uSizeOver2.y;
	p -= uSizeOver2;
	p = abs(p);
	p = float2(p.x - p.y, p.x + p.y) / sqrt(2.0);

	float x2 = p.x * p.x / 16.0;
	float y2 = p.y * p.y / 16.0;
	float r = k * (4.0 * k - p.x * p.y) / 12.0;
	float q = (x2 - y2) * k * k;
	float h = q * q + r * r * r;
	float u;
	if (h < 0.0)
	{
		float m = sqrt(-r);
		u = m * cos(acos(q / (r * m)) / 3.0);
	}
	else
	{
		float m = pow(sqrt(h) - q, 1.0 / 3.0);
		u = (m - r / m) / 2.0;
	}
	float w = sqrt(u + x2);
	float b = k * p.y - x2 * p.x * 2.0;
	float t = p.x / 4.0 - w + sqrt(2.0 * x2 - u + b / w / 4.0);
	t = max(t, sqrt(he * he * 0.5 + k) - he / sqrt(2.0));
	float d = length(p - float2(t, k / t));
	return p.x * p.y < k ? d : -d;
}
float CoolS(float2 p)
{
	p -= uSizeOver2;
	p /= uSizeOver2.y;
	float six = (p.y < 0.0) ? -p.x : p.x;
	p.x = abs(p.x);
	p.y = abs(p.y) - 0.2;
	float rex = p.x - min(round(p.x / 0.4), 0.4);
	float aby = abs(p.y - 0.2) - 0.6;
    
	float d = dot2(float2(six, -p.y) - clamp(0.5 * (six - p.y), 0.0, 0.2));
	d = min(d, dot2(float2(p.x, -aby) - clamp(0.5 * (p.x - aby), 0.0, 0.4)));
	d = min(d, dot2(float2(rex, p.y - clamp(p.y, 0.0, 0.4))));
    
	float s = 2.0 * p.x + aby + abs(aby + 0.4) - 0.4;
	return sqrt(d) * sign(s) * uSizeOver2.y;
}
float CircleWave(float2 p) //圆波 需要指定uRound.xy-圆弧占5/6完整圆的比例 半径 uLineWidth-线宽
{
	p -= uSizeOver2;
	float tb = uRound.x;
	float ra = uRound.y;
	tb = 3.1415927 * 5.0 / 6.0 * max(tb, 0.0001);
	float2 co = ra * float2(sin(tb), cos(tb));
	p.x = abs(mod(p.x, co.x * 4.0) - co.x * 2.0);
	float2 p1 = p;
	float2 p2 = float2(abs(p.x - 2.0 * co.x), -p.y + 2.0 * co.y);
	float d1 = ((co.y * p1.x > co.x * p1.y) ? length(p1 - co) : abs(length(p1) - ra));
	float d2 = ((co.y * p2.x > co.x * p2.y) ? length(p2 - co) : abs(length(p2) - ra));
	return min(d1, d2) - uLineWidth;
}
float ChainedQuadraticBezier(float2 p)
{
	//float d = -1.0;
	float d = 100000.0;
	int N = uCurrentPointCount;
	float2 v[MAXBEZIERPOINT] = uBVectors;
	int r = 0;
	//float s = 1.0;
	for (int n = 0; n < N - 2; n+=2)
	{
		float2 A = v[n];
		float2 B = v[n + 1];
		float2 C = v[n + 2];
		float cd = bezier(p, A, B, C);
		if ( d > cd)
			d = cd;
		//s *= QRoot(p, A, B, C);
		r += QRoot(p, A, B, C);
	}
	
	//return LRoot(p,v[0],v[N-1]) * s * d;
	r += LRoot(p, v[0], v[N - 1]);
	return (1.0 - 2.0 * float(r % 2)) * d;
}
//这两个函数负责色色
//把SDF塞进去就对了√
float4 GetHasBorderColor(float distance)
{
	return lerp(lerp(uBackgroundColor, uBorderColor, smoothstep(uTransition.x, uTransition.y, distance + uBorder)), 0, smoothstep(uTransition.x, uTransition.y, distance));
}
float4 GetNoBorderColor(float distance)
{
	return lerp(uBackgroundColor, 0, smoothstep(uTransition.x, uTransition.y, distance));
}
float4 GetBarColor(float distance)
{
	float4 color = tex2D(uImage0, float2(uTime + distance * uValueScaler, 0.0));
	return lerp(color, 0, smoothstep(uTransition.x, uTransition.y, distance));
}

DEFINE_FUNCTION(Circle)
DEFINE_FUNCTION(RoundedBox)
DEFINE_FUNCTION(Box)
DEFINE_FUNCTION(OrientedBox)
DEFINE_FUNCTION(Segment)
DEFINE_FUNCTION(Rhombus)
DEFINE_FUNCTION(Trapezoid)
DEFINE_FUNCTION(Parallelogram)
DEFINE_FUNCTION(EquilateralTriangle)
DEFINE_FUNCTION(TriangleIsosceles)
DEFINE_FUNCTION(Triangle)
DEFINE_FUNCTION(UnevenCapsule)
DEFINE_FUNCTION(Pentagon)
DEFINE_FUNCTION(Hexagon)
DEFINE_FUNCTION(Octogon)
DEFINE_FUNCTION(Ngon)
DEFINE_FUNCTION(Hexagram)
DEFINE_FUNCTION(Star5)
DEFINE_FUNCTION(StarX)
DEFINE_FUNCTION(Pie)
DEFINE_FUNCTION(CutDisk)
DEFINE_FUNCTION(Arc)
DEFINE_FUNCTION(Ring)
DEFINE_FUNCTION(Horseshoe)
DEFINE_FUNCTION(Vesica)
DEFINE_FUNCTION(OrientedVesica)
DEFINE_FUNCTION(Moon)
DEFINE_FUNCTION(CircleCross)
DEFINE_FUNCTION(Egg)
DEFINE_FUNCTION(Heart)
DEFINE_FUNCTION(Plus)
DEFINE_FUNCTION(Cross)
DEFINE_FUNCTION(Polygon)
DEFINE_FUNCTION(Ellipse)
DEFINE_FUNCTION(Parabola)
DEFINE_FUNCTION(ParabolaSegment)
DEFINE_FUNCTION(QuadraticBezier)
DEFINE_FUNCTION(BlobbyCross)
DEFINE_FUNCTION(Tunnel)
DEFINE_FUNCTION(Stairs)
DEFINE_FUNCTION(QuadraticCircle)
DEFINE_FUNCTION(Hyperbola)
DEFINE_FUNCTION(CoolS)
DEFINE_FUNCTION(CircleWave)
DEFINE_FUNCTION(ChainedQuadraticBezier)
technique Technique1
{
	DEFINE_PASS(Circle)
	DEFINE_PASS(RoundedBox)
	DEFINE_PASS(Box)
	DEFINE_PASS(OrientedBox)
	DEFINE_PASS(Segment)
	DEFINE_PASS(Rhombus)
	DEFINE_PASS(Trapezoid)
	DEFINE_PASS(Parallelogram)
	DEFINE_PASS(EquilateralTriangle)
	DEFINE_PASS(TriangleIsosceles)
	DEFINE_PASS(Triangle)
	DEFINE_PASS(UnevenCapsule)
	DEFINE_PASS(Pentagon)
	DEFINE_PASS(Hexagon)
	DEFINE_PASS(Octogon)
	DEFINE_PASS(Ngon)
	DEFINE_PASS(Hexagram)
	DEFINE_PASS(Star5)
	DEFINE_PASS(StarX)
	DEFINE_PASS(Pie)
	DEFINE_PASS(CutDisk)
	DEFINE_PASS(Arc)
	DEFINE_PASS(Ring)
	DEFINE_PASS(Horseshoe)
	DEFINE_PASS(Vesica)
	DEFINE_PASS(OrientedVesica)
	DEFINE_PASS(Moon)
	DEFINE_PASS(CircleCross)
	DEFINE_PASS(Egg)
	DEFINE_PASS(Heart)
	DEFINE_PASS(Plus)
	DEFINE_PASS(Cross)
	DEFINE_PASS(Polygon)
	DEFINE_PASS(Ellipse)
	DEFINE_PASS(Parabola)
	DEFINE_PASS(ParabolaSegment)
	DEFINE_PASS(QuadraticBezier)
	DEFINE_PASS(BlobbyCross)
	DEFINE_PASS(Tunnel)
	DEFINE_PASS(Stairs)
	DEFINE_PASS(QuadraticCircle)
	DEFINE_PASS(Hyperbola)
	DEFINE_PASS(CoolS)
	DEFINE_PASS(CircleWave)
	DEFINE_PASS(ChainedQuadraticBezier)
}



//综合在一起就是形形色色(
//↓分支过多诉讼
/*float4 SDFShape(float2 coords : TEXCOORD0) : COLOR0
{
	float sdf = 0;
	switch (uStyle)
	{
        case 0:
			sdf = Circle(coords);
			break;
		case 1:
			sdf = RoundedBox(coords);
			break;
		case 2:
			sdf = Box(coords);
			break;
		case 3:
			sdf = OrientedBox(coords);
			break;
		case 4:
			sdf = Segment(coords);
			break;
		case 5:
			sdf = Rhombus(coords);
			break;
		case 6:
			sdf = Trapezoid(coords);
			break;
		case 7:
			sdf = Parallelogram(coords);
			break;
		case 8:
			sdf = EquilateralTriangle(coords);
			break;
		case 9:
			sdf = TriangleIsosceles(coords);
			break;
		case 10:
			sdf = Triangle(coords);
			break;
		case 11:
			sdf = UnevenCapsule(coords);
			break;
		case 12:
		default:
			sdf = CrossShape(coords);
			break;
	}
	return uHasBorder ? GetBorderedColor(sdf, false) : GetNoBorderColor(sdf, false);

}*/