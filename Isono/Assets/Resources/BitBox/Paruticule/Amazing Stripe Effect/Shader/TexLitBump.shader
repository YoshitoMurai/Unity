Shader "Amazing Stripe Effect/Tex Lit Bump" {
	Properties {
		_MainTex        ("Main", 2D) = "white" {}
		_BumpTex        ("Bump", 2D) = "bump" {}
		_StripeTex      ("Stripe", 2D) = "black" {}
		_StripeColor    ("Stripe Color", Color) = (1, 0.8, 0, 1)
		_StripeWidth    ("Stripe Width", Range(0.01, 0.8)) = 0.1
		_StripeDensity  ("Stripe Density", Range(0.1, 20)) = 5
		_StripeMove     ("Stripe Move", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile STRIPE_X STRIPE_Y STRIPE_Z STRIPE_XY STRIPE_XZ STRIPE_YZ STRIPE_XYZ
			#define STRIPE_TEXTURE_LIT_BUMP
			#include "Utils.cginc"
			ENDCG
		}
	}
	FallBack "Diffuse"
}
