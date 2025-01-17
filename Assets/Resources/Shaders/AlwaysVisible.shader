﻿Shader "Unlit/AlwaysVisible"
{
    Properties
    {
		[PerRendererData]_Color("Always visible color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100


		Pass
		{
			Cull Off
			ZWrite Off
			ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			float4 _Color;

			fixed4 frag(v2f i) : SV_Target
			{
				return _Color;
			}


			ENDCG
		}
    }
}
