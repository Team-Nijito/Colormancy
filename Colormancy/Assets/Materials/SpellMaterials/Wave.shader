Shader "Unlit/QuickSilver Bottom"
{
    Properties
    {
        _Color("Color", COLOR) = (1, 1, 1, 1)

        _Lerp("Lerp", Range(0, 1)) = 0.5
        _WaveWidth("Wave Width", Range(0, 1)) = 0.5
        _WaveOutline("Wave Outline", Range(0, 1)) = 0.1
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
                float3 objPos : TEXCOORD1;
            };

            float4 _Color;

            float _Lerp;
            float _WaveWidth;
            float _WaveOutline;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.objPos = v.vertex;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = _Color;

                float r = distance(i.uv, float2(0.5, 0.5)) * 2;
                
                float lerpCorrection = _Lerp * (1 + _WaveWidth) - (_WaveWidth / 2);
                float upperBound = lerpCorrection + _WaveWidth / 2;
                float lowerBound = lerpCorrection - _WaveWidth / 2;
                if (r < upperBound && r > lowerBound)
                    col.a = 1;
                else
                    col.a = 0;

                float upperOutline = lerpCorrection + _WaveWidth / 2 * (1 - _WaveOutline);
                if (r > upperOutline)
                    col.rgb = 0;

                return col;
            }
            ENDCG
        }
    }
}
