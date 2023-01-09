Shader "Custom/Banded_Lighting"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
    }
        SubShader
        {
            Tags { "Queue" = "transparent" "LightMode" = "ForwardBase" "PassFlage" = "OnlyDirectional" } //"RenderType"="Opaque"
            LOD 100
            cull back
            Pass
            {
            Name "BANDED_LIGHTING"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
            };

            float4 _Color;
            sampler2D _MainTex;

            v2f vert(appdata IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);

                return OUT;
            }
            float4 _AmbientColor;

            fixed4 frag(v2f IN) : SV_Target
            {
                float3 normal = normalize(IN.worldNormal);
                float NdotL = dot(_WorldSpaceLightPos0, normal);
                //float lightIntensity = NdotL > 0.3 ? 1 : NdotL > -0.3 ? 0.5 : 0;
                float lightIntensity = smoothstep(0, 0.01, NdotL);
                float4 light = lightIntensity * _LightColor0;
                float4 texColor = tex2D(_MainTex, IN.uv);

                return _Color * (_AmbientColor + light);
            }

            ENDCG
        }
        }
}
