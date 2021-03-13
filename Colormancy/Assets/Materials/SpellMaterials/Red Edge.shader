Shader "Unlit/Red Edge"
{
    Properties
    {
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _Color("Color", COLOR) = (1, 1, 1, 1)
        _Spikiness("Spikiness", Range(0, 1)) = 0
        _Darkness("Darkness", Range(0, 1)) = 0
        _YScale("Y Scale", float) = 1
        _Lerp("Lerp", Range(0, 1)) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

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
                float3 objectPos : TEXCOORD1;
            };

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float _Lerp;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                o.objectPos = v.vertex;
                return o;
            }

            float4 _Color;
            float _Spikiness;
            float _Darkness;
            float _YScale;


            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = _Color;

                float height = tex2D(_NoiseTex, i.objectPos.xz * _Spikiness + _Time.x) * _YScale * _Lerp;

                if (i.objectPos.y > height * 0.5)
                    col.w = 0;
                else {
                    col = lerp(float4(0, 0, 0, 1), _Color, pow(i.objectPos.y / height, _Darkness));
                    col.w *= _Lerp;
                }

                return col;
            }
            ENDCG
        }
    }
}
