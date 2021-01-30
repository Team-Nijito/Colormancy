Shader "Unlit/QuickSilver Bottom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Lerp("Lerp", float) = 0.5
    }
    SubShader
    {
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float r = distance(i.uv, float2(0.5, 0.5)) * 2;

                fixed4 col = tex2D(_MainTex, i.uv);
                
                if (r < _Lerp)
                    r = 1;
                else
                    r = 0;

                return r;
            }
            ENDCG
        }
    }
}
