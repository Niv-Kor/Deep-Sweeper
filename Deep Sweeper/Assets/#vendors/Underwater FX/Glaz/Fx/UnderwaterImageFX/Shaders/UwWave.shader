Shader "GlazUnderwaterFX/UwWave" {
    Properties {
	  _MainTex ("Base (RGB)", 2D) = "" {}
	  _WaveSize("_WaveSize", float) = 5.0
	  _Amplitude("_Amplitude", float) = 0.02	 
	  _Phase("_TimePhase", float) = 0.0
    }
    
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	half4 _MainTex_ST;
	
	float _WaveSize;
	float _Amplitude;
	float _TimePhase;

	//float2 intensity;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : SV_Target 
	{
		float2 uv = i.uv;
		float X = uv.x * _WaveSize + _TimePhase;
  		float Y = uv.y * _WaveSize + _TimePhase;
		float Xoffset = cos(X-Y)*_Amplitude*cos(Y);
  		float Yoffset = sin(X+Y)*_Amplitude*sin(Y);
		uv.x += Xoffset;
		uv.y += Yoffset;
		
		 
		 half4 color = tex2D (_MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));
		
		return color;

	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
  
}

Fallback off
	
} // shader

