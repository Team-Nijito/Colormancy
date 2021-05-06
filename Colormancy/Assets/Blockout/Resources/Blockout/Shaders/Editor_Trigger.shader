// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:180,x:33685,y:33499,varname:node_180,prsc:2|emission-4573-OUT;n:type:ShaderForge.SFN_Tex2d,id:5908,x:30100,y:33186,varname:_node_5908,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-6229-OUT,TEX-129-TEX;n:type:ShaderForge.SFN_FragmentPosition,id:8936,x:26966,y:34710,varname:node_8936,prsc:2;n:type:ShaderForge.SFN_Dot,id:9510,x:30928,y:31974,varname:node_9510,prsc:2,dt:3|A-3886-OUT,B-4016-OUT;n:type:ShaderForge.SFN_NormalVector,id:3886,x:30660,y:31904,prsc:2,pt:False;n:type:ShaderForge.SFN_Vector3,id:4016,x:30660,y:32083,varname:node_4016,prsc:2,v1:1,v2:0,v3:0;n:type:ShaderForge.SFN_Vector3,id:4211,x:30660,y:32202,varname:node_4211,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Vector3,id:5871,x:30660,y:32332,varname:node_5871,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Dot,id:2338,x:30928,y:32173,varname:node_2338,prsc:2,dt:3|A-3886-OUT,B-4211-OUT;n:type:ShaderForge.SFN_Dot,id:6124,x:30928,y:32382,varname:node_6124,prsc:2,dt:3|A-3886-OUT,B-5871-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:129,x:27182,y:35189,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:_Tex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:3118,x:28735,y:35224,varname:node_3118,prsc:2|A-8936-X,B-8936-Z;n:type:ShaderForge.SFN_Multiply,id:7514,x:32880,y:33500,varname:node_7514,prsc:2|A-309-RGB,B-1093-OUT;n:type:ShaderForge.SFN_Color,id:309,x:32491,y:33110,ptovrint:False,ptlb:Color_1,ptin:_Color_1,varname:_Color_1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3490196,c2:0.3686275,c3:0.627451,c4:1;n:type:ShaderForge.SFN_Multiply,id:9265,x:33079,y:33461,varname:node_9265,prsc:2|A-6243-OUT,B-7514-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6243,x:32865,y:33360,ptovrint:False,ptlb:Emissive_Brightness,ptin:_Emissive_Brightness,varname:_Emissive_Brightness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:1757,x:32248,y:33692,varname:node_1757,prsc:2|A-3724-OUT,B-5864-OUT;n:type:ShaderForge.SFN_DepthBlend,id:1576,x:33018,y:34148,varname:node_1576,prsc:2|DIST-2683-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2683,x:32835,y:34148,ptovrint:False,ptlb:Depth_Blend,ptin:_Depth_Blend,varname:_Depth_Blend,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4573,x:33239,y:33701,varname:node_4573,prsc:2|A-9265-OUT,B-1576-OUT;n:type:ShaderForge.SFN_Append,id:5039,x:28704,y:36900,varname:node_5039,prsc:2|A-8936-X,B-8936-Y;n:type:ShaderForge.SFN_Append,id:6229,x:28885,y:33201,varname:node_6229,prsc:2|A-8936-Y,B-8936-Z;n:type:ShaderForge.SFN_Add,id:1093,x:32657,y:33613,varname:node_1093,prsc:2|A-9294-OUT,B-4245-OUT;n:type:ShaderForge.SFN_Vector1,id:9549,x:32168,y:33401,varname:node_9549,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Tex2d,id:8435,x:30077,y:33934,varname:_node_5348,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-2934-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Multiply,id:3045,x:30789,y:34125,varname:node_3045,prsc:2|A-583-OUT,B-6445-OUT;n:type:ShaderForge.SFN_Vector1,id:583,x:30585,y:34072,varname:node_583,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Add,id:6342,x:31046,y:33780,varname:node_6342,prsc:2|A-3140-OUT,B-3045-OUT;n:type:ShaderForge.SFN_Clamp01,id:5864,x:31231,y:33780,varname:node_5864,prsc:2|IN-6342-OUT;n:type:ShaderForge.SFN_Tex2d,id:7023,x:30077,y:34144,varname:_node_7023,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-6456-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:8738,x:30077,y:34376,varname:_node_8738,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-2725-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:7352,x:30077,y:34609,varname:_node_7352,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-5812-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Panner,id:2934,x:29710,y:33804,varname:node_2934,prsc:2,spu:-0.1,spv:0|UVIN-1528-OUT,DIST-9026-OUT;n:type:ShaderForge.SFN_Panner,id:6456,x:29718,y:34063,varname:node_6456,prsc:2,spu:0.1,spv:0|UVIN-6229-OUT,DIST-9026-OUT;n:type:ShaderForge.SFN_Blend,id:9822,x:30304,y:34010,varname:node_9822,prsc:2,blmd:16,clmp:True|SRC-8435-G,DST-7023-G;n:type:ShaderForge.SFN_Multiply,id:1528,x:29427,y:33977,varname:node_1528,prsc:2|A-6229-OUT,B-5087-OUT;n:type:ShaderForge.SFN_Vector1,id:5087,x:29194,y:34051,varname:node_5087,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Blend,id:2135,x:30313,y:34474,varname:node_2135,prsc:2,blmd:16,clmp:True|SRC-8738-G,DST-7352-G;n:type:ShaderForge.SFN_Panner,id:2725,x:29720,y:34323,varname:node_2725,prsc:2,spu:0,spv:-0.1|UVIN-5839-OUT,DIST-9026-OUT;n:type:ShaderForge.SFN_Panner,id:5812,x:29720,y:34538,varname:node_5812,prsc:2,spu:0,spv:0.1|UVIN-217-OUT,DIST-9026-OUT;n:type:ShaderForge.SFN_Blend,id:6445,x:30585,y:34210,varname:node_6445,prsc:2,blmd:6,clmp:True|SRC-9822-OUT,DST-2135-OUT;n:type:ShaderForge.SFN_Time,id:5665,x:26966,y:34856,varname:node_5665,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1079,x:27182,y:34970,varname:node_1079,prsc:2|A-5665-T,B-1467-OUT;n:type:ShaderForge.SFN_Vector1,id:1467,x:26966,y:35062,varname:node_1467,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Multiply,id:2816,x:32248,y:33848,varname:node_2816,prsc:2|A-455-OUT,B-9861-OUT;n:type:ShaderForge.SFN_Multiply,id:961,x:32248,y:34008,varname:node_961,prsc:2|A-2000-OUT,B-3055-OUT;n:type:ShaderForge.SFN_Multiply,id:5839,x:29427,y:34177,varname:node_5839,prsc:2|A-6229-OUT,B-5230-OUT;n:type:ShaderForge.SFN_Vector1,id:5230,x:29194,y:34188,varname:node_5230,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Multiply,id:217,x:29427,y:34340,varname:node_217,prsc:2|A-6229-OUT,B-8438-OUT;n:type:ShaderForge.SFN_Vector1,id:8438,x:29194,y:34398,varname:node_8438,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Tex2d,id:6981,x:30130,y:35147,varname:_node_1198,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-4073-OUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:1226,x:30122,y:35571,varname:_node_9240,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-552-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Multiply,id:4065,x:30834,y:35762,varname:node_4065,prsc:2|A-2398-OUT,B-1593-OUT;n:type:ShaderForge.SFN_Vector1,id:2398,x:30630,y:35709,varname:node_2398,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Add,id:1295,x:31091,y:35417,varname:node_1295,prsc:2|A-3592-OUT,B-4065-OUT;n:type:ShaderForge.SFN_Clamp01,id:9861,x:31276,y:35417,varname:node_9861,prsc:2|IN-1295-OUT;n:type:ShaderForge.SFN_Tex2d,id:8552,x:30122,y:35781,varname:_node_3291,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-1634-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:6094,x:30122,y:36013,varname:_node_2831,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-2312-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:6022,x:30122,y:36246,varname:_node_8346,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-751-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Panner,id:552,x:29755,y:35441,varname:node_552,prsc:2,spu:-0.1,spv:0|UVIN-8701-OUT,DIST-3369-OUT;n:type:ShaderForge.SFN_Panner,id:1634,x:29763,y:35700,varname:node_1634,prsc:2,spu:0.1,spv:0|UVIN-3118-OUT,DIST-3369-OUT;n:type:ShaderForge.SFN_Blend,id:7503,x:30349,y:35647,varname:node_7503,prsc:2,blmd:16,clmp:True|SRC-1226-G,DST-8552-G;n:type:ShaderForge.SFN_Multiply,id:8701,x:29472,y:35614,varname:node_8701,prsc:2|A-3118-OUT,B-8336-OUT;n:type:ShaderForge.SFN_Vector1,id:8336,x:29239,y:35688,varname:node_8336,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Blend,id:542,x:30358,y:36111,varname:node_542,prsc:2,blmd:16,clmp:True|SRC-6094-G,DST-6022-G;n:type:ShaderForge.SFN_Panner,id:2312,x:29765,y:35960,varname:node_2312,prsc:2,spu:0,spv:-0.1|UVIN-3076-OUT,DIST-3369-OUT;n:type:ShaderForge.SFN_Panner,id:751,x:29765,y:36204,varname:node_751,prsc:2,spu:0,spv:0.1|UVIN-6598-OUT,DIST-3369-OUT;n:type:ShaderForge.SFN_Blend,id:1593,x:30630,y:35847,varname:node_1593,prsc:2,blmd:6,clmp:True|SRC-7503-OUT,DST-542-OUT;n:type:ShaderForge.SFN_Multiply,id:3076,x:29472,y:35814,varname:node_3076,prsc:2|A-3118-OUT,B-4343-OUT;n:type:ShaderForge.SFN_Vector1,id:4343,x:29239,y:35825,varname:node_4343,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Multiply,id:6598,x:29472,y:35977,varname:node_6598,prsc:2|A-3118-OUT,B-3088-OUT;n:type:ShaderForge.SFN_Vector1,id:3088,x:29239,y:36035,varname:node_3088,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Relay,id:3369,x:28765,y:35396,varname:node_3369,prsc:2|IN-1079-OUT;n:type:ShaderForge.SFN_Relay,id:9026,x:28915,y:33386,varname:node_9026,prsc:2|IN-1079-OUT;n:type:ShaderForge.SFN_Tex2d,id:1845,x:30086,y:36580,varname:_node_1198_copy,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-9389-OUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:4089,x:30078,y:37004,varname:_node_2892,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-3056-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Multiply,id:6100,x:30790,y:37195,varname:node_6100,prsc:2|A-3230-OUT,B-2041-OUT;n:type:ShaderForge.SFN_Vector1,id:3230,x:30586,y:37142,varname:node_3230,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Add,id:3925,x:31047,y:36850,varname:node_3925,prsc:2|A-1040-OUT,B-6100-OUT;n:type:ShaderForge.SFN_Clamp01,id:3055,x:31232,y:36850,varname:node_3055,prsc:2|IN-3925-OUT;n:type:ShaderForge.SFN_Tex2d,id:1627,x:30078,y:37214,varname:_node_3292,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-3606-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:1283,x:30078,y:37446,varname:_node_838,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-8179-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Tex2d,id:3091,x:30078,y:37679,varname:_node_1917,prsc:2,tex:cfd3ef0affa5a984d948919178dc441a,ntxv:0,isnm:False|UVIN-4775-UVOUT,TEX-129-TEX;n:type:ShaderForge.SFN_Panner,id:3056,x:29711,y:36874,varname:node_3056,prsc:2,spu:-0.1,spv:0|UVIN-2229-OUT,DIST-999-OUT;n:type:ShaderForge.SFN_Panner,id:3606,x:29719,y:37133,varname:node_3606,prsc:2,spu:0.1,spv:0|UVIN-5039-OUT,DIST-999-OUT;n:type:ShaderForge.SFN_Blend,id:780,x:30305,y:37080,varname:node_780,prsc:2,blmd:16,clmp:True|SRC-4089-G,DST-1627-G;n:type:ShaderForge.SFN_Multiply,id:2229,x:29428,y:37047,varname:node_2229,prsc:2|A-5039-OUT,B-3551-OUT;n:type:ShaderForge.SFN_Vector1,id:3551,x:29195,y:37121,varname:node_3551,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Blend,id:7030,x:30314,y:37544,varname:node_7030,prsc:2,blmd:16,clmp:True|SRC-1283-G,DST-3091-G;n:type:ShaderForge.SFN_Panner,id:8179,x:29721,y:37393,varname:node_8179,prsc:2,spu:0,spv:-0.1|UVIN-7328-OUT,DIST-999-OUT;n:type:ShaderForge.SFN_Panner,id:4775,x:29721,y:37637,varname:node_4775,prsc:2,spu:0,spv:0.1|UVIN-7808-OUT,DIST-999-OUT;n:type:ShaderForge.SFN_Blend,id:2041,x:30586,y:37280,varname:node_2041,prsc:2,blmd:6,clmp:True|SRC-780-OUT,DST-7030-OUT;n:type:ShaderForge.SFN_Multiply,id:7328,x:29428,y:37247,varname:node_7328,prsc:2|A-5039-OUT,B-9113-OUT;n:type:ShaderForge.SFN_Vector1,id:9113,x:29195,y:37258,varname:node_9113,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Multiply,id:7808,x:29428,y:37410,varname:node_7808,prsc:2|A-5039-OUT,B-7426-OUT;n:type:ShaderForge.SFN_Vector1,id:7426,x:29195,y:37468,varname:node_7426,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Relay,id:999,x:28734,y:37155,varname:node_999,prsc:2|IN-1079-OUT;n:type:ShaderForge.SFN_Add,id:4245,x:32473,y:33848,varname:node_4245,prsc:2|A-1757-OUT,B-2816-OUT,C-961-OUT;n:type:ShaderForge.SFN_Power,id:3724,x:31244,y:32098,varname:node_3724,prsc:2|VAL-9510-OUT,EXP-8855-OUT;n:type:ShaderForge.SFN_Power,id:455,x:31244,y:32278,varname:node_455,prsc:2|VAL-2338-OUT,EXP-8855-OUT;n:type:ShaderForge.SFN_Power,id:2000,x:31260,y:32490,varname:node_2000,prsc:2|VAL-6124-OUT,EXP-8855-OUT;n:type:ShaderForge.SFN_Vector1,id:8855,x:30905,y:32676,varname:node_8855,prsc:2,v1:5;n:type:ShaderForge.SFN_Vector1,id:5941,x:30969,y:32740,varname:node_5941,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:5570,x:30337,y:33390,varname:node_5570,prsc:2|A-5908-B,B-5327-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3538,x:27182,y:35428,ptovrint:False,ptlb:Extra_Lines,ptin:_Extra_Lines,varname:_Extra_Lines,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.5;n:type:ShaderForge.SFN_Add,id:3140,x:30560,y:33369,varname:node_3140,prsc:2|A-5908-R,B-5570-OUT;n:type:ShaderForge.SFN_Relay,id:5327,x:28915,y:33529,varname:node_5327,prsc:2|IN-3538-OUT;n:type:ShaderForge.SFN_Multiply,id:9133,x:30411,y:35284,varname:node_9133,prsc:2|A-6981-B,B-5657-OUT;n:type:ShaderForge.SFN_Add,id:3592,x:30634,y:35263,varname:node_3592,prsc:2|A-6981-R,B-9133-OUT;n:type:ShaderForge.SFN_Multiply,id:7024,x:30295,y:36716,varname:node_7024,prsc:2|A-1845-B,B-8972-OUT;n:type:ShaderForge.SFN_Add,id:1040,x:30518,y:36695,varname:node_1040,prsc:2|A-1845-R,B-7024-OUT;n:type:ShaderForge.SFN_Relay,id:5657,x:28765,y:35504,varname:node_5657,prsc:2|IN-3538-OUT;n:type:ShaderForge.SFN_Relay,id:8972,x:28746,y:37312,varname:node_8972,prsc:2|IN-3538-OUT;n:type:ShaderForge.SFN_Add,id:9389,x:29590,y:36678,varname:node_9389,prsc:2|A-5474-OUT,B-5039-OUT;n:type:ShaderForge.SFN_Vector1,id:5474,x:29314,y:36556,varname:node_5474,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Fresnel,id:5084,x:32183,y:33517,varname:node_5084,prsc:2|EXP-6169-OUT;n:type:ShaderForge.SFN_Lerp,id:9294,x:32458,y:33408,varname:node_9294,prsc:2|A-8556-OUT,B-9549-OUT,T-5084-OUT;n:type:ShaderForge.SFN_Vector1,id:8556,x:32168,y:33318,varname:node_8556,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:6169,x:31975,y:33535,varname:node_6169,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:4073,x:29699,y:35097,varname:node_4073,prsc:2|A-7703-OUT,B-3118-OUT;n:type:ShaderForge.SFN_Vector2,id:7703,x:29373,y:35016,varname:node_7703,prsc:2,v1:0.5,v2:0;proporder:129-309-6243-2683-3538;pass:END;sub:END;*/

