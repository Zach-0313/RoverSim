Shader "Custom/Shader1"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DisolveTexture("Disolve Texture", 2D) = "white"
		_DisolveY("Current Y of the disolve effect", Float) = 0
		_DisolveSize("Size of the effect", Float) = 2
		_StartingY("Starting point of the effect", Float) = -10
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

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
					float4 vertex : SV_POSITION;
					float3 WorldPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _DisolveTexture;
				float _DisolveY;
				float _DisolveSize;
				float _StartingY;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.WorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{

					float transition = _DisolveY - i.WorldPos.y;
					clip(_StartingY + (transition + (tex2D(_DisolveTexture, i.uv)) * _DisolveSize));

					// sample the texture
					fixed4 col = tex2D(_MainTex, i.uv);

					return col;
				}
				ENDCG
			}
		}
}
