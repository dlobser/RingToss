Shader "Custom/normalFromDepth"
{
    Properties
    {
        _MainTex ("Depth Texture", 2D) = "white" {}
        _Strength ("Strength", Range(0, 1)) = 0.1
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
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
            float _Strength;
 
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample depth value
                float depth = tex2D(_MainTex, i.uv).r;
                
                // Calculate normals using finite difference
                float2 delta = float2(0.01, 0.01); // Adjust the delta as needed
                float leftDepth = tex2D(_MainTex, i.uv - float2( delta.y,0 )).r;
                float rightDepth = tex2D(_MainTex, i.uv + float2( delta.y,0 )).r;
                float upDepth = tex2D(_MainTex, i.uv + float2(0, delta.y)).r;
                float downDepth = tex2D(_MainTex, i.uv - float2(0, delta.y)).r;
                
                float3 normal;
                normal.x = (rightDepth - leftDepth) / (2.0 * delta.x);
                normal.y = (upDepth - downDepth) / (2.0 * delta.y);
                normal.z = 1.0; // Assuming depth increases towards the camera
                
                // Transform normal to world space
                normal = UnityObjectToWorldNormal(normal);
                
                // Normalize and adjust strength
                normal = normalize(normal) * _Strength;
                
                return float4(normal*.5+.5,1);
            }
 
            ENDCG
        }
    }
}
