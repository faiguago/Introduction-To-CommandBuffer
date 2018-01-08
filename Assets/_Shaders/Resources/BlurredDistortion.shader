Shader "FI/BlurredDistortion"
{
	Properties
	{
		_Distortion("Distortion", Range(0, 16)) = 8
		_NormalMap("NormalMap", 2D) = "bump" {}
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 uvgrab : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _NormalMap, _BlurredGrabTex;
			float4 _NormalMap_ST, _BlurredGrabTex_TexelSize;
			float _Distortion;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _NormalMap);
				o.uvgrab = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 bump = UnpackNormal(tex2D(_NormalMap, i.uv)).rg;
				float2 offset = _Distortion * bump * _BlurredGrabTex_TexelSize.xy;

				#if defined(UNITY_Z_0_FAR_FROM_CLIPSPACE)
					i.uvgrab.xy = offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(i.uvgrab.z) + i.uvgrab.xy;
				#else
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
				#endif

				fixed4 col = tex2Dproj(_BlurredGrabTex, UNITY_PROJ_COORD(i.uvgrab));
				return col;
			}

			ENDCG
		}
	}
}
