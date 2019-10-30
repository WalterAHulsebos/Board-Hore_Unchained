Shader "Custom/DynamicGradient" {
	Properties{
		 _Color("Color", Color) = (1,1,1,1)
		 _MainTex("Albedo (RGB)", 2D) = "white" {}
		 _GrayScaleTex("GrayScale (RGB)", 2D) = "white" {}
		 [PerRendererData]_Cutoff("Cutoff", Range(0,1)) = 0.5
	}
		SubShader{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert alpha:fade

			sampler2D _MainTex;
			sampler2D _GrayScaleTex;
			fixed4 _Color;
			float _Cutoff;

			struct Input {
				float2 uv_MainTex;
				float2 uv_GrayScaleTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;

				fixed4 a = tex2D(_GrayScaleTex, IN.uv_GrayScaleTex) * _Color;

				clip(a.rgb - _Cutoff);
			}
			ENDCG
		}
			FallBack "Diffuse"
}