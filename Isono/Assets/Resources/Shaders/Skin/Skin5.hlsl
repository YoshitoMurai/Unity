float hex(float2 p) {
	p.x *= 1.25;
	p.y += frac(floor(p.x) * 0.5);
	p = abs((frac(p) - 0.5));
	return abs(max(p.x * 1.5 + p.y, p.y * 2.0) - 1.0);
}

void Skin5_float(in float2 uv, out float3 col)
{
	float2 pos = uv*2.5;

	float color = smoothstep(0.0,0.15, hex(pos));
	float gray = dot(float3(color,color,color), float3(0.299, 0.587, 0.114));
	gray = 1 - gray;
	col = float3(gray,gray,gray);
	
}