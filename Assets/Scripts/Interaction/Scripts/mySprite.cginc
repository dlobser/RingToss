// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

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
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex   : SV_POSITION;
    fixed4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    float3 wPos : NORMAL;
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

    // billboard mesh towards camera
    //float3 vpos = mul((float3x3)_Object2World, IN.vertex.xyz);
    //float4 worldCoord = float4(_Object2World._m03, _Object2World._m13, _Object2World._m23, 1);
    //float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
    //float4 outPos = mul(UNITY_MATRIX_P, viewPos);

    OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
    OUT.vertex = UnityObjectToClipPos(OUT.vertex);
    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color * _Color * _RendererColor;

    OUT.wPos = mul(unity_ObjectToWorld,IN.vertex).xyz;
    #ifdef PIXELSNAP_ON
    OUT.vertex = UnityPixelSnap (OUT.vertex);
    #endif

    return OUT;
}

sampler2D _MainTex;
sampler2D _Hover;
sampler2D _Click;
sampler2D _Inactive;
sampler2D _AlphaTex;
float4 _Mix;

fixed4 SampleSpriteTexture (sampler2D tex, float2 uv)
{
    fixed4 color = tex2D (tex, uv);

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
    fixed4 base = SampleSpriteTexture (_MainTex, IN.texcoord);// * IN.color;
    fixed4 hover = SampleSpriteTexture (_Hover, IN.texcoord);// * IN.color;
    fixed4 click = SampleSpriteTexture (_Click, IN.texcoord);// * IN.color;
    fixed4 inactive = SampleSpriteTexture (_Inactive, IN.texcoord);// * IN.color;
    fixed4 a = lerp(base,hover,_Mix.x);
    fixed4 b = lerp(a,click,_Mix.y);
    fixed4 c = saturate(lerp(b,inactive,_Mix.z))*IN.color*IN.color.a;
    //float4 wCol = float4(fixSin(IN.wPos.x+_Time.x),fixSin(IN.wPos.x+_Time.y),fixSin(IN.wPos.z+_Time.x),1);
    return c*c.a;//*wCol;
}

#endif // UNITY_SPRITES_INCLUDED
