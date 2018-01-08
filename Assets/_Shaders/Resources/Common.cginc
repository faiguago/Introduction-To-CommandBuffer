#if !defined(COMMON_INCLUDED)
#pragma exclude_renderers d3d11 gles
#define COMMON_INCLUDED

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_TexelSize;

struct v2f {
	float4 pos: SV_POSITION;
	float2 uv: TEXCOORD0;
};

v2f vert(appdata_base v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
	return o;
}

#endif