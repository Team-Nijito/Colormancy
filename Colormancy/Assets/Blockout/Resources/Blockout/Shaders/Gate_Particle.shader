// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:5278,x:34269,y:33095,varname:node_5278,prsc:2|emission-1953-OUT,alpha-5023-OUT,refract-5508-OUT,voffset-1027-OUT;n:type:ShaderForge.SFN_Tex2d,id:3505,x:30464,y:33165,varname:_node_3505,prsc:2,tex:eae322cc193ee15499a2d71dccf466b0,ntxv:0,isnm:False|UVIN-5541-UVOUT,TEX-5438-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:5438,x:29966,y:32806,ptovrint:False,ptlb:Effects_Texture,ptin:_Effects_Texture,varname:_Effects_Texture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:eae322cc193ee15499a2d71dccf466b0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5935,x:32429,y:32508,varname:node_5935,prsc:2|A-9130-RGB,B-1326-RGB,T-550-OUT;n:type:ShaderForge.SFN_Color,id:9130,x:31949,y:32108,ptovrint:False,ptlb:Color_1,ptin:_Color_1,varname:_Color_1,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9117647,c2:0.9117647,c3:0.9117647,c4:1;n:type:ShaderForge.SFN_Color,id:1326,x:31909,y:32340,ptovrint:False,ptlb:Color_2,ptin:_Color_2,varname:_Color_2,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3014706,c2:0.3014706,c3:0.3014706,c4:1;n:type:ShaderForge.SFN_Append,id:995,x:32350,y:33311,varname:node_995,prsc:2|A-150-OUT,B-150-OUT;n:type:ShaderForge.SFN_Multiply,id:5474,x:32544,y:33356,varname:node_5474,prsc:2|A-995-OUT,B-2669-OUT;n:type:ShaderForge.SFN_Vector1,id:2669,x:32321,y:33458,varname:node_2669,prsc:2,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:794,x:32360,y:32898,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:_Alpha,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.6;n:type:ShaderForge.SFN_Tex2d,id:9646,x:30464,y:33361,varname:_node_9646,prsc:2,tex:eae322cc193ee15499a2d71dccf466b0,ntxv:0,isnm:False|UVIN-9898-UVOUT,TEX-5438-TEX;n:type:ShaderForge.SFN_Tex2d,id:7015,x:30464,y:33550,varname:_node_7015,prsc:2,tex:eae322cc193ee15499a2d71dccf466b0,ntxv:0,isnm:False|UVIN-680-UVOUT,TEX-5438-TEX;n:type:ShaderForge.SFN_Tex2d,id:569,x:30464,y:33750,varname:_node_569,prsc:2,tex:eae322cc193ee15499a2d71dccf466b0,ntxv:0,isnm:False|UVIN-855-UVOUT,TEX-5438-TEX;n:type:ShaderForge.SFN_Blend,id:9784,x:31267,y:33220,varname:node_9784,prsc:2,blmd:10,clmp:True|SRC-3505-R,DST-9646-G;n:type:ShaderForge.SFN_Blend,id:2067,x:31280,y:33540,varname:node_2067,prsc:2,blmd:17,clmp:True|SRC-7015-R,DST-569-G;n:type:ShaderForge.SFN_Blend,id:3495,x:31515,y:33396,varname:node_3495,prsc:2,blmd:18,clmp:True|SRC-9784-OUT,DST-2067-OUT;n:type:ShaderForge.SFN_Rotator,id:5541,x:29979,y:33107,varname:node_5541,prsc:2|UVIN-9161-UVOUT,ANG-6144-OUT;n:type:ShaderForge.SFN_Rotator,id:9898,x:29979,y:33287,varname:node_9898,prsc:2|UVIN-9161-UVOUT,ANG-3814-OUT;n:type:ShaderForge.SFN_Rotator,id:680,x:29979,y:33470,varname:node_680,prsc:2|UVIN-9161-UVOUT,ANG-9425-OUT;n:type:ShaderForge.SFN_Rotator,id:855,x:29979,y:33652,varname:node_855,prsc:2|UVIN-9161-UVOUT,ANG-2862-OUT;n:type:ShaderForge.SFN_TexCoord,id:9161,x:29458,y:32858,varname:node_9161,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:2887,x:29129,y:33935,varname:node_2887,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2862,x:29414,y:33574,varname:node_2862,prsc:2|A-2354-OUT,B-2887-TSL;n:type:ShaderForge.SFN_ValueProperty,id:2122,x:29114,y:33131,ptovrint:False,ptlb:Spinner_1,ptin:_Spinner_1,varname:_Spinner_1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9425,x:29414,y:33428,varname:node_9425,prsc:2|A-1918-OUT,B-2887-TSL;n:type:ShaderForge.SFN_Multiply,id:6144,x:29414,y:33125,varname:node_6144,prsc:2|A-2122-OUT,B-2887-TSL;n:type:ShaderForge.SFN_Multiply,id:3814,x:29414,y:33271,varname:node_3814,prsc:2|A-4162-OUT,B-2887-TSL;n:type:ShaderForge.SFN_ValueProperty,id:4162,x:29114,y:33250,ptovrint:False,ptlb:Spinner_2,ptin:_Spinner_2,varname:_Spinner_2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.8;n:type:ShaderForge.SFN_ValueProperty,id:9211,x:28961,y:33353,ptovrint:False,ptlb:Spinner_3,ptin:_Spinner_3,varname:_Spinner_3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:1827,x:28984,y:33537,ptovrint:False,ptlb:Spinner_4,ptin:_Spinner_4,varname:_Spinner_4,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.6;n:type:ShaderForge.SFN_Negate,id:1918,x:29138,y:33366,varname:node_1918,prsc:2|IN-9211-OUT;n:type:ShaderForge.SFN_Negate,id:2354,x:29173,y:33584,varname:node_2354,prsc:2|IN-1827-OUT;n:type:ShaderForge.SFN_NormalVector,id:5955,x:32596,y:33627,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:3445,x:32769,y:33603,varname:node_3445,prsc:2|A-3495-OUT,B-5955-OUT;n:type:ShaderForge.SFN_Multiply,id:1027,x:33129,y:33505,varname:node_1027,prsc:2|A-4465-OUT,B-3445-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4465,x:32743,y:33221,ptovrint:False,ptlb:Wilbblyocity,ptin:_Wilbblyocity,varname:_Wilbblyocity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:2913,x:30836,y:32758,varname:node_2913,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Length,id:5854,x:31187,y:32776,varname:node_5854,prsc:2|IN-7698-OUT;n:type:ShaderForge.SFN_RemapRange,id:7698,x:31017,y:32758,varname:node_7698,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-2913-UVOUT;n:type:ShaderForge.SFN_OneMinus,id:2532,x:31349,y:32776,varname:node_2532,prsc:2|IN-5854-OUT;n:type:ShaderForge.SFN_Multiply,id:7761,x:31580,y:32889,varname:node_7761,prsc:2|A-2532-OUT,B-5764-OUT;n:type:ShaderForge.SFN_Vector1,id:5764,x:31349,y:32923,varname:node_5764,prsc:2,v1:5;n:type:ShaderForge.SFN_OneMinus,id:8628,x:31953,y:32819,varname:node_8628,prsc:2|IN-3495-OUT;n:type:ShaderForge.SFN_Blend,id:150,x:32063,y:33166,varname:node_150,prsc:2,blmd:1,clmp:True|SRC-5700-OUT,DST-3495-OUT;n:type:ShaderForge.SFN_Multiply,id:4126,x:32549,y:33050,varname:node_4126,prsc:2|A-794-OUT,B-150-OUT;n:type:ShaderForge.SFN_Clamp01,id:5700,x:31752,y:32775,varname:node_5700,prsc:2|IN-7761-OUT;n:type:ShaderForge.SFN_Posterize,id:550,x:32301,y:32715,varname:node_550,prsc:2|IN-1458-OUT,STPS-5894-OUT;n:type:ShaderForge.SFN_Vector1,id:5894,x:32102,y:33017,varname:node_5894,prsc:2,v1:5;n:type:ShaderForge.SFN_Abs,id:1458,x:32109,y:32662,varname:node_1458,prsc:2|IN-8628-OUT;n:type:ShaderForge.SFN_TexCoord,id:674,x:31450,y:33914,varname:node_674,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1750,x:31987,y:33985,varname:node_1750,prsc:2|A-472-OUT,B-2388-OUT;n:type:ShaderForge.SFN_Vector1,id:472,x:31668,y:33944,varname:node_472,prsc:2,v1:5;n:type:ShaderForge.SFN_Add,id:2388,x:31683,y:34050,varname:node_2388,prsc:2|A-674-V,B-4191-OUT;n:type:ShaderForge.SFN_Vector1,id:6354,x:31263,y:34263,varname:node_6354,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_OneMinus,id:9605,x:32260,y:34001,varname:node_9605,prsc:2|IN-1750-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7495,x:31263,y:34120,ptovrint:False,ptlb:Pan_Up,ptin:_Pan_Up,varname:_Pan_Up,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Negate,id:4191,x:31461,y:34108,varname:node_4191,prsc:2|IN-7495-OUT;n:type:ShaderForge.SFN_Multiply,id:9835,x:32642,y:33855,varname:node_9835,prsc:2|A-3495-OUT,B-9605-OUT;n:type:ShaderForge.SFN_Clamp01,id:3852,x:32282,y:34158,varname:node_3852,prsc:2|IN-1750-OUT;n:type:ShaderForge.SFN_Vector1,id:1635,x:31987,y:34238,varname:node_1635,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:6076,x:33361,y:33607,varname:node_6076,prsc:2|A-4126-OUT,B-5248-OUT;n:type:ShaderForge.SFN_Multiply,id:1953,x:33332,y:32984,varname:node_1953,prsc:2|A-5935-OUT,B-5248-OUT;n:type:ShaderForge.SFN_Clamp01,id:5248,x:32825,y:33812,varname:node_5248,prsc:2|IN-5312-OUT;n:type:ShaderForge.SFN_Blend,id:5312,x:32417,y:33819,varname:node_5312,prsc:2,blmd:14,clmp:True|SRC-3495-OUT,DST-9605-OUT;n:type:ShaderForge.SFN_Multiply,id:2972,x:33304,y:33358,varname:node_2972,prsc:2|A-5474-OUT,B-5248-OUT;n:type:ShaderForge.SFN_DepthBlend,id:1156,x:33339,y:33926,varname:node_1156,prsc:2|DIST-6937-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6937,x:33067,y:33983,ptovrint:False,ptlb:Depth_Blend,ptin:_Depth_Blend,varname:_Depth_Blend,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5023,x:33588,y:33785,varname:node_5023,prsc:2|A-6076-OUT,B-1156-OUT;n:type:ShaderForge.SFN_Multiply,id:5508,x:33642,y:33545,varname:node_5508,prsc:2|A-2972-OUT,B-1156-OUT;proporder:5438-1326-9130-794-1827-9211-4162-2122-4465-7495-6937;pass:END;sub:END;*/

