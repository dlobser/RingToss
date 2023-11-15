Shader "Unlit/DepthNoiseBlobSymmetry"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BGTex ("BG Texture", 2D) = "white" {}
        _Data("Data",vector) = (0,0,0,0)
        _CamPos("CamPos", vector) = (0,0,0,0)
        _Pow ("Pow", float) = 0
        _DepthOrColor("DepthOrColor", float) = 0
        _DepthOffset ("Depth Offset", float) = 0
        _DepthOffsetSpeed ("Depth Offset Speed", float) = 0
        _Clip ("Clip", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"

            uniform float4x4 _ProjectionMatrix; // Projection matrix of the camera acting as a projector
            uniform float4x4 _WorldToCameraMatrix; // world to camera (acting as a projector) matrix

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 pos : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 pos : COLOR;
                float4 uv2 : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BGTex;
            float4 _Data;
            float4 _CamPos;
            float _Pow;
            float _DepthOrColor;
            float _DepthOffset;
            float _DepthOffsetSpeed;
            float _Clip;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);

                float4x4 m = mul(_ProjectionMatrix, _WorldToCameraMatrix); //
                o.uv2 =  mul(_ProjectionMatrix, o.pos); // clipped position in projector space


                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float l = length(i.uv-.5);
                float n1 = ((snoise(float3(abs(i.uv.x-.5)*7*_CamPos.z,abs(i.uv.y-.5)*7*_CamPos.w,_DepthOffset+i.pos.z+_DepthOffsetSpeed*_Time.y)))*2*l);
                float n2 = ((snoise(float3(abs(i.uv.x-.5)*2*_CamPos.z,abs(i.uv.y-.5)*_CamPos.w*2-n1,_DepthOffset+i.pos.z))+7)*.1);
                float b = saturate((1-l*2)*2)* (n1+n2);
                clip(b-.3-_Clip);
                float4 tex = tex2D(_MainTex,( i.uv2.xy/(i.uv2.w))*.5-float2(.5,.5));////+parallaxOffset);
                float4 texBG = tex2D(_BGTex,( i.uv2.xy/(i.uv2.w))*.5-float2(.5,.5));
                tex = lerp(texBG,tex,saturate(i.pos.y*_CamPos.x+_CamPos.y));
                float depth = pow(b,.4)*.15+pow((length(i.pos-_WorldSpaceCameraPos)+_Data.x)*_Data.y,_Pow)+_Data.w;
                float4 outCol = lerp(tex,depth,_DepthOrColor);
                return outCol;
            }
            ENDCG
        }
    }
}
