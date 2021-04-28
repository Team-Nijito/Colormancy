Shader "Unlit/Brown Rock"
{
    Properties
    {
        _Color("Color", COLOR) = (1, 1, 1, 1)
        _ShadowColor("Shadow Color", COLOR) = (1, 1, 1, 1)
        _Threshold("Threshold", Range(-1, 1)) = 0
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
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            float4 _Color;
            float4 _ShadowColor;
            float _Threshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;

                //float3 lighttovert = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float diffuse = dot(_WorldSpaceLightPos0.xyz, i.normal);

                if (diffuse < _Threshold)
                    col = _ShadowColor;

                return col;
            }
            ENDCG
        }
    }
}
