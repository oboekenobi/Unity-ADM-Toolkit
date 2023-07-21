// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/dropShadow"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

		_distance ("_distance", Range(0,1)) = 0.0005
		_color ("_color", Color) = (1,0,0,1.5)

		_offsetX ("_offsetX", Float) = 0
		_offsetY ("_offsetY", Float) = 0

		[MaterialToggle] _longShadow ("_longShadow", Float) = 1
		_longShadowAngle ("_longShadowAngle", Range(0,360)) = 0.0005

	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"PreviewType" = "Plane"
			"Disabledisttching" = "True"
		}

		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile d3d9 d3d11 glcore gles gles3 metal xboxone ps4 psp2 n3ds wiiu

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				half4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1) * 0.5;
				o.color = v.color;
				return o;
			}

			float2 safemul(float4x4 M, float4 v)
			{
				float2 r;

				r.x = dot(M._m00_m01_m02, v);
				r.y = dot(M._m10_m11_m12, v);

				return r;
			}

			float4 _color;

			uniform sampler2D _GlobalShadowTex;

			uniform float _distance;

			uniform float _offsetX;
			uniform float _offsetY;

			uniform float _longShadow;
			uniform float _longShadowAngle;


			float4 frag(v2f i) : SV_Target
			{
				
				float4 result = float4(1,1,1,0);

				float dist = _distance * 0.01;


				float coef=1.0;
			    float fI=0;

			    #if UNITY_UV_STARTS_AT_TOP
            		i.screenuv.y = 1-i.screenuv.y;
            	#endif

				if (	
				 	_longShadow
					== 0
					) 
				{
					for (int j = 0; j < 5; j++) 
				    {
				    	fI++;
	////			    	coef*=0.32;
	////			    	coef*=0.16;
	////			    	coef*=0.08;
	////					coef*=0.64;
						coef*=0.96;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * 1), i.screenuv.y - _offsetY + (fI * dist * 0))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * 0.70710654317256), i.screenuv.y - _offsetY + (fI * dist * 0.707107019200454))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * -6.73205103601558E-07), i.screenuv.y - _offsetY + (fI * dist * 0.999999999999773))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * -0.707107495228028), i.screenuv.y - _offsetY + (fI * dist * 0.707106067144346))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * -0.999999999999094), i.screenuv.y - _offsetY + (fI * dist * -1.34641020720281E-06))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * -0.707105591115812), i.screenuv.y - _offsetY + (fI * dist * -0.707107971255281))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * 2.01961531035936E-06), i.screenuv.y - _offsetY + (fI * dist * -0.999999999997961))) * coef;
						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * 0.707108447282213), i.screenuv.y - _offsetY + (fI * dist * -0.707105115086956))) * coef;
				    }
				}
				else if (_longShadow == 1)
				{

					float x = cos(_longShadowAngle * 0.0174533);
					float y = sin(_longShadowAngle * 0.0174533);

					for (int j = 0; j < 25; j++) 
				    {
				    	fI++;
	////			    	coef*=0.32;
	////			    	coef*=0.16;
	////			    	coef*=0.08;
	////					coef*=0.64;
						coef*=0.96;


						result += tex2D(_GlobalShadowTex, float2(i.screenuv.x - _offsetX + (fI * dist * x), i.screenuv.y - _offsetY + (fI * dist * y))) * coef * 8;
				    }
				}




			    result *= _color;

			    result.a *= 0.02;

				return result;
			}
			ENDCG
		}
	}

}