Shader "Blockout/Editor_Trigger" {
    Properties {
        _Tex ("Tex", 2D) = "white" {}
        _Color_1 ("Color_1", Color) = (0.3490196,0.3686275,0.627451,1)
        _Emissive_Brightness ("Emissive_Brightness", Float ) = 1
        _Depth_Blend ("Depth_Blend", Float ) = 1
        _Extra_Lines ("Extra_Lines", Float ) = 0.5
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
            Blend One One
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
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float4 _Color_1;
            uniform float _Emissive_Brightness;
            uniform float _Depth_Blend;
            uniform float _Extra_Lines;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 projPos : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float node_8855 = 5.0;
                float2 node_6229 = float2(i.posWorld.g,i.posWorld.b);
                float4 _node_5908 = tex2D(_Tex,TRANSFORM_TEX(node_6229, _Tex));
                float4 node_5665 = _Time + _TimeEditor;
                float node_1079 = (node_5665.g*0.8);
                float node_9026 = node_1079;
                float2 node_2934 = ((node_6229*0.5)+node_9026*float2(-0.1,0));
                float4 _node_5348 = tex2D(_Tex,TRANSFORM_TEX(node_2934, _Tex));
                float2 node_6456 = (node_6229+node_9026*float2(0.1,0));
                float4 _node_7023 = tex2D(_Tex,TRANSFORM_TEX(node_6456, _Tex));
                float2 node_2725 = ((node_6229*0.25)+node_9026*float2(0,-0.1));
                float4 _node_8738 = tex2D(_Tex,TRANSFORM_TEX(node_2725, _Tex));
                float2 node_5812 = ((node_6229*0.75)+node_9026*float2(0,0.1));
                float4 _node_7352 = tex2D(_Tex,TRANSFORM_TEX(node_5812, _Tex));
                float2 node_3118 = float2(i.posWorld.r,i.posWorld.b);
                float2 node_4073 = (float2(0.5,0)+node_3118);
                float4 _node_1198 = tex2D(_Tex,TRANSFORM_TEX(node_4073, _Tex));
                float node_3369 = node_1079;
                float2 node_552 = ((node_3118*0.5)+node_3369*float2(-0.1,0));
                float4 _node_9240 = tex2D(_Tex,TRANSFORM_TEX(node_552, _Tex));
                float2 node_1634 = (node_3118+node_3369*float2(0.1,0));
                float4 _node_3291 = tex2D(_Tex,TRANSFORM_TEX(node_1634, _Tex));
                float2 node_2312 = ((node_3118*0.25)+node_3369*float2(0,-0.1));
                float4 _node_2831 = tex2D(_Tex,TRANSFORM_TEX(node_2312, _Tex));
                float2 node_751 = ((node_3118*0.75)+node_3369*float2(0,0.1));
                float4 _node_8346 = tex2D(_Tex,TRANSFORM_TEX(node_751, _Tex));
                float2 node_5039 = float2(i.posWorld.r,i.posWorld.g);
                float2 node_9389 = (0.5+node_5039);
                float4 _node_1198_copy = tex2D(_Tex,TRANSFORM_TEX(node_9389, _Tex));
                float node_999 = node_1079;
                float2 node_3056 = ((node_5039*0.5)+node_999*float2(-0.1,0));
                float4 _node_2892 = tex2D(_Tex,TRANSFORM_TEX(node_3056, _Tex));
                float2 node_3606 = (node_5039+node_999*float2(0.1,0));
                float4 _node_3292 = tex2D(_Tex,TRANSFORM_TEX(node_3606, _Tex));
                float2 node_8179 = ((node_5039*0.25)+node_999*float2(0,-0.1));
                float4 _node_838 = tex2D(_Tex,TRANSFORM_TEX(node_8179, _Tex));
                float2 node_4775 = ((node_5039*0.75)+node_999*float2(0,0.1));
                float4 _node_1917 = tex2D(_Tex,TRANSFORM_TEX(node_4775, _Tex));
                float3 emissive = ((_Emissive_Brightness*(_Color_1.rgb*(lerp(0.1,0.4,pow(1.0-max(0,dot(normalDirection, viewDirection)),2.0))+((pow(abs(dot(i.normalDir,float3(1,0,0))),node_8855)*saturate(((_node_5908.r+(_node_5908.b*_Extra_Lines))+(0.1*saturate((1.0-(1.0-saturate(round( 0.5*(_node_5348.g + _node_7023.g))))*(1.0-saturate(round( 0.5*(_node_8738.g + _node_7352.g))))))))))+(pow(abs(dot(i.normalDir,float3(0,1,0))),node_8855)*saturate(((_node_1198.r+(_node_1198.b*_Extra_Lines))+(0.1*saturate((1.0-(1.0-saturate(round( 0.5*(_node_9240.g + _node_3291.g))))*(1.0-saturate(round( 0.5*(_node_2831.g + _node_8346.g))))))))))+(pow(abs(dot(i.normalDir,float3(0,0,1))),node_8855)*saturate(((_node_1198_copy.r+(_node_1198_copy.b*_Extra_Lines))+(0.1*saturate((1.0-(1.0-saturate(round( 0.5*(_node_2892.g + _node_3292.g))))*(1.0-saturate(round( 0.5*(_node_838.g + _node_1917.g))))))))))))))*saturate((sceneZ-partZ)/_Depth_Blend));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
