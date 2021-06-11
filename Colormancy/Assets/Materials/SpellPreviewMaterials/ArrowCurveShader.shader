Shader "Unlit/ArrowCurveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 arrows = tex2D(_MainTex, i.uv);
                float4 col = _Color * arrows.a;
                float _TimeMul = 7;

                float radius = distance(i.uv, float2(0.5, 0.5)) + 0.5;

                col.a *= radius;

                if (radius < frac(_Time.x * _TimeMul) + 0.1 && radius > frac(_Time.x * _TimeMul) - 0.1 && col.a != 0)
                    col.a = 1;

                return col;
            }
            ENDCG
        }
    }
}
