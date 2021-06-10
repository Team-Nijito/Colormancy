Shader "Unlit/BoxPreviewShader"
{
    Properties
    {
        _Color("Color", COLOR) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

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
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = _Color;
                float _TimeMul = 10;

                if (!(abs(i.uv.x - 0.5) > 0.45 || abs(i.uv.y - 0.5) > 0.45)) {
                    col.a = 0;
                }
                else
                    col.a = 1;

                if (i.uv.y > frac(_Time.x * _TimeMul) - 0.1 && i.uv.y < frac(_Time.x * _TimeMul) + 0.1) {
                    col.a = max(col.a, lerp(0, 1, (i.uv.y - (frac(_Time.x * _TimeMul) - 0.1)) / 0.2));
                }

                return col;
            }
            ENDCG
        }
    }
}
