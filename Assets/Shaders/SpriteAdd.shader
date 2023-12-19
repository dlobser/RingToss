Shader "Custom/SpriteGlow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GlowTex ("Glow Texture", 2D) = "white" {}
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
            sampler2D _GlowTex;
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
                o.uv = v.uv;

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                half4 glow = tex2D(_GlowTex, i.uv);
                float s = abs(sin(sin(i.uv.x*10)+i.uv.x*.5+i.uv.y*10+_Time.y*1));
                // glow*=((s+.5)*.5);
                // col *= _Color;
                // col+=glow*col.a;

                float p = lerp(0,(s)*1-col,saturate(glow*4-3));
                float4 pp = float4(p,p,p,0);//*(1-col);
                return saturate(pp)+col;//col+saturate(i.col*(glow.r)*(s)*1-col);//col * i.col;
            }

            #endif // UNITY_SPRITES_INCLUDED
            ENDCG
        }
    }
}
