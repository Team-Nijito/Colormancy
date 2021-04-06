Shader "Unlit/QuickSilver Contact"
{
    Properties
    {
        _Radius("Radius", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                float3 normal : TEXCOORD1;
                float4 objectCenter : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Radius;

            v2f vert (appdata v)
            {
                v2f o;

                v.vertex += float4(v.normal, 0) * _Radius;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                o.objectCenter = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = 1;

                float3 cameraToCenter = normalize(_WorldSpaceCameraPos.xyz - i.objectCenter.xyz);
                float fresnel = dot(cameraToCenter, i.normal);
                fresnel = pow(fresnel, 2);
                fresnel = smoothstep(0, 1, fresnel);
                
                col.a = fresnel;

                return col;
            }
            ENDCG
        }
    }
}
