Shader "Unlit/Brown Impact"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Lerp("Lerp", Range(0, 1)) = 1
        _Spikiness("Spikiness", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front

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

            float _Lerp;
            float _Spikiness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = 1;
                fixed height = tex2D(_MainTex, i.uv);

                float fixedLerp = _Lerp - (1 / _Spikiness);
                if (i.uv.y > height / _Spikiness + fixedLerp)
                    col.a = 0;

                col.a *= 1 - _Lerp;

                return col;
            }
            ENDCG
        }
    }
}
