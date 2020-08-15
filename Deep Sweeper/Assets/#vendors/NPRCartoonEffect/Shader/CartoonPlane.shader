Shader "NPR Cartoon Effect/Cartoon Plane" {
	Properties {
		_MainTex      ("Base", 2D) = "white" {}
		_Color        ("Color", Color) = (1, 1, 1, 1)
		_ShadowColor  ("Shadow", Color) = (0.4, 0.4, 0.4, 1.0)
		_ShadowSmooth ("Shadow Smooth", Float) = 0.1
	}
	SubShader {
		Tags{ "LightMode" = "ForwardBase" }
		Pass{
			CGPROGRAM
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST, _Color, _ShadowColor;
			float _ShadowSmooth;
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
				float3 tgsnor : TEXCOORD1;    // tangent space normal
				float3 tgslit : TEXCOORD2;    // tangent space light
				SHADOW_COORDS(3)
				UNITY_FOG_COORDS(4)
			};
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				TANGENT_SPACE_ROTATION;
				o.tgsnor = mul(rotation, v.normal);
				o.tgslit = mul(rotation, ObjSpaceLightDir(v.vertex));
				TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
			float4 frag (v2f IN) : SV_Target
			{
				float3 N = normalize(IN.tgsnor);
				float3 L = normalize(IN.tgslit);
				float ndl = saturate(dot(N, L));
				float4 albedo = tex2D(_MainTex, IN.tex);
				float atten = LIGHT_ATTENUATION(IN);
				atten = smoothstep(0.5 - _ShadowSmooth * 0.5, 0.5 + _ShadowSmooth * 0.5, atten);
				float4 c = lerp(_ShadowColor, _Color * albedo, atten);
				UNITY_APPLY_FOG(IN.fogCoord, c);
				return c;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
