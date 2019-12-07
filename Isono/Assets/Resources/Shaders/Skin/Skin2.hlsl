#define PI 3.14159265359

float random(float n) {
	return frac(abs(sin(n * 55.753) * 367.34));
}

float random(float2 n) {
	return random(dot(n, float2(2.46, -1.21)));
}

float patternize(float n) {
	// Fade by cos()
	return cos(frac(n) * 2.0 * PI);
}

float2 shear(float2 st, float radian) {
	return (st + float2(st.y, 0.0) * cos(radian)) + float2(floor(4.0 * (st.x - st.y * cos(radian))), 0.0);
}

void Skin2_float(in float2 uv,in float time, out float3 col)
{
	float2 st = uv;

	float radian = radians(60.0);
	float scale = 1.0;

	st *= scale;

	float2 newSt = shear(st, radian);

	float n = patternize(random(floor(newSt * 4.0)) + random(floor(newSt * 2.0)) + random(floor(newSt)) + time * 0.3);

	float3 color = float3(n * 0.5, n * 1.5, 0.8);
	float gray = dot(color.rgb, float3(0.299, 0.587, 0.114));
	col = float3(gray,gray,gray);
	
}