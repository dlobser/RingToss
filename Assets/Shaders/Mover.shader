Shader "Custom/MovingLinesCircleMask"
{
    Properties
    {
        _Color("Line Color", Color) = (1,1,1,1)
        // _BackgroundColor("Background Color", Color) = (0,0,0,1)
        _Speed("Speed", Float) = 1.0
        _LineFrequency("Line Frequency", Float) = 10.0
        _CircleRadius("Circle Radius", Float) = 0.5
        _Fade ("Fade", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float4 _BackgroundColor;
            float _Speed;
            float _LineFrequency;
            float _CircleRadius;
            float _Fade;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // fixed4 HueRotate(fixed4 color, float angle)
            // {
            //     float3 k = float3(0.57735, 0.57735, 0.57735);
            //     float cosAngle = cos(angle);
            //     float3x3 rotationMatrix = float3x3(
            //         cosAngle + k.x * k.x * (1.0 - cosAngle),
            //         k.x * k.y * (1.0 - cosAngle) - k.z * sin(angle),
            //         k.x * k.z * (1.0 - cosAngle) + k.y * sin(angle),

            //         k.y * k.x * (1.0 - cosAngle) + k.z * sin(angle),
            //         cosAngle + k.y * k.y * (1.0 - cosAngle),
            //         k.y * k.z * (1.0 - cosAngle) - k.x * sin(angle),

            //         k.z * k.x * (1.0 - cosAngle) - k.y * sin(angle),
            //         k.z * k.y * (1.0 - cosAngle) + k.x * sin(angle),
            //         cosAngle + k.z * k.z * (1.0 - cosAngle)
            //     );

            //     color.rgb = mul(rotationMatrix, color.rgb);
            //     return color;
            // }

            float3 rgb_to_hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
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

            


            float4 frag (v2f i) : SV_Target
            {
                // Create moving lines
                float lineValue = abs(sin(i.uv.x * _LineFrequency + _Time.y * _Speed+ abs(i.uv.y-.5)*10));

                // Create circle mask
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float circleMask = smoothstep(_CircleRadius + 0.1, _CircleRadius, dist);
                // float3 c1 = rgb_to_hsv(_Color.rgb);
                float edge = cos(circleMask*6.28)*-.5+.5;
                float3 c2 = HueRotate(_Color,.5).xyz;
                // float3 c3 = hsv_to_rgb(c2);
                float3 coo = c2;//lerp(_Color.rgb,c3,1);
                float4 cool = float4(coo,1);
                // Combine line and circle mask
                // float4 color = lerp(_BackgroundColor, _Color, step(0.0, lineValue) * circleMask);
                float4 lerpy = lerp(cool,_Color*2,pow(lineValue,3));
                
                return float4(lerpy.xyz,circleMask*saturate(_Fade));//*_Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
