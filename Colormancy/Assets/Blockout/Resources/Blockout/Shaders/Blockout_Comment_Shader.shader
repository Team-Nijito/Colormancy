// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:True,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7063,x:33872,y:33022,varname:node_7063,prsc:2|diff-8855-OUT,spec-4165-OUT,gloss-9454-OUT,alpha-6319-OUT;n:type:ShaderForge.SFN_Multiply,id:8765,x:32140,y:32539,varname:node_8765,prsc:2|A-3570-RGB,B-3747-OUT;n:type:ShaderForge.SFN_Color,id:3570,x:31831,y:32400,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:7396,x:32379,y:32718,varname:node_7396,prsc:2|A-8765-OUT,B-6369-OUT;n:type:ShaderForge.SFN_Clamp01,id:2317,x:32570,y:32736,varname:node_2317,prsc:2|IN-7396-OUT;n:type:ShaderForge.SFN_Multiply,id:6369,x:32185,y:32848,varname:node_6369,prsc:2|A-2928-OUT,B-9940-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9940,x:31831,y:32871,ptovrint:False,ptlb:Drop_Value,ptin:_Drop_Value,varname:_Drop_Value,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Slider,id:4165,x:32028,y:33018,ptovrint:False,ptlb:Metalic,ptin:_Metalic,varname:_Metalic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:9454,x:32028,y:33176,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Dot,id:9243,x:29753,y:31627,varname:node_9243,prsc:2,dt:3|A-1653-OUT,B-5654-OUT;n:type:ShaderForge.SFN_NormalVector,id:1653,x:29485,y:31544,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector3,id:5654,x:29485,y:31723,varname:node_5654,prsc:2,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Vector3,id:8132,x:29485,y:31842,varname:node_8132,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Vector3,id:7766,x:29485,y:31972,varname:node_7766,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Dot,id:8539,x:29753,y:31813,varname:node_8539,prsc:2,dt:3|A-1653-OUT,B-8132-OUT;n:type:ShaderForge.SFN_Dot,id:8068,x:29753,y:31973,varname:node_8068,prsc:2,dt:3|A-1653-OUT,B-7766-OUT;n:type:ShaderForge.SFN_Power,id:39,x:30032,y:31751,varname:node_39,prsc:2|VAL-9243-OUT,EXP-9724-OUT;n:type:ShaderForge.SFN_Power,id:3474,x:30032,y:31905,varname:node_3474,prsc:2|VAL-8539-OUT,EXP-9724-OUT;n:type:ShaderForge.SFN_Power,id:9210,x:30032,y:32083,varname:node_9210,prsc:2|VAL-8068-OUT,EXP-9724-OUT;n:type:ShaderForge.SFN_Vector1,id:9724,x:29753,y:32183,varname:node_9724,prsc:2,v1:3;n:type:ShaderForge.SFN_FragmentPosition,id:3037,x:28744,y:33050,varname:node_3037,prsc:2;n:type:ShaderForge.SFN_Append,id:33,x:29407,y:33012,varname:node_33,prsc:2|A-3037-X,B-3037-Z;n:type:ShaderForge.SFN_Append,id:3126,x:29157,y:32622,varname:node_3126,prsc:2|A-3037-Z,B-3037-Y;n:type:ShaderForge.SFN_Append,id:9497,x:29246,y:33264,varname:node_9497,prsc:2|A-3037-X,B-3037-Y;n:type:ShaderForge.SFN_Tex2dAsset,id:2434,x:29245,y:32271,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:_Tex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:403ab31ff043eff4ea4124a30eb5cdcc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5907,x:30034,y:32691,varname:_node_5907,prsc:2,tex:403ab31ff043eff4ea4124a30eb5cdcc,ntxv:0,isnm:False|UVIN-3126-OUT,TEX-2434-TEX;n:type:ShaderForge.SFN_Tex2d,id:4242,x:30037,y:33004,varname:_node_4242,prsc:2,tex:403ab31ff043eff4ea4124a30eb5cdcc,ntxv:0,isnm:False|UVIN-33-OUT,TEX-2434-TEX;n:type:ShaderForge.SFN_Tex2d,id:5544,x:30037,y:33264,varname:_node_5544,prsc:2,tex:403ab31ff043eff4ea4124a30eb5cdcc,ntxv:0,isnm:False|UVIN-9497-OUT,TEX-2434-TEX;n:type:ShaderForge.SFN_Add,id:3747,x:31259,y:33176,varname:node_3747,prsc:2|A-3277-OUT,B-2988-OUT,C-9823-OUT;n:type:ShaderForge.SFN_Multiply,id:3277,x:30986,y:32963,varname:node_3277,prsc:2|A-39-OUT,B-5907-RGB;n:type:ShaderForge.SFN_Multiply,id:2988,x:30986,y:33144,varname:node_2988,prsc:2|A-3474-OUT,B-4242-RGB;n:type:ShaderForge.SFN_Multiply,id:9823,x:30986,y:33286,varname:node_9823,prsc:2|A-9210-OUT,B-5544-RGB;n:type:ShaderForge.SFN_Multiply,id:1263,x:30971,y:32736,varname:node_1263,prsc:2|A-9210-OUT,B-5544-A;n:type:ShaderForge.SFN_Multiply,id:8241,x:30971,y:32584,varname:node_8241,prsc:2|A-3474-OUT,B-4242-A;n:type:ShaderForge.SFN_Multiply,id:1206,x:30971,y:32432,varname:node_1206,prsc:2|A-39-OUT,B-5907-A;n:type:ShaderForge.SFN_Add,id:2928,x:31198,y:32584,varname:node_2928,prsc:2|A-1206-OUT,B-8241-OUT,C-1263-OUT;n:type:ShaderForge.SFN_Fresnel,id:4285,x:32645,y:32555,varname:node_4285,prsc:2;n:type:ShaderForge.SFN_Lerp,id:6873,x:33050,y:32402,varname:node_6873,prsc:2|A-6687-OUT,B-7438-OUT,T-4285-OUT;n:type:ShaderForge.SFN_Vector1,id:6687,x:32656,y:32391,varname:node_6687,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7438,x:32680,y:32459,varname:node_7438,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:8855,x:33310,y:32911,varname:node_8855,prsc:2|A-6873-OUT,B-2317-OUT;n:type:ShaderForge.SFN_DepthBlend,id:6101,x:32915,y:33678,varname:node_6101,prsc:2|DIST-8171-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8171,x:32675,y:33585,ptovrint:False,ptlb:Depth_Blend,ptin:_Depth_Blend,varname:_Depth_Blend,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:6319,x:33289,y:33490,varname:node_6319,prsc:2|A-3570-A,B-6101-OUT;proporder:3570-9940-4165-9454-2434-8171;pass:END;sub:END;*/

