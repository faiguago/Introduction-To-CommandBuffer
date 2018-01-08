Shader "FI/GaussianBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE 

	static const int maxOffset = 3;

	static const float kernel[7] = {
		0.071303,
		0.131514,
		0.189879,
		0.214607,
		0.189879,
		0.131514,
		0.071303 };

	ENDCG

	SubShader
	{
		// Horizontal Pass
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Common.cginc"


			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = fixed4(0, 0, 0, 0);
				for (int j = -maxOffset; j <= maxOffset; j++) {
					col += tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x * j, 0)) * kernel[j + maxOffset];
				}
				return col;
			}

			ENDCG
		}

		// Vertical Pass
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Common.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = fixed4(0, 0, 0, 0);
				for (int j = -maxOffset; j <= maxOffset; j++) {
					col += tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y * j)) * kernel[j + maxOffset];
				}
				return col;
			}

			ENDCG
		}
	}
}
