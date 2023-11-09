Shader "Unlit/Depth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Data("Data",vector) = (0,0,0,0)
        _CamPos("CamPos", vector) = (0,0,0,0)
        _Pow ("Pow", float) = 0
        _NoiseAmount ("Noise Amount", float) = 0
        _NoiseFrequency ("Noise Frequency", float) = 1
        _NoiseSpeed ("NoiseSpeed", float) = 0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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
                float3 pos : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 pos : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Data;
            float4 _CamPos;
            float _Pow;
            float _NoiseAmount;
            float _NoiseFrequency;
            float _NoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float noise = snoise(i.pos*_NoiseFrequency+_NoiseSpeed*_Time.y);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return lerp(1,(noise+1)/2,_NoiseAmount)*pow((length(i.pos-_WorldSpaceCameraPos)+_Data.x)*_Data.y,_Pow)+_Data.w;
            }
            ENDCG
        }
    }
}
