Shader "Unlit/CameraProjectionShader"
{
 
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 res = mul(unity_ObjectToWorld, v.vertex); // vertex in world position
                float4x4 m = mul(_ProjectionMatrix, _WorldToCameraMatrix); //
                o.uv =  mul(_ProjectionMatrix, res); // clipped position in projector space
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv.xy);
                // return col;
                fixed4 col = tex2D(_MainTex,( i.uv.xy/(i.uv.w))*.5-float2(.5,.5));
                return col;
            }
            ENDCG
        }
    }
}