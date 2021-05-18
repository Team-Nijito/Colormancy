// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:7579,x:34707,y:33413,varname:node_7579,prsc:2|diff-2285-OUT,spec-9116-OUT,gloss-8302-OUT,alpha-2340-OUT,refract-7685-OUT;n:type:ShaderForge.SFN_Color,id:4350,x:31510,y:32385,ptovrint:False,ptlb:Color_1,ptin:_Color_1,varname:_Color_1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5448277,c3:1,c4:1;n:type:ShaderForge.SFN_Fresnel,id:5488,x:31710,y:32836,varname:node_5488,prsc:2|EXP-6683-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6683,x:31509,y:32855,ptovrint:False,ptlb:Water_Power,ptin:_Water_Power,varname:_Water_Power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Lerp,id:5495,x:31940,y:32694,varname:node_5495,prsc:2|A-4350-RGB,B-7838-RGB,T-5488-OUT;n:type:ShaderForge.SFN_Color,id:7838,x:31510,y:32613,ptovrint:False,ptlb:Color_2,ptin:_Color_2,varname:_Color_2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4852941,c2:0.763841,c3:1,c4:1;n:type:ShaderForge.SFN_Step,id:4030,x:32594,y:33490,varname:node_4030,prsc:2|A-2417-OUT,B-1205-OUT;n:type:ShaderForge.SFN_DepthBlend,id:5659,x:31904,y:33263,varname:node_5659,prsc:2|DIST-7008-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7008,x:31432,y:33273,ptovrint:False,ptlb:Blend,ptin:_Blend,varname:_Blend,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Vector1,id:1205,x:32307,y:33461,varname:node_1205,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:2285,x:33219,y:33070,varname:node_2285,prsc:2|A-9015-OUT,B-7423-OUT;n:type:ShaderForge.SFN_Tex2d,id:2868,x:31411,y:33439,varname:_node_2868,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-4919-UVOUT,TEX-5179-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:5179,x:31013,y:34401,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:_Tex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4352,x:31396,y:33655,varname:_node_4352,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-2482-UVOUT,TEX-5179-TEX;n:type:ShaderForge.SFN_Tex2d,id:1774,x:31396,y:33864,varname:_node_1774,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-1013-UVOUT,TEX-5179-TEX;n:type:ShaderForge.SFN_Tex2d,id:6254,x:31396,y:34099,varname:_node_6254,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-3412-UVOUT,TEX-5179-TEX;n:type:ShaderForge.SFN_Multiply,id:2518,x:30783,y:33616,varname:node_2518,prsc:2|A-4743-OUT,B-5264-OUT;n:type:ShaderForge.SFN_Multiply,id:6075,x:30783,y:33776,varname:node_6075,prsc:2|A-4743-OUT,B-582-OUT;n:type:ShaderForge.SFN_Multiply,id:3399,x:30783,y:33936,varname:node_3399,prsc:2|A-4743-OUT,B-7687-OUT;n:type:ShaderForge.SFN_Panner,id:4919,x:31126,y:33442,varname:node_4919,prsc:2,spu:0,spv:1.1|UVIN-4743-OUT,DIST-5224-OUT;n:type:ShaderForge.SFN_Panner,id:2482,x:31140,y:33644,varname:node_2482,prsc:2,spu:0.8,spv:0|UVIN-2518-OUT,DIST-5224-OUT;n:type:ShaderForge.SFN_Panner,id:1013,x:31140,y:33815,varname:node_1013,prsc:2,spu:0,spv:-0.9|UVIN-6075-OUT,DIST-5224-OUT;n:type:ShaderForge.SFN_Panner,id:3412,x:31155,y:34003,varname:node_3412,prsc:2,spu:0.7,spv:0|UVIN-3399-OUT,DIST-5224-OUT;n:type:ShaderForge.SFN_Vector1,id:5264,x:30534,y:33650,varname:node_5264,prsc:2,v1:1.2;n:type:ShaderForge.SFN_Vector1,id:582,x:30534,y:33791,varname:node_582,prsc:2,v1:1.4;n:type:ShaderForge.SFN_Vector1,id:7687,x:30534,y:33913,varname:node_7687,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Blend,id:2692,x:31729,y:33559,varname:node_2692,prsc:2,blmd:0,clmp:True|SRC-2868-R,DST-4352-R;n:type:ShaderForge.SFN_Blend,id:394,x:31743,y:33875,varname:node_394,prsc:2,blmd:0,clmp:True|SRC-1774-R,DST-6254-R;n:type:ShaderForge.SFN_Add,id:6814,x:31931,y:33764,varname:node_6814,prsc:2|A-2692-OUT,B-394-OUT;n:type:ShaderForge.SFN_Multiply,id:2417,x:32307,y:33551,varname:node_2417,prsc:2|A-5659-OUT,B-9413-OUT;n:type:ShaderForge.SFN_Time,id:1187,x:30376,y:34429,varname:node_1187,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5224,x:30613,y:34325,varname:node_5224,prsc:2|A-2856-OUT,B-1187-TSL;n:type:ShaderForge.SFN_ValueProperty,id:2856,x:30346,y:34264,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ConstantClamp,id:9413,x:32102,y:33677,varname:node_9413,prsc:2,min:0.55,max:1|IN-6814-OUT;n:type:ShaderForge.SFN_DepthBlend,id:3323,x:32215,y:31971,varname:node_3323,prsc:2|DIST-2723-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2723,x:31890,y:31958,ptovrint:False,ptlb:Depth_Fade,ptin:_Depth_Fade,varname:_Depth_Fade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Lerp,id:9015,x:32638,y:32422,varname:node_9015,prsc:2|A-3040-OUT,B-5495-OUT,T-3323-OUT;n:type:ShaderForge.SFN_Color,id:4356,x:31850,y:32151,ptovrint:False,ptlb:Depth_Color,ptin:_Depth_Color,varname:_Depth_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1643599,c2:0.3778344,c3:0.5588235,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:2231,x:33313,y:34108,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Lerp,id:3040,x:32160,y:32292,varname:node_3040,prsc:2|A-4356-RGB,B-7838-RGB,T-5488-OUT;n:type:ShaderForge.SFN_Append,id:2365,x:30025,y:33438,varname:node_2365,prsc:2|A-9065-X,B-9065-Z;n:type:ShaderForge.SFN_Multiply,id:4743,x:30448,y:33421,varname:node_4743,prsc:2|A-2365-OUT,B-3245-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:9065,x:29797,y:33405,varname:node_9065,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:3245,x:29985,y:33695,ptovrint:False,ptlb:Tex_World_Scale,ptin:_Tex_World_Scale,varname:_Tex_World_Scale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:7423,x:32826,y:33375,varname:node_7423,prsc:2|A-6598-OUT,B-4030-OUT;n:type:ShaderForge.SFN_OneMinus,id:6598,x:32274,y:33123,varname:node_6598,prsc:2|IN-5659-OUT;n:type:ShaderForge.SFN_Multiply,id:8971,x:33246,y:35244,varname:node_8971,prsc:2|A-290-OUT,B-1668-OUT;n:type:ShaderForge.SFN_Vector1,id:2902,x:32719,y:34972,varname:node_2902,prsc:2,v1:0.003;n:type:ShaderForge.SFN_Lerp,id:1668,x:32914,y:35352,varname:node_1668,prsc:2|A-9963-OUT,B-6782-OUT,T-9413-OUT;n:type:ShaderForge.SFN_Vector2,id:9963,x:32519,y:35277,varname:node_9963,prsc:2,v1:1,v2:1;n:type:ShaderForge.SFN_Negate,id:6782,x:32708,y:35397,varname:node_6782,prsc:2|IN-9963-OUT;n:type:ShaderForge.SFN_Slider,id:9116,x:33456,y:33420,ptovrint:False,ptlb:Spec,ptin:_Spec,varname:_Spec,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Slider,id:8302,x:33472,y:33574,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:290,x:32912,y:35076,varname:node_290,prsc:2|A-2902-OUT,B-2719-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2719,x:32671,y:35138,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:_Refraction,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:2340,x:33492,y:34023,varname:node_2340,prsc:2|A-6423-OUT,B-2231-OUT;n:type:ShaderForge.SFN_Power,id:209,x:33138,y:33950,varname:node_209,prsc:2|VAL-4688-OUT,EXP-4270-OUT;n:type:ShaderForge.SFN_Clamp01,id:6423,x:33313,y:33950,varname:node_6423,prsc:2|IN-209-OUT;n:type:ShaderForge.SFN_Multiply,id:4688,x:32926,y:33930,varname:node_4688,prsc:2|A-9543-OUT,B-4270-OUT;n:type:ShaderForge.SFN_Vector1,id:4270,x:32722,y:34047,varname:node_4270,prsc:2,v1:3;n:type:ShaderForge.SFN_DepthBlend,id:9543,x:32703,y:33913,varname:node_9543,prsc:2|DIST-7008-OUT;n:type:ShaderForge.SFN_DepthBlend,id:9953,x:32747,y:33719,varname:node_9953,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7685,x:33852,y:34449,varname:node_7685,prsc:2|A-9543-OUT,B-8971-OUT;n:type:ShaderForge.SFN_OneMinus,id:5628,x:33644,y:34412,varname:node_5628,prsc:2|IN-9543-OUT;proporder:4350-7838-4356-2231-6683-7008-5179-2856-2723-3245-9116-8302-2719;pass:END;sub:END;*/

