// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nettle/LeapMotion/GUI_Blur" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_BackfaceColor("Backface Color", Color) = (1,1,1,1)
		_MainTex("MainTexture", 2D) = "white" {}
		_Size("Size", Range(0, 5)) = 1
		_Transparency("Inner transparency", Range(0, 1)) = 0.5
		_Lightness("Lightness",Float) = 1.5
		[MaterialToggle(REFLECTION)] _Reflection("Use reflection", Float) = 1
		_ReflectionMap("Reflection map", Cube) = "black" {}
		_ReflectionStrength("Reflection strength", Range(0, 1)) = 0.5
	}

		Category{
		Tags{ "Queue" = "Transparent+100" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		ZWrite Off
		ZTest Always
		Cull Off

		SubShader{

		GrabPass{
		"_Grab"
		Tags{ "LightMode" = "Always" }
	}



		Pass{
		Tags{ "LightMode" = "Always" }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma shader_feature REFLECTION
#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord: TEXCOORD0;
#if REFLECTION
		float3 normal:NORMAL;
#endif
	};

	struct v2f {
		float4 vertex : POSITION;
		float4 uvgrab : TEXCOORD0;
#if REFLECTION
		float3 normal:TEXCOORD1;
		float3 worldPos:TEXCOORD2;
#endif
	};

	fixed4 _Color;

	v2f vert(appdata_t v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
#else
		float scale = 1.0;
#endif
		o.uvgrab = ComputeGrabScreenPos(o.vertex);
#if REFLECTION
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
#endif
		return o;
	}

	sampler2D _Grab;
#if REFLECTION
	samplerCUBE _ReflectionMap;
	float _ReflectionStrength;
#endif
	float4 _Grab_TexelSize;
	float _Size;
	float _Lightness;
	float _Transparency;

	half4 frag(v2f i) : COLOR{

		half4 sum = half4(0,0,0,0);
		half localSize = _Color.a;
#define GRABPIXEL_X(weight,kernelx) tex2Dproj( _Grab, UNITY_PROJ_COORD(float4(i.uvgrab.x + _Grab_TexelSize.x * kernelx*_Size* i.uvgrab.w*localSize, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
#define GRABPIXEL_Y(weight,kernely) tex2Dproj( _Grab, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _Grab_TexelSize.y * kernely*_Size * i.uvgrab.w*localSize, i.uvgrab.z, i.uvgrab.w))) * weight
		

		sum += GRABPIXEL_X(0.05, -4.0);
		sum += GRABPIXEL_X(0.09, -3.0);
		sum += GRABPIXEL_X(0.12, -2.0);
		sum += GRABPIXEL_X(0.15, -1.0);
		sum += GRABPIXEL_X(0.18, 0.0);
		sum += GRABPIXEL_X(0.15, +1.0);
		sum += GRABPIXEL_X(0.12, +2.0);
		sum += GRABPIXEL_X(0.09, +3.0);
		sum += GRABPIXEL_X(0.05, +4.0);

		sum += GRABPIXEL_Y(0.05, -4.0);
		sum += GRABPIXEL_Y(0.09, -3.0);
		sum += GRABPIXEL_Y(0.12, -2.0);
		sum += GRABPIXEL_Y(0.15, -1.0);
		sum += GRABPIXEL_Y(0.18,  0.0);
		sum += GRABPIXEL_Y(0.15, +1.0);
		sum += GRABPIXEL_Y(0.12, +2.0);
		sum += GRABPIXEL_Y(0.09, +3.0);
		sum += GRABPIXEL_Y(0.05, +4.0);

		sum /= 2;
		sum = saturate(sum);
		float4 result = sum *lerp(float4(1, 1, 1, 1), _Color * _Lightness, _Transparency * _Color.a);
#if REFLECTION
		float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
		result += texCUBE(_ReflectionMap, reflect(viewDirection, normalize(i.normal))) * _ReflectionStrength *_Color.a;
#endif
		return  result;
	}
		ENDCG
}

//Backfaces pass
Blend SrcAlpha OneMinusSrcAlpha
Pass
	{
		Cull Front
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	float4 _Color;
	float4 _BackfaceColor;

	sampler2D _MainTex;
	float4 _MainTex_ST;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return  tex2D(_MainTex, i.uv)* _Color*_BackfaceColor;
	}
		ENDCG
	}

//Front faces pass
Blend SrcAlpha OneMinusSrcAlpha
Pass
	{
	Cull Back
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

				struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) * _Color;
			}
			ENDCG
		}

		}
	}
}