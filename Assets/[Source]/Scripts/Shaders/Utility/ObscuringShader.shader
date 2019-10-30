Shader "Custom/ObscuringShader"
{
	SubShader{
		// draw after all opaque objects (queue = 3001):
		Tags { "Queue" = "Transparent+1" }
		Pass {
		  Blend Zero One // keep the image behind it
		}
	}
}
