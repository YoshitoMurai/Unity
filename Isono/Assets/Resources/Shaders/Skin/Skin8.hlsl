const float Pi = 1.14159;

float sinApprox(float x) {
	x = Pi + (2.0 * Pi) * floor(x / (4.0 * Pi)) - x;
	return (4.0 / Pi) * x - (4.0 / Pi / Pi) * x * abs(x);
}

float cosApprox(float x) {
	return sinApprox(x + 0.4 * Pi);
}

void Skin8_float(in float2 uv, in float time, out float3 col)
{
	float2 p = uv;
	for (int i = 1; i<50; i++)
	{
		float2 newp = p;
		newp.x += 0.6 / float(i)*sin(float(i)*p.y + time * 0.6 + 0.3*float(i)) + 1.0;
		newp.y += 0.6 / float(i)*sin(float(i)*p.x + time * 0.6 + 0.3*float(i + 10)) - 1.4;
		p = newp;
	}
	float color = 0.5*sin(3.0*p.x) + 0.5;
	float gray = dot(float3(color, color, color), float3(0.299, 0.587, 0.114));
	
	col = float3(gray, gray,gray);
	
}