Shader "Custom/Mirror" 
{

	Properties
	{

		_MainTex("Texture", 2D) = "white" {}

		_BumpMap("Bumpmap", 2D) = "bump" {}

		_Metalic("Metalic", Range(0,1)) = 0.5

		_MetalicMap("Metalicmap", 2D) = "white" {}

	}

		SubShader
	{

			Tags{ "RenderType" = "Opaque" }

			CGPROGRAM

			#pragma surface surf FakeMetal noshadow nolightmap 

			struct Input {

				float2 uv_MainTex;

				float2 uv_BumpMap;

				float3 worldRefl;

				INTERNAL_DATA

			};



	struct SurfaceOutputCustom
	{

		fixed3 Albedo;  // diffuse color

		fixed3 Normal;  // tangent space normal, if written

		fixed Alpha;    // alpha for transparencies

		fixed3 Emission;

		fixed3 Metalic;

	};



	sampler2D _MainTex;

	sampler2D _BumpMap;

	sampler2D _MetalicMap;

	fixed _Metalic;



	half4 LightingFakeMetal(SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten) {

		half NdotL = saturate(dot(s.Normal, lightDir));

		half diff = NdotL * 0.5 + 0.5;	// Half-Lambert

		half4 c;

		c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);

		c.a = s.Alpha;

		c.rgb *= 1 - s.Metalic;

		return c;

	}



	void surf(Input IN, inout SurfaceOutputCustom o)
	{

		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

		float3 rv = WorldReflectionVector(IN, o.Normal).xyz;

		o.Emission = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, rv).rgb * unity_SpecCube0_HDR.r;


		fixed3 blendTarget = UNITY_SAMPLE_TEXCUBE(unity_SpecCube1, rv).rgb * unity_SpecCube1_HDR.r;

		o.Emission = lerp(blendTarget, o.Emission, unity_SpecCube0_BoxMin.w);


		o.Metalic = tex2D(_MetalicMap, IN.uv_MainTex).rgb * _Metalic;

		o.Emission.rgb *= o.Metalic.rgb;

	}

		ENDCG

	}

		Fallback "Diffuse"

}