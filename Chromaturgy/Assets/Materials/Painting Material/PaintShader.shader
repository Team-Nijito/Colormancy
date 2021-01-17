Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Tiling("Tiling", float) = 1
        _ShadowStrength("Shadow Strength", Range(0, 1)) = 1
        _SpecularStrength("Specular Strength", Range(0, 1)) = 1
        _Bumpiness("Bumpiness", float) = 2
        _NormalComparison("Normal Comparison", float) = 0.1
    }
    SubShader
    {
        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 tangent : TANGENT;
            };

            struct v2f
            {
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1)
                float4 worldPosition : TEXCOORD2;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                o.tangent = UnityObjectToWorldNormal(v.tangent);
                TRANSFER_SHADOW(o)
                return o;
            }

            // perlin noise function by ronja
            float rand3dTo1d(float3 value, float3 dotDir = float3(12.9898, 78.233, 37.719)) {
                //make value smaller to avoid artefacts
                float3 smallValue = sin(value);
                //get scalar value from 3d vector
                float random = dot(smallValue, dotDir);
                //make value more random by making it bigger and then taking the factional part
                random = frac(sin(random) * 143758.5453);
                return random;
            }

            float3 rand3dTo3d(float3 value) {
                return float3(
                    rand3dTo1d(value, float3(12.989, 78.233, 37.719)),
                    rand3dTo1d(value, float3(39.346, 11.135, 83.155)),
                    rand3dTo1d(value, float3(73.156, 52.235, 09.151))
                    );
            }

            float easeIn(float interpolator) {
                return interpolator * interpolator * interpolator * interpolator * interpolator;
            }

            float easeOut(float interpolator) {
                return 1 - easeIn(1 - interpolator);
            }

            float easeInOut(float interpolator) {
                float easeInValue = easeIn(interpolator);
                float easeOutValue = easeOut(interpolator);
                return lerp(easeInValue, easeOutValue, interpolator);
            }

            float perlinNoise(float3 value) {
                float3 fraction = frac(value);

                float interpolatorX = easeInOut(fraction.x);
                float interpolatorY = easeInOut(fraction.y);
                float interpolatorZ = easeInOut(fraction.z);

                float3 cellNoiseZ[2];
                [unroll]
                for (int z = 0; z <= 1; z++) {
                    float3 cellNoiseY[2];
                    [unroll]
                    for (int y = 0; y <= 1; y++) {
                        float3 cellNoiseX[2];
                        [unroll]
                        for (int x = 0; x <= 1; x++) {
                            float3 cell = floor(value) + float3(x, y, z);
                            float3 cellDirection = rand3dTo3d(cell) * 2 - 1;
                            float3 compareVector = fraction - float3(x, y, z);
                            cellNoiseX[x] = dot(cellDirection, compareVector);
                        }
                        cellNoiseY[y] = lerp(cellNoiseX[0], cellNoiseX[1], interpolatorX);
                    }
                    cellNoiseZ[z] = lerp(cellNoiseY[0], cellNoiseY[1], interpolatorY);
                }
                float3 noise = lerp(cellNoiseZ[0], cellNoiseZ[1], interpolatorZ);
                return noise;
            }

            float _Tiling;
            float _ShadowStrength;
            float _SpecularStrength;
            float _Bumpiness;
            float _NormalComparison;

            float4 frag (v2f i) : SV_Target
            {
                float4 col = 1;

                float3 binormal = cross(i.normal, i.tangent);
                float3x3 rotation = float3x3(i.tangent, binormal, i.normal);

                float noiseLerp = (perlinNoise(i.worldPosition.xyz * 10) + 1) / 2;
                noiseLerp = smoothstep(0, 1, noiseLerp);
                noiseLerp = smoothstep(0, 1, noiseLerp);

                // calculate "normal" value using tangent space lol
                float3 v1t = i.worldPosition.xyz + i.tangent * _NormalComparison;
                float3 v2t = i.worldPosition.xyz - i.tangent * _NormalComparison;
                float3 v1b = i.worldPosition.xyz + binormal * _NormalComparison;
                float3 v2b = i.worldPosition.xyz - binormal * _NormalComparison;

                v1t = v1t + i.normal * (perlinNoise(v1t * 10) + 1) / 2 * _Bumpiness;
                v2t = v2t + i.normal * (perlinNoise(v2t * 10) + 1) / 2 * _Bumpiness;
                v1b = v1b + i.normal * (perlinNoise(v1b * 10) + 1) / 2 * _Bumpiness;
                v2b = v2b + i.normal * (perlinNoise(v2b * 10) + 1) / 2 * _Bumpiness;

                float3 bump = normalize(cross(v2t - v1t, v2b - v1b));

                // use half lambertian
                half nl = pow(max(0, dot(i.normal, _WorldSpaceLightPos0.xyz)) * 0.5 + 0.5, 2);
                fixed shadow = pow(SHADOW_ATTENUATION(i) * _ShadowStrength + (1 - _ShadowStrength), 2);

                // specular calculation
                half s = 0;


                float g = 1 - i.color.a;
                if (noiseLerp < g) {
                    col.rgb = i.color.rgb;
                    // normal is in same direction as light or shadowed
                    if (dot(i.normal, _WorldSpaceLightPos0.xyz) > 0) {
                        nl = pow(max(0, dot(bump, _WorldSpaceLightPos0.xyz)) * 0.5 + 0.5, 2);
                        s = pow(max(0, dot(reflect(-normalize(_WorldSpaceLightPos0.xyz), bump), normalize(_WorldSpaceCameraPos - i.worldPosition))), 2) * _SpecularStrength;
                    }
                }
                // possible fix darker color by changing hue?
                if (i.color.a == 1) {
                    col.rgb = 1;
                }
                
                col *= (nl + s) * _LightColor0 * shadow;
                col.rgb += ShadeSH9(half4(i.normal, 1));
                

                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
