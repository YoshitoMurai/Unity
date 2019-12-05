Shader "Amazing Stripe Effect/Hologram" {
	Properties {
		_MainTex         ("Main", 2D) = "white" {}
		_HologramClip    ("Hologram Clip", Range(0, 501)) = 100
		_RimColor        ("Rim Color", Color) = (0, 0.5, 1, 1)
		_RimPower        ("Rim Power", Range(0.1, 8)) = 3
		_RimBrightness   ("Rim Brightness", Range(0, 3)) = 1.5
		_BaseColorAmount ("Base Color Amount", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragHologram
			#pragma multi_compile _ STRIPE_HOLOGRAM_X
			#include "Utils.cginc"
			uniform float4 _RimColor;
			uniform float  _RimPower;
			uniform float  _RimBrightness;
			uniform float  _BaseColorAmount;
			float4 fragHologram (v2f i) : SV_TARGET
			{
				Hologram (i);
				
				float3 N = normalize(i.tgsnor);
				float3 V = normalize(i.tgsview);
				float rim = 1 - saturate(dot(V, N));
				rim = pow(rim, _RimPower)* _RimBrightness;
				
				float3 base = tex2D(_MainTex, i.tex).rgb;
				float3 color = lerp(_RimColor.rgb * rim , base, _BaseColorAmount);
				
				float4 o;
				o.rgb = color;
				o.a = rim;
				return o;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
