Shader "Custom/BlurShader"
{
    Properties
    {
        _Radius("Radius", Range(0, 1)) = 0
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
    }

        Category
    {
        Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent" }

        SubShader
        {
            Tags{ "LightMode" = "UniversalForward" }
            LOD 100

                                Pass
                                {

                                    CGPROGRAM
                                    #pragma vertex vert
                                    #pragma fragment frag
                                    #pragma fragmentoption ARB_precision_hint_fastest
                                    #include "UnityCG.cginc"

                                    struct appdata_t
                                    {
                                        float4 vertex : POSITION;
                                        float2 texcoord: TEXCOORD0;
                                    };

                                    struct v2f
                                    {
                                        float4 vertex : POSITION;
                                        float4 uvgrab : TEXCOORD0;
                                    };

                                    v2f vert(appdata_t v)
                                    {
                                        v2f o;
                                        o.vertex = UnityObjectToClipPos(v.vertex);
                                        #if UNITY_UV_STARTS_AT_TOP
                                        float scale = -1.0;
                                        #else
                                        float scale = 1.0;
                                        #endif
                                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                                        o.uvgrab.zw = o.vertex.zw;
                                        return o;
                                    }

                                    CBUFFER_START(UnityPerMaterial)

                                    sampler2D _MainTex;
                                    float4 _MainTex_ST;
                                    float _Radius;
                                    float4 _Color;

                                    CBUFFER_END

                                    half4 frag(v2f i) : COLOR
                                    {

                                        half4 sum = half4(0,0,0,0);
                                        float radius = 1.41421356237 * _Radius;

                                        #define GRABXYPIXEL(kernelx, kernely) tex2Dproj( _MainTex, UNITY_PROJ_COORD(float4(i.uvgrab.x + _MainTex_ST.x * kernelx, i.uvgrab.y + _MainTex_ST.y * kernely, i.uvgrab.z, i.uvgrab.w)))

                                        sum += GRABXYPIXEL(0.0, 0.0);
                                        int measurments = 1;

                                        for (float range = 1.41421356237f; range <= radius * 1.41; range += 1.41421356237f)
                                        {
                                            sum += GRABXYPIXEL(range, 0.0);
                                            sum += GRABXYPIXEL(-range, 0.0);
                                            sum += GRABXYPIXEL(0.0, range);
                                            sum += GRABXYPIXEL(0.0, -range);
                                            measurments += 4;
                                        }

                                        return _Color * sum / measurments;
                                    }
                                    ENDCG
                                }
                            }
    }
}