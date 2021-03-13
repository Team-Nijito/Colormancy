Shader "Unlit/Orange"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FrontCurvature("Front Curvature", float) = 1
        _FrontDisplacementStrength("Front Displacement Strength", float) = 1
        _Cutoff("Cutoff", Range(0, 1)) = 0
        _ScrollSpeed("Scroll Speed", float) = 1

        _FrontColor("Front Color", COLOR) = (1, 1, 1, 1)


        _Color("Color", COLOR) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

        Pass
        {
            Cull Off
            ZWrite Off
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _FrontCurvature;
            float _FrontDisplacementStrength;
            float _Cutoff;

            float _ScrollSpeed;

            float4 _FrontColor;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float orangeFire = 0;
                float yellowFire = 0;
                float perlin = tex2D(_MainTex, i.uv - _Time.x * _ScrollSpeed);

                float orangeCurve = pow(abs(i.uv.y - 0.5), _FrontCurvature) * _FrontDisplacementStrength;
                orangeFire = step(orangeCurve, i.uv.x);

                float orangeFireLerp = 1 - (i.uv.x - _Cutoff)/(1 - _Cutoff);
                orangeFireLerp *= 1 - pow(abs(i.uv.y - 0.5) * 2, 4);
                float orangeFireEnd = step(perlin, orangeFireLerp);
                orangeFire *= orangeFireEnd;

                float displacement = tex2D(_MainTex, float2(0, abs(i.uv.y - 0.5)) - float2(_Time.x * 5, 0));
                float yellowCurve = pow(abs(i.uv.y - 0.5), _FrontCurvature) * displacement * _FrontDisplacementStrength;
                yellowFire = step(yellowCurve, i.uv.x - 0.01);

                float yellowFireLerp = 1 - (i.uv.x - _Cutoff) / (1 - _Cutoff) * 2;
                yellowFireLerp *= 1 - pow(abs(i.uv.y - 0.5) * 2, i.uv.x * 15);
                float yellowFireEnd = step(perlin, yellowFireLerp);
                yellowFire *= yellowFireEnd;


                float4 composite = orangeFire * _Color;
                if (composite.a != 0 && yellowFire != 0)
                    composite.rgb = _FrontColor.rgb;

                return composite;
            }
            ENDCG
        }
    }
}
