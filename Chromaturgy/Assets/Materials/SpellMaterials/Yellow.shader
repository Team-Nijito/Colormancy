Shader "Unlit/Yellow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", COLOR) = (1, 1, 1, 1)
		_ColorFresnel ("Color Fresnel", COLOR) = (1, 1, 1, 1)
		_RotationSpeed("Rotation Speed", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 position : TEXCOORD1;
				float3 worldNormal : NORMAL;
				float3 worldPosition : TEXCOORD2;
            };

			float4 _Color;
			float4 _ColorFresnel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.position = v.vertex;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;

				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPosition);
				float fresnel = 1 - pow(dot(viewDirection, i.worldNormal), 2);

				col = lerp(col, _ColorFresnel, fresnel);

				return col;
            }
            ENDCG
        }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

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
				float3 position : TEXCOORD1;
			};

			float4 _Color;
			float _RotationSpeed;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * 0.02);
				o.uv = v.uv;
				o.position = v.vertex;
				return o;
			}

			float rand(float2 co) { return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453); }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = 0;

				// turn the sphere into a grid
				i.uv.x += _Time.x * _RotationSpeed;

				int x = i.uv.x * 30 % 30;

				float vGradient = frac(i.position.y * 10 + 0.25);
				int grid = rand(float2(0, (int)(i.position.y * 10 + 0.25 + 10))) * 30;

				// remove parts
				if (vGradient < 0.5 && x != grid)
					col = 1;

				if (vGradient > 0.5 && x - 1 == grid)
					col = 1;

				return col;
				
			}
			ENDCG
		}
    }
}
