Shader "Unlit/QuickSilver Bolt"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollTex("Scroll Texture", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", float) = 1
        _DisplacementStrength("Displacement Strength", Range(0, 1)) = 0
        _Thickness("Thickness", Range(0, 1)) = 0
        _Width("Width", Range(0, 3)) = 1
        _TopWidth ("Top Width", float) = 1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _ScrollTex;
            float4 _ScrollTex_ST;

            float _ScrollSpeed;
            float _DisplacementStrength;

            float _Thickness;
            float _Width;
            float _TopWidth;

            v2f vert (appdata v)
            {
                v2f o;

                float2 scrollUV = v.uv + float2(0, _Time.y * _ScrollSpeed);

                float displacement = tex2Dlod(_ScrollTex, float4(scrollUV, 0.0, 0.0)).x;
                displacement -= 0.5;
                displacement *= 2;
                v.vertex.x += displacement * _DisplacementStrength * v.uv.y;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = 1;

                // shrink the uv at the bottom and top
                i.uv.x -= 0.5;
                i.uv.x /= i.uv.y * _TopWidth;
                i.uv.x /= pow(1 - i.uv.y, 0.5);
                i.uv.x /= _Width;
                i.uv.x += 0.5;
                i.uv.x = clamp(i.uv.x, 0, 1);

                // handle the bolt visually
                fixed4 bolt = tex2D(_MainTex, i.uv);
                float mask = step(_Thickness, bolt.r);
                float remap = (bolt.r - _Thickness) / (1 - _Thickness);
                float lerp = remap * mask;

                col.a = lerp;

                if (_Thickness == 1)
                    col.a = 0;

                // handle flashing
                float timeTick = uint(_Time.y * 20) % 10;
                if (timeTick == 0 || timeTick == 3)
                    col.a = 0;

                return col;
            }
            ENDCG
        }
    }
}
