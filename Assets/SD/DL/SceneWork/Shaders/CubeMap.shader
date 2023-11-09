Shader "Unlit/Cube-Projection-Mix"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _CubeTex ("Cubemap Texture", CUBE) = "" {}
        // _LightData ("Light", vector) = (0,0,0,0)
        // _TintColor ("Color", color) = (1,1,1,1)
        // _LightColor ("Light Color", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent""Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        // ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // uniform float4x4 _ProjectionMatrix; // Projection matrix of the camera acting as a projector
            // uniform float4x4 _WorldToCameraMatrix; // world to camera (acting as a projector) matrix

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 pos : COLOR;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float3 pos : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 wPos : TANGENT;
            };

            samplerCUBE _CubeTex;
            // sampler2D _MainTex;
            // float4 _MainTex_ST;
            // float4 _LightData;
            // float4 _TintColor;
            // float4 _LightColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);

                // float4 res = mul(unity_ObjectToWorld, v.vertex); // vertex in world position
                // o.wPos = res;
                // float4x4 m = mul(_ProjectionMatrix, _WorldToCameraMatrix); //
                // o.uv =  mul(_ProjectionMatrix, res); // clipped position in projector space

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // float4 calculateLight(fixed4 outCol, v2f i){
            //     float l = saturate(1-length(i.wPos - _LightData.xyz)*_LightData.w);
            //     float4 o = outCol*_TintColor + outCol*float4(l,l,l,1) * _LightColor*_LightColor.a*5;
            //     o.a = outCol.a;
            //     return o ;
            // }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float3 spherical = normalize(i.pos);
                fixed4 cubeCol = texCUBE(_CubeTex, spherical);
                // float2 UV = ( i.uv.xy/(i.uv.w))*.5-float2(.5,.5);
                // fixed4 col = tex2D(_MainTex,UV);
                // // apply fog
                // fixed4 outCol = lerp(cubeCol,col,saturate((1-length(i.uv.xy/(i.uv.w)))*3)*saturate(i.pos.z));
                // // clip(outCol.a-.1);
                // UNITY_APPLY_FOG(i.fogCoord, col);

                return cubeCol;
            }
            ENDCG
        }
    }
}
