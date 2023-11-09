Shader "Unlit/CameraProjectionShaderFade"
{
 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        _MainTex3 ("Texture", 2D) = "white" {}
        _Mask("Mask",2D) = "white" {}
        _Data("Data",vector) = (0,0,0,0)
        _Data2("Data2",vector) = (0,0,0,0)
        _Clip("Clip",float) = 0
        _Transition("Transition", float) = 0
    }
 
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            uniform float4x4 _ProjectionMatrix; // Projection matrix of the camera acting as a projector
            uniform float4x4 _WorldToCameraMatrix; // world to camera (acting as a projector) matrix
 
            struct appdata
            {
                float4 vertex : POSITION;

            };
 
            struct v2f
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 pos : COLOR;
            };
 
            sampler2D _MainTex;
            sampler2D _MainTex2;
            sampler2D _MainTex3;
            sampler2D _Mask;
            float4 _MainTex_ST;
            float4 _Data;
           float4 _Data2;
           float _Clip;
           float _Transition;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 res = mul(unity_ObjectToWorld, v.vertex); // vertex in world position
                float4x4 m = mul(_ProjectionMatrix, _WorldToCameraMatrix); //
                o.uv =  mul(_ProjectionMatrix, res); // clipped position in projector space
                o.pos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv.xy);
                // return col;
                float2 UV = ( i.uv.xy/(i.uv.w))*.5-float2(.5,.5);
                fixed4 col = tex2D(_MainTex,UV);
                fixed4 col2 = tex2D(_MainTex2,UV);
                fixed4 mask = tex2D(_Mask,UV);
                float cloud = tex2D(_MainTex3,(i.pos.xz+float2(0,i.pos.y*.01))*_Data2.x+float2(_Time.y*_Data2.y,_Time.y*_Data2.z));
                float l = length(UV+float2(.5,.5));
                float s = sin(l*3+_Time.y*6)*.1+.9;
                float4 output = (1-l*3)*.2+s*cloud*_Data2.w * lerp(col,col2,saturate(_Transition+_Data.x+col.r*_Data.y+l*_Data.z));
                clip(mask.r-_Clip);
                return output;
            }
            ENDCG
        }
        Pass
        {
            Tags {"LightMode"="ShadowCaster"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

             struct appdata
            {
                float4 vertex : POSITION;
            };
 
            struct v2f
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

             v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 res = mul(unity_ObjectToWorld, v.vertex); // vertex in world position
                // float4x4 m = mul(_ProjectionMatrix, _WorldToCameraMatrix); //
                // o.uv =  mul(_ProjectionMatrix, res); // clipped position in projector space
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv.xy);
                // return col;
                // fixed4 col = tex2D(_MainTex,( i.uv.xy/(i.uv.w))*.5-float2(.5,.5));
                return 1;
            }
            ENDCG
        }
    }
     
}