Shader "Blockout/Blockout_Shader_Comment" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Drop_Value ("Drop_Value", Float ) = 0.5
        _Metalic ("Metalic", Range(0, 1)) = 0.5
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _Tex ("Tex", 2D) = "white" {}
        _Depth_Blend ("Depth_Blend", Float ) = 0.1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _Drop_Value;
            uniform float _Metalic;
            uniform float _Gloss;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float _Depth_Blend;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                UNITY_FOG_COORDS(7)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD8;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metalic;
                float specularMonochrome;
                float node_9724 = 3.0;
                float node_39 = pow(abs(dot(i.normalDir,float3(1,0,0))),node_9724);
                float2 node_3126 = float2(i.posWorld.b,i.posWorld.g);
                float4 _node_5907 = tex2D(_Tex,TRANSFORM_TEX(node_3126, _Tex));
                float node_3474 = pow(abs(dot(i.normalDir,float3(0,1,0))),node_9724);
                float2 node_33 = float2(i.posWorld.r,i.posWorld.b);
                float4 _node_4242 = tex2D(_Tex,TRANSFORM_TEX(node_33, _Tex));
                float node_9210 = pow(abs(dot(i.normalDir,float3(0,0,1))),node_9724);
                float2 node_9497 = float2(i.posWorld.r,i.posWorld.g);
                float4 _node_5544 = tex2D(_Tex,TRANSFORM_TEX(node_9497, _Tex));
                float3 diffuseColor = (lerp(1.0,0.5,(1.0-max(0,dot(normalDirection, viewDirection))))*saturate(((_Color.rgb*((node_39*_node_5907.rgb)+(node_3474*_node_4242.rgb)+(node_9210*_node_5544.rgb)))+(((node_39*_node_5907.a)+(node_3474*_node_4242.a)+(node_9210*_node_5544.a))*_Drop_Value)))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,(_Color.a*saturate((sceneZ-partZ)/_Depth_Blend)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _Drop_Value;
            uniform float _Metalic;
            uniform float _Gloss;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float _Depth_Blend;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                float4 projPos : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metalic;
                float specularMonochrome;
                float node_9724 = 3.0;
                float node_39 = pow(abs(dot(i.normalDir,float3(1,0,0))),node_9724);
                float2 node_3126 = float2(i.posWorld.b,i.posWorld.g);
                float4 _node_5907 = tex2D(_Tex,TRANSFORM_TEX(node_3126, _Tex));
                float node_3474 = pow(abs(dot(i.normalDir,float3(0,1,0))),node_9724);
                float2 node_33 = float2(i.posWorld.r,i.posWorld.b);
                float4 _node_4242 = tex2D(_Tex,TRANSFORM_TEX(node_33, _Tex));
                float node_9210 = pow(abs(dot(i.normalDir,float3(0,0,1))),node_9724);
                float2 node_9497 = float2(i.posWorld.r,i.posWorld.g);
                float4 _node_5544 = tex2D(_Tex,TRANSFORM_TEX(node_9497, _Tex));
                float3 diffuseColor = (lerp(1.0,0.5,(1.0-max(0,dot(normalDirection, viewDirection))))*saturate(((_Color.rgb*((node_39*_node_5907.rgb)+(node_3474*_node_4242.rgb)+(node_9210*_node_5544.rgb)))+(((node_39*_node_5907.a)+(node_3474*_node_4242.a)+(node_9210*_node_5544.a))*_Drop_Value)))); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * (_Color.a*saturate((sceneZ-partZ)/_Depth_Blend)),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _Drop_Value;
            uniform float _Metalic;
            uniform float _Gloss;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float node_9724 = 3.0;
                float node_39 = pow(abs(dot(i.normalDir,float3(1,0,0))),node_9724);
                float2 node_3126 = float2(i.posWorld.b,i.posWorld.g);
                float4 _node_5907 = tex2D(_Tex,TRANSFORM_TEX(node_3126, _Tex));
                float node_3474 = pow(abs(dot(i.normalDir,float3(0,1,0))),node_9724);
                float2 node_33 = float2(i.posWorld.r,i.posWorld.b);
                float4 _node_4242 = tex2D(_Tex,TRANSFORM_TEX(node_33, _Tex));
                float node_9210 = pow(abs(dot(i.normalDir,float3(0,0,1))),node_9724);
                float2 node_9497 = float2(i.posWorld.r,i.posWorld.g);
                float4 _node_5544 = tex2D(_Tex,TRANSFORM_TEX(node_9497, _Tex));
                float3 diffColor = (lerp(1.0,0.5,(1.0-max(0,dot(normalDirection, viewDirection))))*saturate(((_Color.rgb*((node_39*_node_5907.rgb)+(node_3474*_node_4242.rgb)+(node_9210*_node_5544.rgb)))+(((node_39*_node_5907.a)+(node_3474*_node_4242.a)+(node_9210*_node_5544.a))*_Drop_Value))));
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metalic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
