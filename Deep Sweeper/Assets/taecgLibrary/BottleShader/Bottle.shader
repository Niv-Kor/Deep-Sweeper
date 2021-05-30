Shader "taecg/UI/Bottle"
{
	Properties
	{
		[HideInInspector]_MainTex("MainTex", 2D) = "white" {}
		[Header(Base)]
		_GrowUp("Grow Up", Range( 0 , 1)) = 0
		_Back("Back (Height,Speed,Amplitude,Wave)", Vector) = (0.02,1,0.02,4)
		_Front("Front (Height,Speed,Amplitude,Wave)", Vector) = (0,-1,0.02,3)
		_TopColor("Top Color", Color) = (1,0.9,0.15,1)
		_BottomColor("Bottom Color", Color) = (0.83,0.45,0.07,1)
		_BackColor("Back Color", Color) = (0.63,0.3,0.02,1)
		_NoiseTex("NoiseTex", 2D) = "white" {}
		[Header(Noise)]_NoiseIntensity("Noise Intensity", Range( -3 , 3)) = 0.7
		[PowerSlider(3)]_NoiseDistortion("Noise Distortion", Range( 0 , 1)) = 0.1
		_Noise("Noise", Vector) = (1,1,-0.1,-0.1)
		[Header(Bubble)]_BubbleIntensity("Bubble Intensity", Range( -3 , 3)) = 0.7
		[PowerSlider(3)]_BubbleDistortion("Bubble Distortion", Range( 0 , 1)) = 0.07
		_Bubble("Bubble", Vector) = (3,3,-0.1,-0.2)
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZWrite Off
		ZTest LEqual
		
		Pass
		{
			Name "Liquid"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 uv : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform half4 _Back;
			uniform half _GrowUp;
			uniform half4 _Front;
			uniform float4 _BottomColor;
			uniform float4 _TopColor;
			uniform float4 _BackColor;
			uniform sampler2D _NoiseTex;
			uniform float4 _NoiseTex_ST;
			uniform float _NoiseDistortion;
			uniform float4 _Noise;
			uniform float _NoiseIntensity;
			uniform float _BubbleDistortion;
			uniform float4 _Bubble;
			uniform float _BubbleIntensity;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.uv.xy = v.uv.xy;
				o.uv.zw = 0;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);

				fixed4 finalColor;

				//Back（水面）
				float backSpeed = _Time.y * _Back.y;
				float backHeight = _Back.x + _GrowUp;
				float backShape =  backHeight > ( i.uv.y + ( sin( ( ( i.uv.x + backSpeed ) * _Back.w ) ) * _Back.z ) ) ? 1.0 : 0.0 ;

				//Front（水体）
				float frontSpeed = _Time.y * _Front.y;
				float frontHeight = _Front.x + _GrowUp;
				fixed4 bottomColor = fixed4(_BottomColor.rgb , 1.0);
				fixed4 topColor = fixed4(_TopColor.rgb , 1.0);
				fixed4 frontColor = lerp( bottomColor , topColor , ( i.uv.y / frontHeight ));
				fixed4 frontShape =  frontHeight > ( i.uv.y + ( sin( ( ( i.uv.xy.x + frontSpeed ) * _Front.w ) ) * _Front.z ) ) ? frontColor : 0.0 ;

				//Noise
				float2 uv_NoiseTex = i.uv.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				fixed4 noiseTex = tex2D(_NoiseTex,uv_NoiseTex);

				float2 noiseUV = lerp( i.uv.xy , noiseTex.bb, _NoiseDistortion);
				float noiseColor = tex2D( _NoiseTex, (noiseUV * _Noise.xy + ( _Time.y * _Noise.zw )) ).r * _NoiseIntensity;
				float shape = 1-clamp(0,1,i.uv.y / frontHeight);
				float2 bubbleUV = lerp( i.uv.xy , noiseTex.bb , _BubbleDistortion);
				float bubbleColor = tex2D( _NoiseTex, (bubbleUV * _Bubble.xy + ( _Time.y * _Bubble.zw )) ).g * _BubbleIntensity;
				float shapeClamp = clamp( ( backShape + frontShape.w ) , 0.0 , 1.0 );

				//MainTex
				float2 uv_MainTex = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				fixed4 mainTex = tex2D(_MainTex,uv_MainTex);

				fixed4 backFinalColor = ( backShape - frontShape.w ) * _BackColor ;
				fixed4 frontFinalColor = frontShape + ( noiseColor * shape ) + ( bubbleColor * shape ) ;
				
				finalColor = ( backFinalColor + frontFinalColor ) * ( shapeClamp * mainTex.a );
				return finalColor;
			}
			ENDCG
		}
	}	
}