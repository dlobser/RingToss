Shader "Unlit/SDFNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Data ("Data", vector) = (1,1,1,1)
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 pos : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Data;

            float sdRoundBox(float3 p, float3 b, float r)
            {
                b*=float3(_Data.y,_Data.z,1);
                float3 q = abs(p) - b;
                r*=_Data.x;
                
                return smoothstep(.5-_Data.w,.5+_Data.w,(length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r));
            }

      

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld,v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float3 normal = normalize(float3(
                    sdRoundBox(i.pos + float3(0.01, 0, 0), float3(1, 1, 1), 0.5) - sdRoundBox(i.pos - float3(0.01, 0, 0), float3(1, 1, 1), 0.5),
                    sdRoundBox(i.pos + float3(0, 0.01, 0), float3(1, 1, 1), 0.5) - sdRoundBox(i.pos - float3(0, 0.01, 0), float3(1, 1, 1), 0.5),
                    sdRoundBox(i.pos + float3(0, 0, 0.01), float3(1, 1, 1), 0.5) - sdRoundBox(i.pos - float3(0, 0, 0.01), float3(1, 1, 1), 0.5)
                ));
                normal.z = 1-sdRoundBox(i.pos,float3(1,1,1),.5)*2;
                //()
                return float4(normal*.5+.5,1);
            }
            ENDCG
        }
    }
}

// Shader "Unlit/SDFNormal"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         LOD 100

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             // Enable fog
//             #pragma multi_compile_fog

//             #include "UnityCG.cginc"

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 float4 vertex : SV_POSITION;
//                 float3 worldPos : TEXCOORD1;
//                 UNITY_FOG_COORDS(1)
//             };

//             sampler2D _MainTex;
//             float4 _MainTex_ST;

//             float sdRoundBox(float3 p, float3 b, float r)
//             {
//                 float3 q = abs(p) - b;
//                 return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
//             }

//             v2f vert (appdata v)
//             {
//                 v2f o;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//                 o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//                 UNITY_TRANSFER_FOG(o, o.vertex);
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 // Sample the texture
//                 fixed4 col = tex2D(_MainTex, i.uv);
//                 // Calculate normal using SDF approximation
//                 float3 normal = normalize(float3(
//                     sdRoundBox(i.worldPos + float3(0.01, 0, 0), float3(1, 1, 1), 0.5) - sdRoundBox(i.worldPos - float3(0.01, 0, 0), float3(1, 1, 1), 0.5),
//                     sdRoundBox(i.worldPos + float3(0, 0.01, 0), float3(1, 1, 1), 0.5) - sdRoundBox(i.worldPos - float3(0, 0.01, 0), float3(1, 1, 1), 0.5),
//                     sdRoundBox(i.worldPos + float3(0, 0, 0.01), float3(1, 1, 1), 0.5) - sdRoundBox(i.worldPos - float3(0, 0, 0.01), float3(1, 1, 1), 0.5)
//                 ));
//                 // Apply fog
//                 UNITY_APPLY_FOG(i.fogCoord, col);
//                 // Output the normal as color (for visualization)
//                 return float4(normal * 0.5 + 0.5, 1);
//             }
//             ENDCG
//         }
//     }
// }
