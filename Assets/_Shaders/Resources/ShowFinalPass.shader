Shader "FI/ShowFinalPass"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Common.cginc"

			sampler2D _FinalTex;

			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_FinalTex, i.uv);
			}

			ENDCG
		}
	}
}
