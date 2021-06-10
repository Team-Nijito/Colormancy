Shader "Unlit/CircleArrowsPreviewShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", COLOR) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float4 pattern = tex2D(_MainTex, i.uv);
                    float g = (pattern.r + pattern.g + pattern.b) / 3;
                    float4 col = _Color;
                    
                    col.a = pattern.a;

                    if (pattern.a != 0) {
                        col.a = max(0, (g - 0.3) * 2);

                        float radius = pow(distance(i.uv, float2(0.5, 0.5)) * 2, 0.5);

                        if (radius < frac(_Time.y / 1.5) + 0.1 && radius > frac(_Time.y / 1.5) - 0.1)
                            col.a = 1;
                    }

                    return col;
                }
                ENDCG
            }
        }
}
