float dia(in float2 p, in float time) {
	float a = atan2(p.y, p.x);
	float s = floor((abs(p.x) + abs(p.y)) * 100.0);
	s *= sin(s * 24.0);
	float s2 = frac(sin(s));

	float c = step(1.0, tan(a + s + s2 * time * 0.1) * 10.5 + 0.5);

	c *= s2 * 0.5 + 0.1;
	return c;
}

void Skin3_float(in float2 uv,in float time, out float3 col)
{
	float2 p = uv*0.1;

	float s = sin(time * 100.0) * cos(time * 120.0 + 32.0);
	float ss = s > 0.9 ? s * sin(floor(p.y * 32.0) / 32.0) * 0.4 - 0.2 : 0.0;
	ss *= 0.5;
	time *= 20;
	float3 color = float3(
		dia(float2(p.x + ss * floor(p.y * 16.0) / 16.0, p.y),time),
		dia(p + 0.2,time),
		dia(p-0.2,time)
	);
	float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
	col = float3(gray,gray,gray);
	
}