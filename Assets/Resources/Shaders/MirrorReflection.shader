Shader "Venat/MirrorReflection"
{
	Properties
	{
		_Color ("Color Tint", Color) = (1,1,1,1)
		_ReflectionPortion("Reflection degree", Range(0,1)) = 0
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ReflectionTex ("", 2D) = "white" {}
	}

	SubShader
	{ 
		Pass 
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _MainTex_ST;

			sampler2D_half _MainTex;
			sampler2D_half _ReflectionTex;
			fixed4 _Color;
			fixed _ReflectionPortion;

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				fixed4 refl : TEXCOORD1;
				fixed4 pos : SV_POSITION;
			};			

			v2f vert(fixed4 pos : POSITION, fixed2 uv : TEXCOORD0)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (pos);
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				o.refl = ComputeScreenPos (o.pos);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 tex = tex2D(_MainTex, i.uv) * _Color;
				
				fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(i.refl)) * _Color.a;

				return lerp(tex, refl, _ReflectionPortion);
			}

			ENDCG
	    }
	}
}