Shader "Custom/SingleRingShader"
{
    Properties
    {
        _MainColor("Color", Color) = (1,1,1,1)
        _Scale("Scale", Float) = 1.0
        _Thickness("Thickness", Float) = 0.1
        _EdgeSoftness("Edge Softness", Float) = 0.01
        _Brightness("Brightness", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100
                ZTest Off

        Blend SrcAlpha OneMinusSrcAlpha

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

            float4 _MainColor;
            float _Scale;
            float _Thickness;
            float _EdgeSoftness;
            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Normalized coordinates
                float2 uv = i.uv * 2 - 1;
                uv *= _Scale;

                // Create the ring
                float dist = length(uv);
                // dist+= _Brightness;
                float alpha = smoothstep(_Thickness + _EdgeSoftness, _Thickness - _EdgeSoftness, dist);

                // Color and brightness
                float4 color = _MainColor;
                float a = saturate((cos((saturate(alpha)*6.28))*-.5+.5)*_Brightness);//

                // Final color output
                return float4(color.rgb, a*color.a);
            }
            ENDCG
        }
    }
}
