Shader "NPR Cartoon Effect/Cartoon No Outline" {
	Properties {
		[Header(Basic)]
		_MainTex           ("Base", 2D) = "white" {}
		_BumpTex           ("Bump", 2D) = "bump" {}
		_HighlitColor      ("Highlit", Color) = (0.6, 0.6, 0.6, 1.0)
		_DarkColor         ("Dark", Color) = (0.4, 0.4, 0.4, 1.0)
		_StylizedShadowTex ("Stylized Shadow", 2D) = "black" {}
		_Saturation        ("Saturation", Range(0, 2)) = 1
		_DarkSideRange     ("Dark Side Range", Range(-1, 1)) = 0
		[Space(10)][Header(Ramp Shading)]
		_RampTex       ("Ramp", 2D) = "white" {}
		_RampThreshold ("Ramp Threshold", Float) = 0.5
		_RampSmooth    ("Ramp Smoothing", Float) = 0.1
		[Space(10)][Header(Specular)]
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
		_SpecPower     ("Specular Power", Float) = 128
		_SpecSmooth    ("Specular Smooth", Float) = 0.1
		[Space(10)][Header(Stylized Specular)]
		_SpecularScale        ("Specular Scale", Range(0, 0.05)) = 0.01
		_SpecularTranslationX ("Specular Translation X", Range(-1, 1)) = 0
		_SpecularTranslationY ("Specular Translation Y", Range(-1, 1)) = 0
		_SpecularRotationX    ("Specular Rotation X", Range(-180, 180)) = 0
		_SpecularRotationY    ("Specular Rotation Y", Range(-180, 180)) = 0
		_SpecularRotationZ    ("Specular Rotation Z", Range(-180, 180)) = 0
		_SpecularScaleX       ("Specular Scale X", Range(-1, 1)) = 0
		_SpecularScaleY       ("Specular Scale Y", Range(-1, 1)) = 0
		_SpecularSplitX       ("Specular Split X", Range(0, 1)) = 0
		_SpecularSplitY       ("Specular Split Y", Range(0, 1)) = 0
		[Space(10)][Header(Rim)]
		_RimColor ("Rim Color", Color) = (0.8, 0.8, 0.8, 0.6)
		_RimMin   ("Rim Min", Float) = 0.5
		_RimMax   ("Rim Max", Float) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		Pass {
			CGPROGRAM
			#include "Cartoon.cginc"
			#pragma multi_compile_fwdbase
			#pragma multi_compile _ NCE_BUMP
			#pragma multi_compile _ NCE_RAMP_TEXTURE
			#pragma multi_compile _ NCE_SPECULAR
			#pragma multi_compile _ NCE_STYLIZED_SPECULAR
			#pragma multi_compile _ NCE_STYLIZED_SHADOW
			#pragma vertex vert
			#pragma fragment frag

			v2f vert (appdata_tan v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.tex.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.tex.zw = TRANSFORM_TEX(v.texcoord, _StylizedShadowTex);
				TANGENT_SPACE_ROTATION;
				o.tgsnor = mul(rotation, v.normal);
				o.tgslit = mul(rotation, ObjSpaceLightDir(v.vertex));
				o.tgsview = mul(rotation, ObjSpaceViewDir(v.vertex));
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			float4 frag (v2f i) : SV_TARGET
			{
				return fragCartoon(i);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
	CustomEditor "CartoonShaderUI"
}
