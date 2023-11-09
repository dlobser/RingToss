Shader "Unlit/maponsphere"
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
// Cull Front
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            

            // struct appdata
            // {
            //     float4 vertex : POSITION;
            //     float2 uv : TEXCOORD0;
            // };

            // struct v2f
            // {
            //     float2 uv : TEXCOORD0;
            //     UNITY_FOG_COORDS(1)
            //     float4 vertex : SV_POSITION;
            //     float3 pos : COLOR;
            // };

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
                float3 pos : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DepthTex;
            float _Parallax;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = float2(v.vertex.x+1,v.vertex.z+1);//TRANSFORM_TEX(v.uv, _MainTex);
                fixed4 dep = tex2Dlod(_DepthTex, float4(o.uv.xy*.5,0,0));
                float l = length(v.vertex.xz);
                float3 vv = v.vertex;
                vv *= 1+ max(0,sign(v.vertex.y))*(dep-.5)*_Parallax*max(.2,l)*v.vertex.y;
                o.vertex = UnityObjectToClipPos(vv);

                o.pos = v.vertex;
                // o.pos.x = sin(v.vertex.z+_Time.y*4*6);
                v.tangent = float4(0,0,1,0);
                TANGENT_SPACE_ROTATION;
                o.viewDir = mul (rotation, ObjSpaceViewDir(v.vertex));
                o.viewDir.x = (.5*(1+sin(v.vertex.y+dep*2+_Time.y*6)));
                o.viewDir.z = (.5*(1+sin(v.vertex.y+dep*2+_Time.y*3.12345)));
                o.viewDir.y = l+dep;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture

                // fixed4 dep = tex2D(_DepthTex, i.uv);

                // float2 parallaxOffset = ParallaxOffset((1-dep.r)-.25, _Parallax, i.viewDir);
                
                // fixed4 col = tex2D(_MainTex, i.uv+parallaxOffset);

                // // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                // clip(col.a-.01);
                // return col;//float4(parallaxOffset.x,parallaxOffset.y,0,1);

                fixed4 col = tex2D(_MainTex, float2(i.pos.x+1,i.pos.z+1)*.5);

            
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                //
                return ((1-i.viewDir.z)*col.g*(1-i.viewDir.x)*(1-i.viewDir.y)*.2+(i.viewDir.x)*.2*col.r)*saturate(i.pos.y-1)+col*saturate(i.pos.y*.5);
            }
            ENDCG
        }
    }
}
