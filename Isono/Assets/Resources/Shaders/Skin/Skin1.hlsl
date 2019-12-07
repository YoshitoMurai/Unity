float hex(float2 p)
{
	p.x *= 0.57735;
	p = abs((fmod(p, 0.8) - 0.4));
	return abs(max(p.x + p.x, p.y) - 0.5);
}

void Skin1_float(in float2 uv, in float2 res,in float time, out float3 col)
{
	float2 pos = uv.xy;
	float2 p = 40.0f * pos;
	float s = sin(dot(p, p) / -64. + time);
	s = pow(abs(s), 0.5) * sign(s);
	float  r = .35 + .25 * s;
	float t =0.8;
	p =  mul(p,float2x2(cos(t), -sin(t), sin(t), cos(t)));
	float f = hex(p*r)*0.5+s;
	col = float3(f, f, f);
	
}