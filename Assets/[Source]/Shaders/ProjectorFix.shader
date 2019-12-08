Shader "FX/Projector Blend"
{
	Properties
	{
		_Color("MainColor", Color) = (1,1,1,1)
		_ShadowTex("Cookie", 2D) = "gray"    { TexGen ObjectLinear }
		_FalloffTex("FallOff", 2D) = "white"   { TexGen ObjectLinear }
	}

		Subshader
	{
		Pass
		{
			ZWrite Off
			Offset - 3, -3
			AlphaTest Greater 0
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

			SetTexture[_ShadowTex]
			{
				constantColor[_Color]
				combine texture * constant
				Matrix[_Projector]
			}
			SetTexture[_FalloffTex]
			{
				constantColor(0,0,0,0)
				combine previous lerp(texture alpha) constant
				Matrix[_ProjectorClip]
			}
		}
	}
}