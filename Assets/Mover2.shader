Shader "Custom/Moving2"
{
    Properties
    {
        _LineColor("Line Color", Color) = (1,1,1,1)
        // _BackgroundColor("Background Color", Color) = (0,0,0,1)
        _Speed("Speed", Float) = 1.0
        _LineFrequency("Line Frequency", float) = 10.0
        _CircleRadius("Circle Radius", float) = 0.5
        _AngleAmount ("Angle Amount", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend One One
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

            float4 _LineColor;
            float4 _BackgroundColor;
            float _Speed;
            float _LineFrequency;
            float _CircleRadius;
            float _AngleAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Create moving lines
                float lineValue = abs(sin(i.uv.x * _LineFrequency + _Time.y * _Speed+ abs(i.uv.y-.5)*_AngleAmount));

                // Create circle mask
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float circleMask = smoothstep(_CircleRadius + 0.1, _CircleRadius, dist);

                // Combine line and circle mask
                // float4 color = lerp(_BackgroundColor, _LineColor, step(0.0, lineValue) * circleMask);

                return pow(lineValue,3)*circleMask*_LineColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
