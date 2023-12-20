Shader "Unlit/PlatformShaderStripes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Square ("Square", vector) = (0,.4,.4,0)
        _Edge ("Edge", vector) = (.2,.3,.5,0)
        _HueShift("Hue Shift", float) = 0.0
        _MasterHueShift("Master Hue Shift", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent""Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 pos : Color;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _Square;
            float _HueShift;
            float _MasterHueShift;
            float4 _Edge;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 rgb_to_hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float sdBox( in float2 p, in float2 b )
            {
                float2 d = abs(p)-b;
                return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
            }

            float3 hsv_to_rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            fixed4 HueRotate(fixed4 color, float hueAngle)
            {
                // Convert RGB to HSV
                float3 hsv = rgb_to_hsv(color.rgb);

                // Rotate the hue, ensuring it remains in the range [0, 1]
                hsv.x += hueAngle;
                if (hsv.x > 1.0) hsv.x -= 1.0;
                if (hsv.x < 0.0) hsv.x += 1.0;

                hsv.z = (hsv.z+.5)%1;

                // Convert back to RGB
                color.rgb = hsv_to_rgb(hsv);
                return color;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                _Color = HueRotate(_Color,_MasterHueShift);
                float l = length(i.uv-.5);
                float b = sdBox(i.uv-.5,float2(_Square.y,_Square.z))+.25;
                l = lerp(l,b,_Square.x);
                float ring = smoothstep(_Edge.x,_Edge.x+_Edge.y, l);
                float edge = cos(ring*6.28)*-.5+.5;
                edge*=_Edge.w;
                float center = smoothstep(_Edge.x,0,l);
                float shadow = smoothstep(_Edge.z,_Edge.x,l);
                float shadowLerp = smoothstep(_Edge.x+_Edge.y*.5+.01,_Edge.x+_Edge.y*.5,l);
                float4 col = lerp(_Color,HueRotate(_Color,_HueShift),saturate(edge+center*_Square.w))+edge*abs(sin(_Time.z+i.pos.y)*.5);
                // col = lerp( col ,col*.2,ring);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float4 c = lerp(_Color,HueRotate(_Color,_HueShift),(1-l*2));
                return float4(c.r,c.g,c.b,saturate((sin(l*50+_Time.z*3)*.5)+.5)*saturate(1-l*2));//float4((col*(.2+shadowLerp)).xyz,lerp(shadow*.6,1,shadowLerp));
            }
            ENDCG
        }
    }
}
