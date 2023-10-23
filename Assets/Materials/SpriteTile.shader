Shader "Custom/SlicedSpriteWithTiling"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TileTex ("Tiling Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _TileAmount ("Tile Amount", float) = 1.0
        [HideInInspector] _MainTex_TexelSize ("Texture Size", Vector) = (0,0,0,0)
        [HideInInspector] _MainTex_SpriteBorder ("Sprite Border", Vector) = (0,0,0,0)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1) // Hidden variable for the SpriteRenderer's color

    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcColor OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            // #include "spriteDepth.cginc"

                        
            #ifndef UNITY_SPRITES_INCLUDED
            #define UNITY_SPRITES_INCLUDED

            #include "UnityCG.cginc"

            #ifdef UNITY_INSTANCING_ENABLED

                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    // SpriteRenderer.Color while Non-Batched/Instanced.
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                    // this could be smaller but that's how bit each entry is regardless of type
                    UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

            #endif // instancing

            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
                fixed4 _RendererColor;
                fixed2 _Flip;
            #endif
                float _EnableExternalAlpha;
            CBUFFER_END


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 col : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID

            };

            struct v2f
            {
                float2 uvMain : TEXCOORD0;
                float2 uvTile : TEXCOORD1;
                float4 col : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _TileTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_SpriteBorder;
            float _TileAmount;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                o.col = v.col * _RendererColor;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Calculate the UV for the sprite texture
                o.uvMain = v.uv;

                // Calculate tiling UV based on sprite borders
                float2 borderUV = _MainTex_SpriteBorder.xy / _MainTex_TexelSize.xy;
                o.uvTile = v.vertex.xy * _TileAmount;// lerp(borderUV, 1.0 - borderUV, v.uv) * _TileAmount;
                
                #ifdef PIXELSNAP_ON
                    o.vertex = UnityPixelSnap (o.vertex);
                #endif
                
                return o;
            }

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                    fixed4 alpha = tex2D (_AlphaTex, uv);
                    color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
                #endif

                    return color;
                }
/*
            float4 OverlayBlend(float4 lower, float4 upper) {
                upper.a = smoothstep(.45,.55,1-upper.g);
                float4 isGreaterThanHalf = step(0.5, lower);
                float4 valueUnitLess = lower / 0.5;
                float4 valueUnitMore = (1.0 - lower) / 0.5;
                float4 minValue = lower - (1.0 - lower);

                float4 resultLess = upper * valueUnitLess;
                float4 resultMore = (upper * valueUnitMore) + minValue;

                return lerp(resultLess, resultMore, isGreaterThanHalf);
            }
*/

            half4 frag (v2f i) : SV_Target
            {
                half4 tileCol = tex2D(_TileTex, i.uvTile);
                half4 mainCol = tex2D(_MainTex, i.uvMain + tileCol.xy*.05 - float2(.025,.025));
                mainCol.a = smoothstep(.45,.55,mainCol.a);
                //float4 oblend = OverlayBlend(mainCol, tileCol);
                //mainCol.a = oblend.a;
                half4 finalColor = lerp(mainCol, tileCol,  mainCol.a);
                finalColor *= _RendererColor * mainCol.a;
                return float4(finalColor.r,finalColor.g,finalColor.b,mainCol.a);
            }
            #endif // UNITY_SPRITES_INCLUDED
            ENDCG
        }
    }
}
