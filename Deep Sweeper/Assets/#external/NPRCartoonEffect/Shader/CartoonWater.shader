Shader "NPR Cartoon Effect/Water" {
	Properties {
		[Header(Color)]
		_ShallowColor         ("Shallow", Color) = (0.325, 0.807, 0.971, 0.725)
		_DeepColor            ("Deep", Color) = (0.086, 0.407, 1, 0.749)
		_DepthDistance        ("Depth Distance", Float) = 0.1
		[Header(Foam)]
		_FoamColor            ("Color", Color) = (1, 1, 1, 1)
		_FoamDensity          ("Density", Float) = 0.03
		_FoamNoise            ("Noise", 2D) = "white" {}
		_FoamScroll           ("Scroll Speed (xy)", Vector) = (0.03, 0.03, 0, 0)
		_FoamCutoff           ("Cutoff", Range(0, 1)) = 0.7
		_FoamDistortion       ("Distortion", 2D) = "white" {}
		_FoamDistortionAmount ("Distortion Amount", Range(0, 1)) = 0.2
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			#define SMOOTHSTEP_AA 0.01
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 blend (float4 top, float4 bottom)
			{
				float3 c = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
				float a = top.a + bottom.a * (1 - top.a);
				return float4(c, a);
			}

			sampler2D _CameraDepthTexture;
			sampler2D _FoamNoise;      float4 _FoamNoise_ST;
			sampler2D _FoamDistortion; float4 _FoamDistortion_ST;
			float4 _ShallowColor, _DeepColor, _FoamColor;
			float _DepthDistance, _FoamCutoff, _FoamDistortionAmount, _FoamDensity;
			float2 _FoamScroll;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 scrpos : TEXCOORD1;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrpos = ComputeScreenPos(o.pos);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _FoamNoise);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _FoamDistortion);
				return o;
			}
			float4 frag (v2f i) : SV_Target
			{
				float d = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrpos)).r);
				float diff = d - i.scrpos.w;
				float4 waterColor = lerp(_ShallowColor, _DeepColor, saturate(diff / _DepthDistance));

				float cutoff = saturate(diff / _FoamDensity) * _FoamCutoff;

				float2 distort = (tex2D(_FoamDistortion, i.uv.zw).xy * 2 - 1) * _FoamDistortionAmount;
				float2 uv = float2(
					(i.uv.x + _Time.y * _FoamScroll.x) + distort.x,
					(i.uv.y + _Time.y * _FoamScroll.y) + distort.y);
				float nis = smoothstep(cutoff - SMOOTHSTEP_AA, cutoff + SMOOTHSTEP_AA, tex2D(_FoamNoise, uv).r);

				float4 foamColor = _FoamColor;
				foamColor.a *= nis;
				return blend(foamColor, waterColor);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
