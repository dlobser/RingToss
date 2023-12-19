// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Depth"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Data("Data",vector) = (0,0,0,0)
        _CamPos("CamPos", vector) = (0,0,0,0)
        _Power ("Power", float) = 1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
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

            // Material Color.
            fixed4 _Color;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 wPos : NORMAL;
                float3 wDir : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 wPos : NORMAL;
                float3 wDir : TANGENT;
                float4 angle : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);

                float2 screenPos = ComputeScreenPos(UnityObjectToClipPos(OUT.vertex));
                float2 objPos = OUT.vertex.xy / OUT.vertex.w;
                float2 dir = normalize(OUT.vertex - IN.vertex);
                float rotationAngle = atan2(dir.y, dir.x);

                OUT.angle = float4(0,0,0,0);
                OUT.angle.x = rotationAngle;
                // billboard mesh towards camera
                //float3 vpos = mul((float3x3)_Object2World, IN.vertex.xyz);
                //float4 worldCoord = float4(_Object2World._m03, _Object2World._m13, _Object2World._m23, 1);
                //float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
                //float4 outPos = mul(UNITY_MATRIX_P, viewPos);

 
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                OUT.wPos = IN.vertex;//mul(unity_ObjectToWorld,IN.vertex).xyz;

                half3 wNormal = UnityObjectToWorldNormal(IN.wPos);
                half3 wTangent = UnityObjectToWorldDir(IN.wDir.xyz);
                wNormal = mul((float3x3)UNITY_MATRIX_V, wNormal);
                wTangent = mul((float3x3)UNITY_MATRIX_V, wTangent)-wNormal;

                float3 wp = mul(unity_ObjectToWorld,float4(0,0,0,1)).xyz;
                float3 wo = mul(unity_ObjectToWorld,float4(0,1,0,1)).xyz;
                float3 m = wp-wo;
                OUT.angle.y = atan2(m.x,m.y);

                OUT.wDir = wTangent;

                


                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            float3x3 rotationMatrix(float3 axis, float angle)
            {
                axis = normalize(axis);
                float s = sin(angle);
                float c = cos(angle);
                float oc = 1.0 - c;

                return float3x3(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
                    oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
                    oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float4 _Data;
            float _Power;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

            #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, uv);
                color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
            #endif

                return color;
            }

            float fixSin(float i){
                return .9+((1+sin(i*6.28*.1))*.5)*.1;
            }
            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord);
                float a = (IN.angle.y+3.1415)/(3.1415*2);
                fixed4 c2 = SampleSpriteTexture (IN.texcoord+
                mul(rotationMatrix(float3(0,0,1),a*6.28),float3(0,IN.wDir.y*.1,0))/sin(IN.angle.y) );
                fixed4 c3 = SampleSpriteTexture (IN.texcoord+
                mul(rotationMatrix(float3(0,0,1),a*6.28),float3(0,(IN.wDir.y*-.1),0))/sin(IN.angle.y) );
                c2 *= c2.a;
                c.rgb *= c.a;
                float d = (length(IN.wPos-_WorldSpaceCameraPos)+_Data.x)*_Data.y;
                d = pow(d,_Power)+_Data.w;
                d*=c.a;

                float3 b = IN.wDir.xyz;
                float p = saturate(c*(1-c2))+c*.5-saturate(c3.a);
                // float4 wCol = float4(fixSin(IN.wPos.x+_Time.x),fixSin(IN.wPos.x+_Time.y),fixSin(IN.wPos.z+_Time.x),1);
                return p;//float4(p,p,p,1);
            }

            #endif // UNITY_SPRITES_INCLUDED

        ENDCG
        }
    }
}
