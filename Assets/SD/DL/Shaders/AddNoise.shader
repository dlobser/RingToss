Shader "Unlit/AddNoise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseData("Noise Data", vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent""Queue"="Transparent" }
        LOD 100
        Blend One One
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Noise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _NoiseData;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float2 UV = i.uv;
                // UV.x = abs(UV.x-.5);
                // i.uv= UV;
                float r = length(UV-.5);
                float theta = atan2(UV.y-.5, UV.x-.5);
                i.uv = float2(sin(r*35),sin(theta*3));
                float nr = snoise(float3(i.uv.x,i.uv.y,_Time.y*_NoiseData.y))*_NoiseData.z;
                float ng = snoise(float3(i.uv.x,i.uv.y,_Time.y*_NoiseData.y+_NoiseData.w*2))*_NoiseData.z;//snoise(float3(i.uv.x*_NoiseData.x+(_NoiseData.w*2),i.uv.y*_NoiseData.x,_Time.y*_NoiseData.y))*_NoiseData.z;
                float nb = snoise(float3(i.uv.x,i.uv.y,_Time.y*_NoiseData.y+_NoiseData.w*3))*_NoiseData.z;//snoise(float3(i.uv.x*_NoiseData.x+(_NoiseData.w*3),i.uv.y*_NoiseData.x,_Time.y*_NoiseData.y))*_NoiseData.z;

                float nr2 = snoise(float3(i.uv.x*_NoiseData.x*.2+(_NoiseData.w*1),i.uv.y*_NoiseData.x*.2,_Time.y*_NoiseData.y))*_NoiseData.z;
                float ng2 = snoise(float3(i.uv.x*_NoiseData.x*.2+(_NoiseData.w*2),i.uv.y*_NoiseData.x*.2,_Time.y*_NoiseData.y))*_NoiseData.z;
                float nb2 = snoise(float3(i.uv.x*_NoiseData.x*.2+(_NoiseData.w*3),i.uv.y*_NoiseData.x*.2,_Time.y*_NoiseData.y))*_NoiseData.z;


                return float4(nr,ng,nb,1);
            }
            ENDCG
        }
    }
}
