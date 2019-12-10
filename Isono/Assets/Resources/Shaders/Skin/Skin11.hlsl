
void Skin11_float(in float2 uv, in float time, float noise, out float3 col)
{
	float2 p = uv;

	p *= 0.75;

	float a = atan2(p.y, p.x);
	float r = sqrt(dot(p, p));

	a += sin(0.5*r - 1.0*time);

	float h = 0.5 + 0.5*cos(18.0*a);

	float s = smoothstep(0.4, 0.5, h);

	float2 uv1;
	uv1.x = time + 1.0 / (r + .1*s);
	uv1.y = 3.0*a / 3.1416;

	float3 color = float3(0.6,0.6,0.6);

	float ao = smoothstep(0.0, 0.9, h) - smoothstep(0.9, 1.0, h);
	color *= 1.0 - 0.8*ao*r;
	color *= r;
	
	col = color*color;
	col = pow(col, 0.5);
	col = clamp(0.0, 1.0,col);
	col = 1 - col;
	col = pow(col*col, 3.5);
}