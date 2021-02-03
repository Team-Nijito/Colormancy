Shader "Unlit/Red Base"
{
    Properties
    {
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _Color("Color", COLOR) = (1, 1, 1, 1)
        _Spikiness("Spikiness", Range(0, 1)) = 1
        _Lerp("Lerp", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float4 _Color;
            float _Spikiness;
            float _Lerp;

            fixed4 frag(v2f i) : SV_Target
            {
                const float _PI = 3.14159;
                float4 col;

                float radius = distance(i.uv, float2(0.5, 0.5)) * 2 * _Lerp;

                float2 rvector = (i.uv - float2(0.5, 0.5)) * 2;
                float angle = atan(rvector.y / rvector.x) / (_PI / 2);

                float noise = tex2D(_NoiseTex, float2(radius * _Spikiness - _Time.x, angle));
                float colorLerp = 1 - (noise * (1 - pow(radius, 3)));
                col.rgb = lerp(float4(0, 0, 0, 1), _Color, colorLerp);
                col.w = colorLerp * _Lerp;

                return col;
            }
            ENDCG
        }
    }
}
