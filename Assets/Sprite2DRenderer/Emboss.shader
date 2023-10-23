Shader "Custom/SpriteEmboss" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _EmbossStrength ("Emboss Strength", Range(0, 1)) = 0.1
        _EmbossDepth ("Emboss Depth", Range(0, 1)) = 0.2
    }

    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float _EmbossStrength;
        float _EmbossDepth;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            // Sample the sprite texture
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate the emboss direction in screen space using the screen space normals
            float3 screenNormal = UnpackNormal(tex2D(_MainTex, IN.uv_MainTex));
            float3 embossDir = normalize(screenNormal);

            // Calculate the emboss factor based on the dot product of the emboss direction and the view direction
            float embossFactor = saturate(dot(embossDir, _WorldSpaceCameraPos));

            // Adjust the emboss factor based on the strength and depth properties
            embossFactor = pow(embossFactor, _EmbossDepth) * _EmbossStrength;

            // Combine the original color with the embossed color
            col.rgb += embossFactor;

            o.Albedo = screenNormal.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
}