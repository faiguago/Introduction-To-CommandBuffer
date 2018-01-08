Shader "FI/ShowBlurred"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Common.cginc"

			sampler2D _BlurredTex;

			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_BlurredTex, i.uv);
			}

			ENDCG
		}
	}
}