Shader "Blockout/Blockout_Water" {
    Properties {
        _Color_1 ("Color_1", Color) = (0,0.5448277,1,1)
        _Color_2 ("Color_2", Color) = (0.4852941,0.763841,1,1)
        _Depth_Color ("Depth_Color", Color) = (0.1643599,0.3778344,0.5588235,1)
        _Alpha ("Alpha", Float ) = 0.5
        _Water_Power ("Water_Power", Float ) = 2
        _Blend ("Blend", Float ) = 1
        _Tex ("Tex", 2D) = "white" {}
        _Speed ("Speed", Float ) = 1
        _Depth_Fade ("Depth_Fade", Float ) = 1
        _Tex_World_Scale ("Tex_World_Scale", Float ) = 0.5
        _Spec ("Spec", Range(0, 1)) = 0.5
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _Refraction ("Refraction", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        GrabPass{ }
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Color_1;
            uniform float _Water_Power;
            uniform float4 _Color_2;
            uniform float _Blend;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float _Speed;
            uniform float _Depth_Fade;
            uniform float4 _Depth_Color;
            uniform float _Alpha;
            uniform float _Tex_World_Scale;
            uniform float _Spec;
            uniform float _Gloss;
            uniform float _Refraction;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_9543 = saturate((sceneZ-partZ)/_Blend);
                float2 node_9963 = float2(1,1);
                float4 node_1187 = _Time + _TimeEditor;
                float node_5224 = (_Speed*node_1187.r);
                float2 node_4743 = (float2(i.posWorld.r,i.posWorld.b)*_Tex_World_Scale);
                float2 node_4919 = (node_4743+node_5224*float2(0,1.1));
                float4 _node_2868 = tex2D(_Tex,TRANSFORM_TEX(node_4919, _Tex));
                float2 node_2482 = ((node_4743*1.2)+node_5224*float2(0.8,0));
                float4 _node_4352 = tex2D(_Tex,TRANSFORM_TEX(node_2482, _Tex));
                float2 node_1013 = ((node_4743*1.4)+node_5224*float2(0,-0.9));
                float4 _node_1774 = tex2D(_Tex,TRANSFORM_TEX(node_1013, _Tex));
                float2 node_3412 = ((node_4743*0.8)+node_5224*float2(0.7,0));
                float4 _node_6254 = tex2D(_Tex,TRANSFORM_TEX(node_3412, _Tex));
                float node_9413 = clamp((saturate(min(_node_2868.r,_node_4352.r))+saturate(min(_node_1774.r,_node_6254.r))),0.55,1);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_9543*((0.003*_Refraction)*lerp(node_9963,(-1*node_9963),node_9413)));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Spec,_Spec,_Spec);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_5488 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_Water_Power);
                float node_5659 = saturate((sceneZ-partZ)/_Blend);
                float3 diffuseColor = (lerp(lerp(_Depth_Color.rgb,_Color_2.rgb,node_5488),lerp(_Color_1.rgb,_Color_2.rgb,node_5488),saturate((sceneZ-partZ)/_Depth_Fade))+((1.0 - node_5659)*step((node_5659*node_9413),0.5)));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float node_4270 = 3.0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(saturate(pow((node_9543*node_4270),node_4270))*_Alpha)),1);
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _Color_1;
            uniform float _Water_Power;
            uniform float4 _Color_2;
            uniform float _Blend;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float _Speed;
            uniform float _Depth_Fade;
            uniform float4 _Depth_Color;
            uniform float _Alpha;
            uniform float _Tex_World_Scale;
            uniform float _Spec;
            uniform float _Gloss;
            uniform float _Refraction;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_9543 = saturate((sceneZ-partZ)/_Blend);
                float2 node_9963 = float2(1,1);
                float4 node_1187 = _Time + _TimeEditor;
                float node_5224 = (_Speed*node_1187.r);
                float2 node_4743 = (float2(i.posWorld.r,i.posWorld.b)*_Tex_World_Scale);
                float2 node_4919 = (node_4743+node_5224*float2(0,1.1));
                float4 _node_2868 = tex2D(_Tex,TRANSFORM_TEX(node_4919, _Tex));
                float2 node_2482 = ((node_4743*1.2)+node_5224*float2(0.8,0));
                float4 _node_4352 = tex2D(_Tex,TRANSFORM_TEX(node_2482, _Tex));
                float2 node_1013 = ((node_4743*1.4)+node_5224*float2(0,-0.9));
                float4 _node_1774 = tex2D(_Tex,TRANSFORM_TEX(node_1013, _Tex));
                float2 node_3412 = ((node_4743*0.8)+node_5224*float2(0.7,0));
                float4 _node_6254 = tex2D(_Tex,TRANSFORM_TEX(node_3412, _Tex));
                float node_9413 = clamp((saturate(min(_node_2868.r,_node_4352.r))+saturate(min(_node_1774.r,_node_6254.r))),0.55,1);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_9543*((0.003*_Refraction)*lerp(node_9963,(-1*node_9963),node_9413)));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Spec,_Spec,_Spec);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float node_5488 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_Water_Power);
                float node_5659 = saturate((sceneZ-partZ)/_Blend);
                float3 diffuseColor = (lerp(lerp(_Depth_Color.rgb,_Color_2.rgb,node_5488),lerp(_Color_1.rgb,_Color_2.rgb,node_5488),saturate((sceneZ-partZ)/_Depth_Fade))+((1.0 - node_5659)*step((node_5659*node_9413),0.5)));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float node_4270 = 3.0;
                fixed4 finalRGBA = fixed4(finalColor * (saturate(pow((node_9543*node_4270),node_4270))*_Alpha),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
