Shader "Hidden/NGSS_DepthCombine"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE

		#pragma vertex vert
		#pragma fragment frag
		#pragma exclude_renderers gles d3d9
		#pragma target 3.0

		#include "UnityCG.cginc"
		half4 _MainTex_ST;
		/*
#if !defined(UNITY_SINGLE_PASS_STEREO)
#define UnityStereoTransformScreenSpaceTex(uv) (uv)
#endif*/
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			//float2 uv2 : TEXCOORD0;
		};

		v2f vert (appdata v)
		{
			v2f o = (v2f)0;

			o.vertex = UnityObjectToClipPos(v.vertex);
			//o.uv = v.uv;//NGSS 2.0
			o.uv = ComputeNonStereoScreenPos(o.vertex).xy;//NGSS 2.0
			
			//o.uv = UnityStereoTransformScreenSpaceTex(v.uv);
			
			#if UNITY_UV_STARTS_AT_TOP
			//o.uv2 = UnityStereoTransformScreenSpaceTex(v.uv);
			if (_MainTex_ST.y < 0.0)
				o.uv.y = 1.0 - o.uv.y;
			#endif
			return o;
		}

		ENDCG
		
		Pass // combine depths
		{
			CGPROGRAM
			
			//sampler2D _CameraDepthTexture;
			//half4 _CameraDepthTexture_ST;
			//sampler2D NGSS_StaticDepthMap;
			//sampler2D _MainTex;//dynamic one
			UNITY_DECLARE_SHADOWMAP(NGSS_StaticDepthMap);
			UNITY_DECLARE_SHADOWMAP(_MainTex);
			SamplerState my_point_clamp;
			
			fixed4 frag (v2f input) : SV_Target
			{
				//float depthS = tex2D(NGSS_StaticDepthMap, UnityStereoTransformScreenSpaceTex(input.uv)).r;
				//float depthD = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(input.uv)).r;
				//float depthS = tex2D(NGSS_StaticDepthMap, input.uv).r;
				//float depthD = tex2D(_MainTex, input.uv).r;
				float depthS = NGSS_StaticDepthMap.SampleLevel(my_point_clamp, input.uv, 0.0);
				float depthD = _MainTex.SampleLevel(my_point_clamp, input.uv, 0.0);
				
				#if defined(UNITY_REVERSED_Z)
					return depthS > depthD ? depthS : depthD;
				#else
					return depthS < depthD ? depthS : depthD;
				#endif
			}
			ENDCG
		}
	}
	Fallback Off
}
