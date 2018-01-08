Shader "FI/FinalPass"
{
	SubShader
	{

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Common.cginc"

			float _Intensity;
			fixed4 _GlowColor;

			sampler2D _PrePassTex,
				_BlurredTex;

			fixed4 frag(v2f i) : SV_Target
			{
				return _GlowColor * _Intensity * (tex2D(_BlurredTex, i.uv) - tex2D(_PrePassTex, i.uv));
			}

			ENDCG
		}

	}
}
