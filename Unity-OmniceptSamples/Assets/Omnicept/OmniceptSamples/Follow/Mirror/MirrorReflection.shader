// original source from: http://wiki.unity3d.com/index.php/MirrorReflection4
Shader "FX/MirrorReflection"
{
    Properties
    {
        _ReflectionTexLeft ("_ReflectionTexLeft", 2D) = "white" {}
        _ReflectionTexRight ("_ReflectionTexRight", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float4  uv : TEXCOORD0;
                float4 pos : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID 
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv =  ComputeScreenPos (o.pos);
                return o;
            }
            sampler2D _MainTex;
            sampler2D _ReflectionTexLeft;
            sampler2D _ReflectionTexRight;

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                fixed4 refl;
                if (unity_StereoEyeIndex > 0){
                    refl = tex2Dproj(_ReflectionTexRight, UNITY_PROJ_COORD(i.uv));
                }
                else{
                    refl = tex2Dproj(_ReflectionTexLeft, UNITY_PROJ_COORD(i.uv));
                }
                
                return refl;
            }
            ENDCG
        }
    }
}