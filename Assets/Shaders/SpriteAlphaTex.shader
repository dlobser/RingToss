Shader "Custom/SpriteAlphaTex"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _AlphaTex ("Sprite Alpha Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Clip ("Clip Black", float) = 0.0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1) // Hidden variable for the SpriteRenderer's color
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #ifndef UNITY_SPRITES_INCLUDED
            #define UNITY_SPRITES_INCLUDED

            #include "UnityCG.cginc"

            #ifdef UNITY_INSTANCING_ENABLED
                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    // SpriteRenderer.Color while Non-Batched/Instanced.
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
            #endif // instancing

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 col : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 col : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaTex;
            float4 _AlphaTex_ST;
            float4 _Color;
            float _Clip;
            
            #ifndef UNITY_INSTANCING_ENABLED
                fixed4 _RendererColor;
            #endif

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                o.col = v.col * _RendererColor * _Color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                half4 alpha = tex2D(_AlphaTex, i.uv);
                // col *= _Color;
                col.a = alpha.r;

                // // Clip the blacks
                // if (col.r < _Clip && col.g < _Clip && col.b < _Clip)
                //     discard;
                
                return col * i.col;
            }

            #endif // UNITY_SPRITES_INCLUDED
            ENDCG
        }
    }
}
