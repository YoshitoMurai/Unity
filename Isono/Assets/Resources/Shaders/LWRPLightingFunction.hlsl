float distLine(float2 p, float2 a, float2 b) {
	float2 ap = p - a;
	float2 ab = b - a;
	float aDotB = clamp(dot(ap, ab) / dot(ab, ab), 0.0, 1.0);
	return length(ap - ab * aDotB);
}

float drawLine(float2 uv, float2 a, float2 b) {
	float l = smoothstep(0.014f, 0.01f, distLine(uv, a, b));
	float dist = length(b - a);
	return l * (smoothstep(1.3, 0.8, dist) * 0.5 + smoothstep(0.04, 0.03, abs(dist - 0.75)));
}

float n21(float2 i) {
	i += frac(i * float2(223.64, 823.12));
	i += dot(i, i + 23.14);
	return frac(i.x * i.y);
}

float2 n22(float2 i) {
	float x = n21(i);
	return float2(x, n21(i + x));
}

float2 getPoint(float2 id, float2 offset) {
	return offset + sin(n22(id + offset) * (_Time+5) * 1.0) * 0.4;
}

float layer(float2 uv) {
	float m = 0.0;
	float t = _Time * 2.0;

	float2 gv = frac(uv) - 0.5;
	float2 id = floor(uv) - 0.5;

	float2 p[9];
	int i = 0;
	for (float y = -1.0; y <= 1.0; y++) {
		for (float x = -1.0; x <= 1.0; x++) {
			p[i++] = getPoint(id, float2(x, y));
		}
	}

	for (int i = 0; i < 9; i++) {
		m += drawLine(gv, p[4], p[i]);
		float sparkle = 1.0 / pow(length(gv - p[i]), 1.5) * 0.005;
		m += sparkle * (sin(t + frac(p[i].x) * 12.23) * 0.4 + 0.6);
	}

	m += drawLine(gv, p[1], p[3]);
	m += drawLine(gv, p[1], p[5]);
	m += drawLine(gv, p[7], p[3]);
	m += drawLine(gv, p[7], p[5]);

	return m;
}

void Plexus_float (in float2 uv, out float3 col)
{
	float2 r = float2(540, 960);
	float3 c = sin(2.0 * float3(.234, .324, .768)) * 0.4 + 0.6;
	col = float3(0.4,0.4,0.5);
	c.x += (uv.x + 0.5);

	float m = 0.0;

	float2x2 rotMat = float2x2(1, 0, -0, 1);
	uv = mul(rotMat,uv);

	for (float i = 0.0; i <= 1.0; i += 1.0 / 4.0) {
		float z = frac(i);
		float size = lerp(15.0, .1, z) * 0.80;
		float fade = smoothstep(0.0, 1.0, z) * smoothstep(1.0, 0.9, z);
		m += layer((size * uv) + i * 10.0) * fade;
	}

	col += m;
	col += abs(uv.x*1.5) * 0.5;
	col *= float3(0, 0.25, 0.3);
}