Shader "Unlit/OutLine"
{
	Properties
	{
		_Diffuse("_Diffuse",color) = (1,1,1,1)
		_OutLineCol("OutLineCol",color) = (1,0,0,1)
		_OutLineFactor("OutLineFactor",Range(0,1)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull off
			Stencil
			{
				Ref 0
				Comp GEqual
				Pass Zero
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _OutLineCol;
			float _OutLineFactor;


			struct v2f
			{
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				v.vertex.xyz += v.normal * _OutLineFactor;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _OutLineCol;
				return col;
			}
			ENDCG
		}
	}
}
