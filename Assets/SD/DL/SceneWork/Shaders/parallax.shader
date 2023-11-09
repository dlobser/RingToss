Shader "Unlit/parallax_unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthTex ("Texture", 2D) = "white" {}
        _Parallax ("Parallax", float) = 1

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
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _DepthTex;
            float4 _MainTex_ST;
            float _Parallax;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TANGENT_SPACE_ROTATION;
                o.viewDir = mul (rotation, ObjSpaceViewDir(v.vertex));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 dep = tex2D(_DepthTex, i.uv);

                float2 parallaxOffset = ParallaxOffset((1-dep.r)*.5+.5, _Parallax, i.viewDir);
                
                fixed4 col = tex2D(_MainTex, i.uv+parallaxOffset);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                clip(col.a-.01);
                return col;//float4(parallaxOffset.x,parallaxOffset.y,0,1);
            }
            ENDCG
        }
    }
}
