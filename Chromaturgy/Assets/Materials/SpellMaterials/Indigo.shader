Shader "Custom/Indigo"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Bumpiness("Bumpiness", float) = 2
		_NormalComparison("Normal Comparison", float) = 0.1
		_Color("Color", COLOR) = (1, 1, 1, 1)
		_Lerp("Lerp", Range(0, 1)) = 1
	}
		SubShader
		{
			Pass
			{

				Tags {"LightMode" = "ForwardBase"}
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
					float3 tangent : TANGENT;
				};

				struct v2f
				{
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
					float4 worldPosition : TEXCOORD2;
					float3 normal : NORMAL;
					float3 tangent : TANGENT;
					float4 pos : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.normal = UnityObjectToWorldNormal(v.normal);
					o.uv = v.uv;
					o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
					o.tangent = UnityObjectToWorldNormal(v.tangent);
					return o;
				}

				float _Bumpiness;
				float _NormalComparison;
				float4 _Color;
				float _Lerp;

				float4 frag(v2f i) : SV_Target
				{
					i.uv.y *= 2;
					i.uv -= _Time.x;

					// calculate bumped normal using tangent space lol
					float3 bitangent = cross(i.normal, i.tangent);

					// failed heightmap lol idc
					float3 x1 = float3(_NormalComparison, tex2D(_MainTex, i.uv + float2(_NormalComparison, 0)).x * _Bumpiness, 0);
					float3 x2 = float3(-_NormalComparison, tex2D(_MainTex, i.uv + float2(-_NormalComparison, 0)).x * _Bumpiness, 0);
					float3 y1 = float3(0, tex2D(_MainTex, i.uv + float2(0, _NormalComparison)).x * _Bumpiness, _NormalComparison);
					float3 y2 = float3(0, tex2D(_MainTex, i.uv + float2(0, -_NormalComparison)).x * _Bumpiness, -_NormalComparison);

					float3 normalMap = normalize(cross(x1 - x2, y1 - y2));

					float3x3 tbn = float3x3(i.tangent, bitangent, i.normal);
					tbn = transpose(tbn);

					float3 bumpedNormal = mul(tbn, normalMap);

					float nl = max(0.5, dot(bumpedNormal, _WorldSpaceLightPos0.xyz));
					
					float3 viewVector = normalize(i.worldPosition - _WorldSpaceCameraPos);
					float fresnel = pow(1 - dot(i.normal, -viewVector), 2);

					float4 col = _Color;
					col.rgb *= min(nl + fresnel, 1);

					if ((dot(bumpedNormal, _WorldSpaceLightPos0.xyz) + 1) / 2 < 1 - _Lerp)
						col.w = 0;

					return col;
				}
				ENDCG
			}
		}
}
