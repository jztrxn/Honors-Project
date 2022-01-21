Shader "Glia/Eye UV"
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
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Ph ysically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf NoLighting fullforwardshadows alpha:auto

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _EyePosX;
        half _EyePosY;
        half _PupilSize;
        half _Openess;
        fixed4 _PupilColor;
        fixed4 _IrisColorMain;
        fixed4 _IrisColorDetail;
        fixed4 _SkinColor;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        //Unlit we will move this to normal (not surface) shader later
        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo; 
            c.a = s.Alpha;
            return c;
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

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = fixed4(0,0,0,0);
            
            eye(c,float2(IN.uv_MainTex.x, IN.uv_MainTex.y),float2(0.5 ,0.5), 0.35);            
            
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }



        ENDCG
    }
    FallBack "Diffuse"
}
