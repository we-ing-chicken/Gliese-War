Shader "Custom/Banded_2"
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
	}
		SubShader
		{
			Pass
			{
				Tags
				{
					"LightMode" = "ForwardBase"
					"PassFlags" = "OnlyDirectional"
				}

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			#pragma multi_compile_fwdbase

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

			v2f vert(appdata IN)
			{
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
				OUT.viewDir = WorldSpaceViewDir(IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				TRANSFER_SHADOW(OUT)
				return OUT;
			}

			float4 _Color;

			float4 _AmbientColor;

			float4 _SpecularColor;
			float _Glossiness;

			float4 _RimColor;
			float _RimAmount;
			float _RimThreshold;

			float4 frag(v2f IN) : SV_Target
			{
				float3 normal = normalize(IN.worldNormal);
				float3 viewDir = normalize(IN.viewDir);

				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = SHADOW_ATTENUATION(IN);
				float lightIntensity = NdotL > 0.3 ? 1 : NdotL > -0.3 ? 0.5 : 0;
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

				float4 sample = tex2D(_MainTex, IN.uv);

				return (light + _AmbientColor + rim) * _Color * sample;
			}
			ENDCG
		}

			// Shadow casting support.
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}

