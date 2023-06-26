Shader "Custom/SolidTransparent"
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
            Name  "FrontPass"
            Tags {"LightMode" = "SRPDefaultUnlit"}
            ZWrite On
            ColorMask 0
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
Pass {

			Name "Outline"
				Cull Front
            Tags {"LightMode" = "UniversalForward"}
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

    }
}