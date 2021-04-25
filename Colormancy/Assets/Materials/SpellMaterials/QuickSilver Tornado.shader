Shader "Unlit/QuickSilver Tornado"
{
    Properties
    {
		_Color ("Color", COLOR) = (1, 1, 1, 1)

        _NoiseTex ("Texture", 2D) = "white" {}
		_DisplacementDistance("Displacement Distance", float) = 0
		_DisplacementOffset("Displacement Offset", float) = 0
		_BottomOffset("Bottom Offset", float) = 0
		_VerticalScaling("Vertical Scaling", float) = 0

		_UVScrollX("UV Scroll X", float) = 0
		_UVScrollY("UV Scroll Y", float) = 0

		_ColorThreshold("Color Threshold", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha
		

        Pass
        {
			Cull Back

			Stencil {
				Ref 2
				Comp always
				Pass replace
			}

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

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

			float _DisplacementDistance;
			float _DisplacementOffset;
			float _BottomOffset;
			float _VerticalScaling;

			float _UVScrollX;
			float _UVScrollY;

			float _ColorThreshold;
			float4 _Color;

			v2f vert(appdata v)
			{
				v2f o;

				// scale horizontal slice
				float3 pointToCenter = normalize(float3(v.vertex.x, 0, v.vertex.z));
				v.vertex.xyz += pointToCenter * pow(v.vertex.y * _VerticalScaling, 4);

				// calculate radial offset
				float radius = v.vertex.y + _BottomOffset;
				float angle = _Time.y + v.vertex.y * _DisplacementOffset;

				float displacement_x = radius * sin(angle);
				float displacement_y = radius * cos(angle);

				v.vertex.x += displacement_x * _DisplacementDistance;
				v.vertex.z += displacement_y * _DisplacementDistance;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = _Color;

				i.uv.x += _Time.x * _UVScrollX + i.uv.y * 0.2;
				i.uv.y -= _Time.x * _UVScrollY;

				float noise = tex2D(_NoiseTex, i.uv);
				col.a = pow(smoothstep(_ColorThreshold, 1, noise), 0.1);

				return col;
			}
            ENDCG
        }
		Pass
		{
			Cull Front

			Stencil {
				Ref 2
				Comp always
				Pass replace
			}

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

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;

			float _DisplacementDistance;
			float _DisplacementOffset;
			float _BottomOffset;
			float _VerticalScaling;

			float _UVScrollX;
			float _UVScrollY;

			float _ColorThreshold;
			float4 _Color;

			v2f vert(appdata v)
			{
				v2f o;

				// scale horizontal slice
				float3 pointToCenter = normalize(float3(v.vertex.x, 0, v.vertex.z));
				v.vertex.xyz += pointToCenter * pow(v.vertex.y * _VerticalScaling, 4);

				// calculate radial offset
				float radius = v.vertex.y + _BottomOffset;
				float angle = _Time.y + v.vertex.y * _DisplacementOffset;

				float displacement_x = radius * sin(angle);
				float displacement_y = radius * cos(angle);

				v.vertex.x += displacement_x * _DisplacementDistance;
				v.vertex.z += displacement_y * _DisplacementDistance;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = _Color;

				i.uv.x += _Time.x * _UVScrollX + i.uv.y * 0.2;
				i.uv.y -= _Time.x * _UVScrollY;

				float noise = tex2D(_NoiseTex, i.uv);
				col.a = pow(smoothstep(_ColorThreshold, 1, noise), 0.4);

				return col;
			}
			ENDCG
		}
    }
}
