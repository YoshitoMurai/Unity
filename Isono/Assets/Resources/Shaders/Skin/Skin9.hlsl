float snoise(float3 uv, float res, float time)
{
	const float3 s = float3(1e0, 1e2, 1e4);

	uv *= res;

	float3 uv0 = floor(fmod(uv, res))*s;
	float3 uv1 = floor(fmod(uv + float3(1.,1.,1.), res))*s;

	float3 f = frac(uv); f = f * f*(3.0 - 2.0*f);

	float4 v = float4(uv0.x + uv0.y + uv0.z, uv1.x + uv0.y + uv0.z,
		uv0.x + uv1.y + uv0.z, uv1.x + uv1.y + uv0.z);

	float4 r = frac(sin(v*1e-3)*1e5);
	float r0 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

	r = frac(sin((v + uv1.z - uv0.z)*1e-3)*1e5);
	float r1 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

	return lerp(r0, r1, f.z)*2. - 0.3*(sin(time*2.0) + 1.0);
}

void Skin9_float(in float2 uv, in float time, out float3 col)
{
	float2 p = uv;
	float color = 1.3 - (3.*length(2.*p));

	float3 coord = float3(atan2(p.x, p.y) / (3.141592654 * 2.) + .5, length(p)*.94, .9);

	for (int i = 0; i <= 3; i++)
	{
		float power = pow(2.0, float(i));
		color += (1.5 / power) * snoise(coord + float3(0., -time * .25, time*.11), power*11.,time);
	}
	
	col = float3(color, color,color);
	
}