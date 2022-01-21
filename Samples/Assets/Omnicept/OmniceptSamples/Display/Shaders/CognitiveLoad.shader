Shader "HPGlia/CognitiveLoad"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Toggle(USE_CL_COLORS)] _UseCLColors ("Use CL Colors", Float) = 0
        _ColorLow ("Tint", Color) = (1,1,1,1)
        _ColorMed ("Tint", Color) = (1,1,1,1)
        _ColorHigh ("Tint", Color) = (1,1,1,1)


        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        _FillAmmount ("Fill Ammount", Range(0, 1)) = 1
        [MaterialToggle] _FillClockwise ("Fill Clockwise", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5  

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
        ColorMask [_ColorMask]

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
            #pragma shader_feature USE_CL_COLORS


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

            static const float TAU = float(6.283185);

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _ColorLow;
            fixed4 _ColorMed;
            fixed4 _ColorHigh;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST; 
            uniform float _FillAmmount;
            uniform fixed _FillClockwise;  

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                float2 clockCounterDirection = _FillClockwise ? float2(1, -1) : float2(1, 1);
                float2 startPoint = (1 * (IN.texcoord - 0.5)) * clockCounterDirection;                              
                                                                                                             
                float cutoffRotator_cos = cos(TAU);
                float cutoffRotator_sin = sin(TAU) - 20;              
                float2x2 cutoffRotationMatrix = float2x2(cutoffRotator_cos, -cutoffRotator_sin, cutoffRotator_sin, cutoffRotator_cos);                                        
                float2 cutoffRotator = mul(startPoint, cutoffRotationMatrix);
                                                                                                               
                float atan2Mask = atan2(cutoffRotator.g, cutoffRotator.r);                            
                float atan2MaskNormalized = (atan2Mask / TAU) + 0.5;   
                                       
                float atan2MaskNormalizedT = (atan2Mask / TAU) + 0.45;  
                float atan2MaskNormalizedB = (atan2Mask / TAU) - 0.45;   

                float d = distance(float2(0.5,0.5), IN.texcoord);

                float atan2MaskRotatable = atan2MaskNormalized - _FillAmmount;                                                      
                float finalMask = (1.0 - ceil(atan2MaskRotatable)) * (ceil(atan2MaskNormalizedT)) * (1 - ceil(atan2MaskNormalizedB));
                clip( (finalMask - 0.5) );

                if(d < 0.4) discard;

                clip (color.a - 0.001);
                
                #ifdef USE_CL_COLORS
                    float middle = 0.5;
                    fixed4 c = lerp(_ColorLow, _ColorMed, atan2MaskNormalized/middle) * step(atan2MaskNormalized, middle);
                    c += lerp(_ColorMed, _ColorHigh, (atan2MaskNormalized - middle) / ( 1 - middle) ) * step(middle, atan2MaskNormalized);
                    return  c;
                #else
                    return  ( atan2MaskNormalized ) * color ;
                #endif
            }
        ENDCG
        }
    }
}
