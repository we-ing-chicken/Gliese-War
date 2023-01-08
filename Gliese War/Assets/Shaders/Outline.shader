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
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
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

                //UNITY_TRANSFER_FOG(OUT, OUT.vertex);
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // sample the texture
                float4 texColor = tex2D(_OutlineTex, IN.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return texColor * _OutlineColor;
            }
            ENDCG
        }
        
        Pass
        {
            Name "OBJECT"

            CGPROGRAM // Allows talk between two languages : shader lab and nvidia C for graphics

            #pragma vertex vert // Define for the building function
            #pragma fragment frag // Define for coloring function

            #include "UnityCG.cginc" // Built in shader functions

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

            float4 _Color;
            sampler2D _MainTex;

            v2f vert(appdata IN)
            {
                v2f OUT;

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, IN.uv);

                return texColor * _Color;
            }

            ENDCG
        }
    }
}
