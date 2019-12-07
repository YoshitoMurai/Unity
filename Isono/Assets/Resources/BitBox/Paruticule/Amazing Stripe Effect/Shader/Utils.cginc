// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#ifndef UTILS_CGINC
#define UTILS_CGINC

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform sampler2D _BumpTex;
uniform sampler2D _StripeTex;
uniform float4 _StripeColor;
uniform float _StripeWidth;
uniform float _StripeDensity;
uniform float3 _StripeMove;
uniform float _HologramClip;

struct v2f
{
	float4 pos     : SV_POSITION;
	float4 uvw     : TEXCOORD0;
	float2 tex     : TEXCOORD1;
	float3 tgsnor  : TEXCOORD2;
	float3 tgslit  : TEXCOORD3;
	float3 tgsview : TEXCOORD4;
	float4 scrpos  : TEXCOORD5;
};
v2f vert (appdata_tan v)
{
	TANGENT_SPACE_ROTATION;
	
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
//	float4 p = mul(_Object2World, v.vertex);  // control stripe move with _StripeMove
	float4 p = v.vertex;
	o.uvw = p * _StripeDensity;
	o.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
	o.tgsnor = mul(rotation, SCALED_NORMAL);
	o.tgslit = mul(rotation, ObjSpaceLightDir(v.vertex));
	o.tgsview = mul(rotation, ObjSpaceViewDir(v.vertex));
	o.scrpos = ComputeScreenPos(o.pos);
	return o;
}
float4 frag (v2f i) : SV_TARGET
{
	// stripe
	float t = _Time.y;
	float stripex = tex2D(_StripeTex, float2(i.uvw.x + t * _StripeMove.x, 1 - _StripeWidth)).x;
	float stripey = tex2D(_StripeTex, float2(i.uvw.y + t * _StripeMove.y, 1 - _StripeWidth)).x;
	float stripez = tex2D(_StripeTex, float2(i.uvw.z + t * _StripeMove.z, 1 - _StripeWidth)).x;

	float checker = 0;
#if STRIPE_X
	checker = stripex;
#elif STRIPE_Y
	checker = stripey;
#elif STRIPE_Z
	checker = stripez;
#elif STRIPE_XY
	checker = stripex * stripey;
#elif STRIPE_XZ
	checker = stripex * stripez;
#elif STRIPE_YZ
	checker = stripey * stripez;
#elif STRIPE_XYZ
	checker = stripex * stripey * stripez;
#endif

#ifdef STRIPE_TRANSPARENCY
	return float4(_StripeColor.rgb, 1 - checker);
#endif

	// light + texture
	float4 base = tex2D(_MainTex, i.tex);
#ifdef STRIPE_TEXTURE
	return lerp(base, _StripeColor, 1 - checker);
#endif
#ifdef STRIPE_TEXTURE_LIT
	float3 N = normalize(i.tgsnor);
	float3 L = normalize(i.tgslit);
	base *= dot(N, L);
	return lerp(base, _StripeColor, 1 - checker) * _LightColor0;
#endif
#ifdef STRIPE_TEXTURE_LIT_BUMP
	float3 N = UnpackNormal(tex2D(_BumpTex, i.tex));
	float3 L = normalize(i.tgslit);
	base *= dot(N, L);
	return lerp(base, _StripeColor, 1 - checker) * _LightColor0;
#endif
}
void Hologram (v2f i)
{
	float2 scruv = i.scrpos.xy / i.scrpos.w;
	if (_HologramClip <= 500.0)
#ifdef STRIPE_HOLOGRAM_X
		clip (frac(scruv.x * _HologramClip) - 0.5);
#else
		clip (frac(scruv.y * _HologramClip) - 0.5);
#endif
}

#endif