Shader "Glia/EyeUI"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "black" {}
        _PupilColor ("Pupil Color", Color) = (0,0,0,1)
        _IrisColorMain ("Iris Main Color", Color) = (0,0,0,1)
        _IrisColorDetail ("Iris Detail Color", Color) = (0,0,0,1)        
        _SkinColor ("Skin Color", Color) = (0.59,0.36, 0.3, 1)
        _EyePosX ("Eye position X", Range(-1,1)) = 0
        _EyePosY ("Eye position Y", Range(-1,1)) = 0
        _PupilSize ("Pupil size", Range(0.2,1)) = 0
        _Openess ("Openess", Range(0,1)) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            half _EyePosX;
            half _EyePosY;
            half _PupilSize;
            half _Openess;
            fixed4 _PupilColor;
            fixed4 _IrisColorMain;
            fixed4 _IrisColorDetail;
            fixed4 _SkinColor;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX( float2(v.texcoord.x, 1 - v.texcoord.y), _MainTex);

                OUT.color = v.color;
                return OUT;
            }

            void eye(inout float4 c,in float2 coord,in float2 pos, in float size)
            {
                float4 e=c;
                
                float2 iris = pos + float2(_EyePosX/2.5, _EyePosY/4);
                
                float2 almond = coord;
                            
                almond=float2(almond.x,almond.y-sign(0.5-almond.y)*size*0.9);
                coord-=(float2(0.5,0.5)-coord)*0.6*size;
                
                float2 rad=coord-iris;
                // e to Iris color
                e = lerp(_IrisColorMain, _IrisColorDetail, (coord.y-iris.y)/size);

                // Add Iris inperfections
                e *= (fmod(floor(atan2(rad.y,rad.x)*10.0),2.0)==0.0) ? 1 : 0.95;

                // e to pupil or iris depending coord
                e = length(coord-iris) > (size/3.0)*_PupilSize ? e : _PupilColor;
                
                // e to pupil/iris or sclera depending coords
                e = length(coord-iris) > (size/1.5) ? float4(1.0,251.0/255.0,237.0/255.0, 1.0) : e;

                // e to eye or skin
                e = length(pos-coord) < size ? e : _SkinColor;
                
                e = abs(pos.y-coord.y) < (size * _Openess ) - 0.01 ? e : float4(0,0,0,1);
                
                e = abs(pos.y-coord.y) < (size * _Openess) + 0.01 ? e : _SkinColor;

                //Transparent Bottom part
                e.a = length(almond-pos)<size*1.5 ? 1.0 : 0.0;
                //Transparent Top part
                //e.a *=  (length(almond-pos)>size*1.4 && almond.y>=pos.y) ? 1 : 1;
                c= e;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = fixed4(0,0,0,0);
            
                eye(c,float2(IN.texcoord.x, IN.texcoord.y),float2(0.5 ,0.5), 0.35);            
            

                #ifdef UNITY_UI_CLIP_RECT
                c.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (c.a - 0.001);
                #endif

                return c;
            }
        ENDCG
        }
    }
}
