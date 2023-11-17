// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/SpriteLighting3"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        // _Palette ("Palette Texture", 2D) = "white" {}
        _TextureTex ("Texture Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        // _LineColor ("LineColor", Color) = (1,1,1,1)
        _Ambient ("Ambient Color", Color) = (1,1,1,1)
        _Data("Data",vector) = (0,0,0,0)
        _Edge ("Edge", vector ) = (0,0,0,0)
        // _CamPos("CamPos", vector) = (0,0,0,0)
        _Power ("Power", vector) = (1,1,1,1)
        _MainLightPosition("MainLightPosition", Vector) = (0,0,0,0)
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
                float2 wcoord : TEXCOORD1;
                float3 wPos : NORMAL;
                float3 wDir : TANGENT;
                float4 angle : TEXCOORD2;
                float3 T : TEXCOORD3;
				float3 B : TEXCOORD4;
				float3 N : TEXCOORD5;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            sampler2D _MainTex;
            sampler2D _Palette;
            sampler2D _TextureTex;
            sampler2D _AlphaTex;
            float4 _Data;
            float4 _Power;
            float4 _MainLightPosition;
            float4 _Ambient;
            float4 _Edge;
            float4 _LineColor;

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                // UNITY_SETUP_INSTANCE_ID (IN);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);

                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;// _Color * _RendererColor;

                // OUT.wPos = IN.vertex;//mul(unity_ObjectToWorld,IN.vertex).xyz;

                // calc lightDir vector heading current vertex
				float4 worldPosition = mul(unity_ObjectToWorld, IN.vertex);
				float3 lightDir = worldPosition.xyz - _MainLightPosition.xyz;
				OUT.wDir = normalize(lightDir);

				// calc viewDir vector 
				float3 viewDir = normalize(worldPosition.xyz - float3(0,0,10000));
				OUT.wPos = worldPosition;

				// calc Normal, Binormal, Tangent vector in world space
				// cast 1st arg to 'float3x3' (type of input.normal is 'float3')
				float3 worldNormal = mul((float3x3)unity_ObjectToWorld, IN.wPos);
				float3 worldTangent = mul((float3x3)unity_ObjectToWorld, IN.wDir);
				
				float3 binormal = cross(IN.wPos, IN.wDir.xyz); // *input.tangent.w;
				float3 worldBinormal = mul((float3x3)unity_ObjectToWorld, binormal);

				// and, set them
				OUT.N = normalize(worldNormal);
				OUT.T = normalize(worldTangent);
				OUT.B = normalize(worldBinormal);

                OUT.angle = float4(0,0,0,0);
                float3 wp = mul(unity_ObjectToWorld,float3(-1,-1,-1)).xyz;
                float3 wo = mul(unity_ObjectToWorld,IN.vertex).xyz;
                float3 m = wp-wo;
                OUT.angle.xyz = wo.xyz;
                OUT.angle.w = length(worldPosition.xyz - _MainLightPosition.xyz);

                OUT.wcoord = IN.vertex.xy;//mul(unity_ObjectToWorld,IN.vertex).xy;//mul(unity_ObjectToWorld,worldPosition.xyz).xy;
			           
                #ifdef PIXELSNAP_ON
                // OUT.vertex = UnityPixelSnap (OUT.vertex);
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

            // half3 ObjectScale() {
            //     return half3(
            //         length(unity_ObjectToWorld._m00_m10_m20),
            //         length(unity_ObjectToWorld._m01_m11_m21),
            //         length(unity_ObjectToWorld._m02_m12_m22)
            //     );
            // }
            float2 ConvertNumberToGridCoordinate(float number)
            {

                float stepSize = 4;

                // Calculate the x and y coordinates in the grid
                int x = (number);
                int y = 0;//floor((number - x * stepSize) / stepSize);

                // Return the grid coordinates as float2
                return float2(x, x);
            }

            fixed4 GetNormalFromDepthTexture(v2f IN)
            {
                // Sample depth value
                float depth = tex2D(_TextureTex, IN.texcoord).r;
                
                // Calculate normals using finite difference
                float2 delta = float2(_Data.z,_Data.z); // Adjust the delta as needed
                float leftDepth = tex2D(_TextureTex, IN.texcoord - float2( delta.y,0 )).r;
                float rightDepth = tex2D(_TextureTex, IN.texcoord + float2( delta.y,0 )).r;
                float upDepth = tex2D(_TextureTex, IN.texcoord + float2(0, delta.y)).r;
                float downDepth = tex2D(_TextureTex, IN.texcoord - float2(0, delta.y)).r;
                
                float3 normal;
                normal.x = (leftDepth - rightDepth) / (2.0 * delta.x);
                normal.y = (downDepth - upDepth) / (2.0 * delta.y);
                normal.z = 1.0; // Assuming depth increases towards the camera
                
                // Transform normal to world space
                normal = UnityObjectToWorldNormal(normal);
                
                // Normalize and adjust strength
                normal = normalize(normal) * _Data.w;
                
                return float4(normal*.5+.5,1);
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord);
				float3 tangentNormal = GetNormalFromDepthTexture(IN);// tex2D(_MainTex, IN.texcoord).xyz;
                float4 textureTex = tex2D(_MainTex, (IN.texcoord));
				tangentNormal = normalize(tangentNormal * 2 - 1);
				float3x3 TBN = float3x3(normalize(IN.T), normalize(IN.B*-1), normalize(IN.N));
				TBN = transpose(TBN);
                float a = (IN.angle.x+3.1415)/(3.1415*2);
                float3x3 rot = rotationMatrix(float3(0,0,1),a*-6.28*2);
				float3 worldNormal = mul(TBN,  tangentNormal);
				float4 albedo = tex2D(_MainTex, IN.texcoord);
				float3 lightDir = normalize(IN.wDir);
				float3 diffuse = saturate((dot(worldNormal, -lightDir)+1)*.5* _Power.w );
                float dropoff = (pow((((1-length(IN.wPos.xyz-_MainLightPosition.xyz))*_Power.x*.01)+_Power.y),_Power.z));//
                float alpha = smoothstep(.5,1,albedo.a);
                float li = smoothstep(_Edge.x+IN.color.g,_Edge.y+IN.color.g,albedo.a);
                textureTex.xyz*=diffuse.r*dropoff*_Color;
                textureTex.xyz += _Ambient.rgb;
				return textureTex;
            }

            #endif // UNITY_SPRITES_INCLUDED

        ENDCG
        }
    }
}
