// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NormalMapShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_LightColor("LightColor", Color) = (1,1,1,1)
        _AmbientColor("Ambient Color", Color) = (1,1,1,1)
        _LightDropoff("Light Dropoff", float) = 1
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SpecularMap("SpecularMap (RGB)", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "white" {}
		_MainLightPosition("MainLightPosition", Vector) = (0,0,0,0)
	}
	SubShader{
		Pass{
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 200
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vs_main
			#pragma fragment fs_main

			struct VS_IN
			{
				float4 position : POSITION;

				float3 normal : NORMAL;
				float3 tangent : TANGENT;
				// float3 binormal : BINORMAL; // unity does not support BINORMAL semantic?
				float2 uv : TEXCOORD0;
			};

			struct VS_OUT
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float3 lightdir : TEXCOORD1;
				float3 viewdir : TEXCOORD2;
                float4 angle : COLOR;
				float3 T : TEXCOORD3;
				float3 B : TEXCOORD4;
				float3 N : TEXCOORD5;

				// TANGENT, BINORMAL, NORMAL semantics are only available for input of vertex shader
			};

			uniform float4 _Color;
			uniform float4 _LightColor;
            float4 _AmbientColor;
            float _LightDropoff;

			uniform float3 _MainLightPosition;

			uniform sampler _MainTex;
			uniform sampler _SpecularMap;
			uniform sampler _NormalMap;

			VS_OUT vs_main(VS_IN input)
			{
				VS_OUT output;

				// calc output position directly
				output.position = UnityObjectToClipPos(input.position);

				// pass uv coord
				output.uv = input.uv;
				
				// calc lightDir vector heading current vertex
				float4 worldPosition = mul(unity_ObjectToWorld, input.position);
				float3 lightDir = worldPosition.xyz - _MainLightPosition.xyz;
				output.lightdir = normalize(lightDir);

				// calc viewDir vector 
				float3 viewDir = normalize(worldPosition.xyz - float3(0,0,-1000));// _WorldSpaceCameraPos.xyz);
				output.viewdir = viewDir;

				// calc Normal, Binormal, Tangent vector in world space
				// cast 1st arg to 'float3x3' (type of input.normal is 'float3')
				float3 worldNormal = mul((float3x3)unity_ObjectToWorld, input.normal);
				float3 worldTangent = mul((float3x3)unity_ObjectToWorld, input.tangent);
				
				float3 binormal = cross(input.normal, input.tangent.xyz); // *input.tangent.w;
				float3 worldBinormal = mul((float3x3)unity_ObjectToWorld, binormal);

                output.angle = float4(0,0,0,0);
                float3 wp = mul(unity_ObjectToWorld,float3(-1,-1,-1)).xyz;
                float3 wo = mul(unity_ObjectToWorld,float3(1,1,1)).xyz;
                float3 m = wp-wo;
                output.angle.xyz = worldPosition.xyz;
                output.angle.w = atan2(m.x,m.y);

				// and, set them
				output.N = normalize(worldNormal);
				output.T = normalize(worldTangent);
				output.B = normalize(worldBinormal);
			
				return output;
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

			float4 fs_main(VS_OUT input) : COLOR
			{
				// obtain a normal vector on tangent space
				float3 tangentNormal = tex2D(_NormalMap, input.uv).xyz;
                float oColor = tangentNormal.b;
				// and change range of values (0 ~ 1)
				tangentNormal = normalize(tangentNormal * 2 - 1);

				// 'TBN' transforms the world space into a tangent space
				// we need its inverse matrix
				// Tip : An inverse matrix of orthogonal matrix is its transpose matrix
				float3x3 TBN = float3x3(normalize(input.T), normalize(input.B*-1), normalize(input.N));
				TBN = transpose(TBN);

                float a = 0;//(input.angle.w)/10000000;
                float3x3 rot = rotationMatrix(float3(0,0,1),a);//*-6.28*2+3.1415/2);
				// finally we got a normal vector from the normal map
				float3 worldNormal = mul(TBN,tangentNormal);//mul(rot,tangentNormal));


				// finally we got a normal vector from the normal map
				// float3 worldNormal = mul(TBN, tangentNormal);

				// Lambert here (cuz we're calculating Normal vector in this pixel shader)
				float4 albedo = tex2D(_MainTex, input.angle.xy*_Color.a*5);
				float3 lightDir = normalize(_MainLightPosition-input.angle.xyz);//_MainLightPosition.xyz);//
                float3 shadowDir = normalize(input.angle.xyz-_MainLightPosition);
				// calc diffuse, as we did in pixel shader
				float3 diffuse = saturate(dot(worldNormal, lightDir));
                float3 shadow = saturate(dot(worldNormal, shadowDir));
                float lightDistance = length(_MainLightPosition-input.angle.xyz);
				diffuse = _LightColor * max(0,diffuse-lightDistance*_LightDropoff);

				// // Specular here
				// float3 specular = 0;
				// if (diffuse.x > 0) {
				// 	float3 reflection = reflect(lightDir, worldNormal);
				// 	float3 viewDir = normalize(input.viewdir);

				// 	specular = saturate(dot(reflection, -viewDir));
				// 	specular = pow(specular, 20.0f);

				// 	float4 specularIntensity = tex2D(_SpecularMap, input.uv);
				// 	specular *= _LightColor * specularIntensity;
				// }

				// make some ambient,
				// float3 ambient = float3(0.1f, 0.1f, 0.1f) * 3 * albedo;

				// combine all of colors
                float4 oCol = (_Color * float4(diffuse, 1)+_AmbientColor)*saturate(float4(1-shadow,1));
                oCol.a = saturate(oColor*4);
				return albedo*oCol;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}