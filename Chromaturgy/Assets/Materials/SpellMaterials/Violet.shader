Shader "Unlit/Violet"
{
    Properties
    {
        _NoiseTex ("Texture", 2D) = "white" {}
        _Color ("Color", COLOR) = (1, 1, 1, 1)
		_Color2 ("Color 2", COLOR) = (1, 1, 1, 1)
		_OutlineColor("Outline Color", COLOR) = (1, 1, 1, 1)
		_OutlineStrength("Outline Strength", Range(0, 0.2)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPosition : TEXCOORD1;
            };

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float4 _Color;
			float4 _Color2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w * 2;
				float aspectRatio = _ScreenParams.x / _ScreenParams.y;
				textureCoordinate.x *= aspectRatio;
				textureCoordinate.x += textureCoordinate.y;
				textureCoordinate.y += _Time.x;
				float perlin = tex2D(_NoiseTex, textureCoordinate);

                return lerp(_Color, _Color2, perlin);
            }
            ENDCG
        }

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
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;

			float4 _OutlineColor;
			float _OutlineStrength;

			v2f vert(appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);

				float2 perlinCoords = o.uv;
				perlinCoords.x += _Time.x;
				float perlin = tex2Dlod(_NoiseTex, float4(perlinCoords, 0, 0));

				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _OutlineStrength * perlin);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = _OutlineColor;

				return col;
			}
			ENDCG
		}
    }
}
