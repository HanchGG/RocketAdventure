Shader "Unlit/LMGUICircle"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CircleTex("CircleTexture", 2D) = "white" {}
		_Color("color", Color) = (1,1,1,1)
		_ClipValue("clipValue", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+101" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always
		Pass
		{
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _CircleTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _ClipValue;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 colCircle = tex2D(_CircleTex, i.uv);
				clip(colCircle.a - _ClipValue);

				fixed4 col = tex2D(_MainTex, i.uv);
				return col * _Color;
			}
			ENDCG
		}
	}
}
