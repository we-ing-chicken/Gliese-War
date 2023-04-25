Shader "Custom/Outline"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}

        _OutlineTex("Outline Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1,1,1,1) //¿Ü°û¼± »ö±ò ÁöÁ¤
        _OutlineWidth("Outline Width", Range(1.0,10.0)) = 1.1 // ¿Ü°û¼± µÎ²²
    }
    SubShader
    {
        Tags { "Queue" = "transparent"  } //"RenderType"="Opaque"
        LOD 100

        cull front

        Pass
        {
            Name "OUTLINE"

            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            sampler2D _OutlineTex;

            v2f vert (appdata IN)
            {
                IN.vertex.xyz *= _OutlineWidth;
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                float4 texColor = tex2D(_OutlineTex, IN.uv);
                return texColor * _OutlineColor;
            }
            ENDCG
        }
        
    }
}
