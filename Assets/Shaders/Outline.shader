Shader "Custom/Outline" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_OutlineThickness ("Outline Thickness", Float) = 0
		_StencilRef ("Stencil Reference", Float) = 1
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}

	SubShader 
	{
		// Default Pass ...
		Name "Standard Surface Pass"
		Tags{ "RenderType" = "Opaque" }
		Stencil
		{
			Ref[_StencilRef]
			Comp Always
			Pass Replace
		}

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
		// End Default Pass

		// Outline Pass ...
		Pass
		{
			Name "Outline Pass"
			Tags { "RenderType"="Opaque" }
			Stencil
			{
				Ref [_StencilRef]
				Comp NotEqual
				Pass Replace
			}
		
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
			};

			float _OutlineThickness;

			v2f vert(appdata_base v)
			{
				v2f o;
			
				fixed3 outlineDir = v.normal * _OutlineThickness;
				o.vertex = UnityObjectToClipPos(v.vertex + outlineDir);

				return o;
			}

			fixed4 _OutlineColor;

			fixed4 frag(v2f i) : SV_Target
			{
				return _OutlineColor;
			}
			ENDCG
		}
		// End Outline Pass		
	}
	FallBack "Diffuse"
}