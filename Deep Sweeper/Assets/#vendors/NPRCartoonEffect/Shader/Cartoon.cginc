#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
sampler2D _RampTex;
float _RampThreshold, _RampSmooth;

fixed calcRamp (float ndl)
{
#if NCE_RAMP_TEXTURE
	fixed ramp = tex2D(_RampTex, float2(ndl, 0.5)).r;
#else
	fixed ramp = smoothstep(_RampThreshold - _RampSmooth * 0.5, _RampThreshold + _RampSmooth * 0.5, ndl);
#endif
	return ramp;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
float4 _SpecularColor;
float _SpecularScale;
float _SpecularTranslationX, _SpecularTranslationY;
float _SpecularRotationX, _SpecularRotationY, _SpecularRotationZ;
float _SpecularScaleX, _SpecularScaleY;
float _SpecularSplitX, _SpecularSplitY;
float _SpecPower, _SpecSmooth;

fixed3 calcSpecular (float3 N, float3 H)
{
#if NCE_STYLIZED_SPECULAR
	// specular highlights scale
	H = H - _SpecularScaleX * H.x * float3(1, 0, 0);
	H = normalize(H);
	H = H - _SpecularScaleY * H.y * float3(0, 1, 0);
	H = normalize(H);

	// specular highlights rotation
	#define DegreeToRadian 0.0174533
	float radX = _SpecularRotationX * DegreeToRadian;
	float3x3 rotMatX = float3x3(
		1,	0, 		 	0,
		0,	cos(radX),	sin(radX),
		0,	-sin(radX),	cos(radX));
	float radY = _SpecularRotationY * DegreeToRadian;
	float3x3 rotMatY = float3x3(
		cos(radY), 	0, 		-sin(radY),
		0,			1,		0,
		sin(radY), 	0, 		cos(radY));
	float radZ = _SpecularRotationZ * DegreeToRadian;
	float3x3 rotMatZ = float3x3(
		cos(radZ), 	sin(radZ), 	0,
		-sin(radZ), cos(radZ), 	0,
		0, 			0,			1);
	H = mul(rotMatZ, mul(rotMatY, mul(rotMatX, H)));
	H = normalize(H);

	// specular highlights translation
	H = H + float3(_SpecularTranslationX, _SpecularTranslationY, 0);
	H = normalize(H);

	// specular highlights split
	float signX = 1;
	if (H.x < 0)
		signX = -1;

	float signY = 1;
	if (H.y < 0)
		signY = -1;

	H = H - _SpecularSplitX * signX * float3(1, 0, 0) - _SpecularSplitY * signY * float3(0, 1, 0);
	H = normalize(H);
				
	// stylized specular light
	float spec = dot(N, H);
	float w = fwidth(spec);
	return lerp(float3(0, 0, 0), _SpecularColor.rgb, smoothstep(-w, w, spec + _SpecularScale - 1.0));
#else
	float ndh = saturate(dot(N, H));
	float spec = pow(ndh, _SpecPower);
	spec = smoothstep(0.5 - _SpecSmooth * 0.5, 0.5 + _SpecSmooth * 0.5, spec);
	return _SpecularColor * spec;
#endif
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
struct v2f
{
	float4 pos : SV_POSITION;
	float4 tex : TEXCOORD0;
	float3 tgsnor : TEXCOORD1;    // tangent space normal
	float3 tgslit : TEXCOORD2;    // tangent space light
	float3 tgsview : TEXCOORD3;   // tangent space view
	LIGHTING_COORDS(4, 5)
};

sampler2D _MainTex, _StylizedShadowTex, _BumpTex;
float4 _MainTex_ST, _StylizedShadowTex_ST;
fixed4 _HighlitColor, _DarkColor, _RimColor;
fixed _RimMin, _RimMax, _Saturation, _DarkSideRange;

float4 fragCartoon (v2f i)
{
#if NCE_BUMP
	float3 N = UnpackNormal(tex2D(_BumpTex, i.tex.xy));
#else
	float3 N = normalize(i.tgsnor);
#endif
	float3 L = normalize(i.tgslit);
	float3 V = normalize(i.tgsview);
	float3 H = normalize(V + L);

	//
	// cartoon light model
	//
				
	// ambient light from Unity render setting
	float3 ambientColor = UNITY_LIGHTMODEL_AMBIENT.xyz;

	// rim light
	half rim = 1.0 - saturate(dot(V, N));
	rim = smoothstep(_RimMin, _RimMax, rim) * _RimColor.a;
				
	fixed4 albedo = tex2D(_MainTex, i.tex.xy);
	albedo = lerp(albedo, _RimColor, rim);
				
	// diffuse cartoon light
	float ndl = saturate(dot(N, L) - _DarkSideRange);
	float diff = ndl * LIGHT_ATTENUATION(i);

	fixed4 darkColor = _DarkColor;
#if NCE_STYLIZED_SHADOW
	darkColor = tex2D(_StylizedShadowTex, i.tex.zw) * _DarkColor;
#endif
	fixed ramp = calcRamp(diff);

	fixed4 c = lerp(_HighlitColor, darkColor, _DarkColor.a);
	fixed4 rampColor = lerp(c, _HighlitColor, ramp);				
	float4 diffuseColor = albedo * rampColor;

#if NCE_SPECULAR
	fixed3 specularColor = calcSpecular(N, H);
#else
	fixed3 specularColor = 0.0;
#endif
	float3 lit = ambientColor + (diffuseColor.rgb + specularColor) * _LightColor0;
	float3 gray = Luminance(lit).xxx;
	float3 rc = lerp(gray, lit, _Saturation);
	return float4(rc, 1.0);
}