Shader "Blockout/Gate_Partile" {
    Properties {
        _Effects_Texture ("Effects_Texture", 2D) = "white" {}
        [HDR]_Color_2 ("Color_2", Color) = (0.3014706,0.3014706,0.3014706,1)
        [HDR]_Color_1 ("Color_1", Color) = (0.9117647,0.9117647,0.9117647,1)
        _Alpha ("Alpha", Float ) = 0.6
        _Spinner_4 ("Spinner_4", Float ) = 0.6
        _Spinner_3 ("Spinner_3", Float ) = 1
        _Spinner_2 ("Spinner_2", Float ) = 0.8
        _Spinner_1 ("Spinner_1", Float ) = 1
        _Wilbblyocity ("Wilbblyocity", Float ) = 1
        _Pan_Up ("Pan_Up", Float ) = 0.5
        _Depth_Blend ("Depth_Blend", Float ) = 1
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _Effects_Texture; uniform float4 _Effects_Texture_ST;
            uniform float4 _Color_1;
            uniform float4 _Color_2;
            uniform float _Alpha;
            uniform float _Spinner_1;
            uniform float _Spinner_2;
            uniform float _Spinner_3;
            uniform float _Spinner_4;
            uniform float _Wilbblyocity;
            uniform float _Pan_Up;
            uniform float _Depth_Blend;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 projPos : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_2887 = _Time + _TimeEditor;
                float node_5541_ang = (_Spinner_1*node_2887.r);
                float node_5541_spd = 1.0;
                float node_5541_cos = cos(node_5541_spd*node_5541_ang);
                float node_5541_sin = sin(node_5541_spd*node_5541_ang);
                float2 node_5541_piv = float2(0.5,0.5);
                float2 node_5541 = (mul(o.uv0-node_5541_piv,float2x2( node_5541_cos, -node_5541_sin, node_5541_sin, node_5541_cos))+node_5541_piv);
                float4 _node_3505 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_5541, _Effects_Texture),0.0,0));
                float node_9898_ang = (_Spinner_2*node_2887.r);
                float node_9898_spd = 1.0;
                float node_9898_cos = cos(node_9898_spd*node_9898_ang);
                float node_9898_sin = sin(node_9898_spd*node_9898_ang);
                float2 node_9898_piv = float2(0.5,0.5);
                float2 node_9898 = (mul(o.uv0-node_9898_piv,float2x2( node_9898_cos, -node_9898_sin, node_9898_sin, node_9898_cos))+node_9898_piv);
                float4 _node_9646 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_9898, _Effects_Texture),0.0,0));
                float node_680_ang = ((-1*_Spinner_3)*node_2887.r);
                float node_680_spd = 1.0;
                float node_680_cos = cos(node_680_spd*node_680_ang);
                float node_680_sin = sin(node_680_spd*node_680_ang);
                float2 node_680_piv = float2(0.5,0.5);
                float2 node_680 = (mul(o.uv0-node_680_piv,float2x2( node_680_cos, -node_680_sin, node_680_sin, node_680_cos))+node_680_piv);
                float4 _node_7015 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_680, _Effects_Texture),0.0,0));
                float node_855_ang = ((-1*_Spinner_4)*node_2887.r);
                float node_855_spd = 1.0;
                float node_855_cos = cos(node_855_spd*node_855_ang);
                float node_855_sin = sin(node_855_spd*node_855_ang);
                float2 node_855_piv = float2(0.5,0.5);
                float2 node_855 = (mul(o.uv0-node_855_piv,float2x2( node_855_cos, -node_855_sin, node_855_sin, node_855_cos))+node_855_piv);
                float4 _node_569 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_855, _Effects_Texture),0.0,0));
                float node_3495 = saturate((0.5 - 2.0*(saturate(( _node_9646.g > 0.5 ? (1.0-(1.0-2.0*(_node_9646.g-0.5))*(1.0-_node_3505.r)) : (2.0*_node_9646.g*_node_3505.r) ))-0.5)*(saturate(abs(_node_7015.r-_node_569.g))-0.5)));
                v.vertex.xyz += (_Wilbblyocity*(node_3495*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float node_5700 = saturate(((1.0 - length((i.uv0*2.0+-1.0)))*5.0));
                float4 node_2887 = _Time + _TimeEditor;
                float node_5541_ang = (_Spinner_1*node_2887.r);
                float node_5541_spd = 1.0;
                float node_5541_cos = cos(node_5541_spd*node_5541_ang);
                float node_5541_sin = sin(node_5541_spd*node_5541_ang);
                float2 node_5541_piv = float2(0.5,0.5);
                float2 node_5541 = (mul(i.uv0-node_5541_piv,float2x2( node_5541_cos, -node_5541_sin, node_5541_sin, node_5541_cos))+node_5541_piv);
                float4 _node_3505 = tex2D(_Effects_Texture,TRANSFORM_TEX(node_5541, _Effects_Texture));
                float node_9898_ang = (_Spinner_2*node_2887.r);
                float node_9898_spd = 1.0;
                float node_9898_cos = cos(node_9898_spd*node_9898_ang);
                float node_9898_sin = sin(node_9898_spd*node_9898_ang);
                float2 node_9898_piv = float2(0.5,0.5);
                float2 node_9898 = (mul(i.uv0-node_9898_piv,float2x2( node_9898_cos, -node_9898_sin, node_9898_sin, node_9898_cos))+node_9898_piv);
                float4 _node_9646 = tex2D(_Effects_Texture,TRANSFORM_TEX(node_9898, _Effects_Texture));
                float node_680_ang = ((-1*_Spinner_3)*node_2887.r);
                float node_680_spd = 1.0;
                float node_680_cos = cos(node_680_spd*node_680_ang);
                float node_680_sin = sin(node_680_spd*node_680_ang);
                float2 node_680_piv = float2(0.5,0.5);
                float2 node_680 = (mul(i.uv0-node_680_piv,float2x2( node_680_cos, -node_680_sin, node_680_sin, node_680_cos))+node_680_piv);
                float4 _node_7015 = tex2D(_Effects_Texture,TRANSFORM_TEX(node_680, _Effects_Texture));
                float node_855_ang = ((-1*_Spinner_4)*node_2887.r);
                float node_855_spd = 1.0;
                float node_855_cos = cos(node_855_spd*node_855_ang);
                float node_855_sin = sin(node_855_spd*node_855_ang);
                float2 node_855_piv = float2(0.5,0.5);
                float2 node_855 = (mul(i.uv0-node_855_piv,float2x2( node_855_cos, -node_855_sin, node_855_sin, node_855_cos))+node_855_piv);
                float4 _node_569 = tex2D(_Effects_Texture,TRANSFORM_TEX(node_855, _Effects_Texture));
                float node_3495 = saturate((0.5 - 2.0*(saturate(( _node_9646.g > 0.5 ? (1.0-(1.0-2.0*(_node_9646.g-0.5))*(1.0-_node_3505.r)) : (2.0*_node_9646.g*_node_3505.r) ))-0.5)*(saturate(abs(_node_7015.r-_node_569.g))-0.5)));
                float node_150 = saturate((node_5700*node_3495));
                float node_1750 = (5.0*(i.uv0.g+(-1*_Pan_Up)));
                float node_9605 = (1.0 - node_1750);
                float node_5248 = saturate(saturate(( node_3495 > 0.5 ? (node_9605 + 2.0*node_3495 -1.0) : (node_9605 + 2.0*(node_3495-0.5)))));
                float node_1156 = saturate((sceneZ-partZ)/_Depth_Blend);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (((float2(node_150,node_150)*0.1)*node_5248)*node_1156);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float node_5894 = 5.0;
                float3 node_5935 = lerp(_Color_1.rgb,_Color_2.rgb,floor(abs((1.0 - node_3495)) * node_5894) / (node_5894 - 1));
                float3 emissive = (node_5935*node_5248);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(((_Alpha*node_150)*node_5248)*node_1156)),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Effects_Texture; uniform float4 _Effects_Texture_ST;
            uniform float _Spinner_1;
            uniform float _Spinner_2;
            uniform float _Spinner_3;
            uniform float _Spinner_4;
            uniform float _Wilbblyocity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_2887 = _Time + _TimeEditor;
                float node_5541_ang = (_Spinner_1*node_2887.r);
                float node_5541_spd = 1.0;
                float node_5541_cos = cos(node_5541_spd*node_5541_ang);
                float node_5541_sin = sin(node_5541_spd*node_5541_ang);
                float2 node_5541_piv = float2(0.5,0.5);
                float2 node_5541 = (mul(o.uv0-node_5541_piv,float2x2( node_5541_cos, -node_5541_sin, node_5541_sin, node_5541_cos))+node_5541_piv);
                float4 _node_3505 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_5541, _Effects_Texture),0.0,0));
                float node_9898_ang = (_Spinner_2*node_2887.r);
                float node_9898_spd = 1.0;
                float node_9898_cos = cos(node_9898_spd*node_9898_ang);
                float node_9898_sin = sin(node_9898_spd*node_9898_ang);
                float2 node_9898_piv = float2(0.5,0.5);
                float2 node_9898 = (mul(o.uv0-node_9898_piv,float2x2( node_9898_cos, -node_9898_sin, node_9898_sin, node_9898_cos))+node_9898_piv);
                float4 _node_9646 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_9898, _Effects_Texture),0.0,0));
                float node_680_ang = ((-1*_Spinner_3)*node_2887.r);
                float node_680_spd = 1.0;
                float node_680_cos = cos(node_680_spd*node_680_ang);
                float node_680_sin = sin(node_680_spd*node_680_ang);
                float2 node_680_piv = float2(0.5,0.5);
                float2 node_680 = (mul(o.uv0-node_680_piv,float2x2( node_680_cos, -node_680_sin, node_680_sin, node_680_cos))+node_680_piv);
                float4 _node_7015 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_680, _Effects_Texture),0.0,0));
                float node_855_ang = ((-1*_Spinner_4)*node_2887.r);
                float node_855_spd = 1.0;
                float node_855_cos = cos(node_855_spd*node_855_ang);
                float node_855_sin = sin(node_855_spd*node_855_ang);
                float2 node_855_piv = float2(0.5,0.5);
                float2 node_855 = (mul(o.uv0-node_855_piv,float2x2( node_855_cos, -node_855_sin, node_855_sin, node_855_cos))+node_855_piv);
                float4 _node_569 = tex2Dlod(_Effects_Texture,float4(TRANSFORM_TEX(node_855, _Effects_Texture),0.0,0));
                float node_3495 = saturate((0.5 - 2.0*(saturate(( _node_9646.g > 0.5 ? (1.0-(1.0-2.0*(_node_9646.g-0.5))*(1.0-_node_3505.r)) : (2.0*_node_9646.g*_node_3505.r) ))-0.5)*(saturate(abs(_node_7015.r-_node_569.g))-0.5)));
                v.vertex.xyz += (_Wilbblyocity*(node_3495*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
