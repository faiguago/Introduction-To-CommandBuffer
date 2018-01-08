Shader "FI/ShowPrePass"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Common.cginc"

			sampler2D _PrePassTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_PrePassTex, i.uv);
				return col;
			}

			ENDCG
		}
	}
}
