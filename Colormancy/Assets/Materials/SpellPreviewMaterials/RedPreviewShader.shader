Shader "Unlit/RedPreviewShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", COLOR) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        
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

            float4 frag(v2f i) : SV_Target
            {
                i.uv -= float2(0.5, 0.5);
                float2x2 rotMat = { cos(_Time.x), -sin(_Time.x), sin(_Time.x), cos(_Time.x) };

                i.uv = mul(i.uv, rotMat);
                i.uv += float2(0.5, 0.5);

                float4 pattern = tex2D(_MainTex, i.uv);
                float g = (pattern.r + pattern.g + pattern.b) / 3;
                float4 col = _Color;
                col.a = abs(sin(_Time.y));

                if (g > 0.9)
                    col.a = 0;

                return col;
            }
            ENDCG
        }
    }
}
