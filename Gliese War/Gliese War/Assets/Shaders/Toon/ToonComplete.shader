Shader "Custom/Banded_5"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		[HDR]
		_AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
		[HDR]
		_SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
		_Glossiness("Glossiness", Float) = 32
		[HDR]
		_RimColor("Rim Color", Color) = (1,1,1,1)
		_RimAmount("Rim Amount", Range(0, 1)) = 0.716
		_RimThreshold("Rim Threshold", Range(0, 1)) = 0.1

			_OutlineExtrusion("Outline Extrusion", float) = 0.02
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
	}
		SubShader
		{
				 Tags { "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" } 

			Pass
			{
				Tags
			{
				"LightMode" = "UniversalForward"
			}
			ZWrite On

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 worldNormal : NORMAL;
					float2 uv : TEXCOORD0;
					float3 viewDir : TEXCOORD1;
					SHADOW_COORDS(2)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.viewDir = WorldSpaceViewDir(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					TRANSFER_SHADOW(o)
					return o;
				}

				float4 _Color;

				float4 _AmbientColor;

				float4 _SpecularColor;
				float _Glossiness;

				float4 _RimColor;
				float _RimAmount;
				float _RimThreshold;

				float4 frag(v2f i) : SV_Target
				{
					float3 normal = normalize(i.worldNormal);
					float3 viewDir = normalize(i.viewDir);

					float NdotL = dot(_WorldSpaceLightPos0, normal);

					float shadow = SHADOW_ATTENUATION(i);
					float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);
					float4 light = lightIntensity * _LightColor0;

					float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
					float NdotH = dot(normal, halfVector);

					float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
					float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
					float4 specular = specularIntensitySmooth * _SpecularColor;

					float rimDot = 1 - dot(viewDir, normal);
					float rimIntensity = rimDot * pow(NdotL, _RimThreshold);
					rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
					float4 rim = rimIntensity * _RimColor;

					float4 sample = tex2D(_MainTex, i.uv);

					return (light + _AmbientColor + specular + rim) * _Color * sample;
				}
					ENDCG
			}
			
			Pass {

			Name "Outline"
				Cull Front
            
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				float4 _OutlineColor;
				float _OutlineExtrusion;

				struct vertexInput
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float4 color : COLOR;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					float4 newPos = input.vertex;

					// normal extrusion technique
					float3 normal = normalize(input.normal);
					newPos += float4(normal, 0.0) * _OutlineExtrusion;

					// convert to world space
					output.pos = UnityObjectToClipPos(newPos);

					output.color = _OutlineColor;
					return output;
				}

				float4 frag(vertexOutput input) : COLOR
				{
					return input.color;
				}


				ENDCG
			}
				Pass
				{
					Name  "TransparentPass"
					Tags {"LightMode" = "UniversalForward"}
					ZWrite Off
					Blend SrcAlpha OneMinusSrcAlpha

					HLSLPROGRAM

					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

					#pragma prefer_hlslcc gles   //  GLES 2.0 호환
					#pragma exclude_renderers d3d11_9x  // dx9.0 호환 제거

					#pragma vertex vert
					#pragma fragment frag


					struct appdata
					{
						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
					};

					struct v2f
					{
						float4 vertex : SV_POSITION;
						float2 uv : TEXCOORD0;
					};


					sampler2D _MainTex;

					// SRP Batcher
					CBUFFER_START(UnityPerMaterial)
						float4 _MainTex_ST;
						float4 _Color;
					CBUFFER_END


					v2f vert(appdata v)
					{
						v2f o;
						o.vertex = TransformObjectToHClip(v.vertex.xyz);
						o.uv = v.uv;

						return o;
					}

					float4 frag(v2f i) : SV_Target
					{
						float2 mainTexUV = i.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
						float4 col = tex2D(_MainTex, mainTexUV) * _Color;

						return col;
					}

					ENDHLSL
				}

UsePass"Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}