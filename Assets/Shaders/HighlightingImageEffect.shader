﻿Shader "Hidden/HighlightingImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AdditionalTex("Additional Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _AdditionalTex;

            float2 _MainTex_TexelSize;

            fixed4 frag (v2f i) : SV_Target
            {
                int NumberOfIterations = 12;

                //split texel size into smaller words
                float step = _MainTex_TexelSize.x;

                //and a final intensity that increments based on surrounding intensities.
                float4 intensity = 0.0;

                //for every iteration we need to do horizontally
                for (int k = 0; k < NumberOfIterations; ++k)
                {
                    //increase our output color by the pixels in the area
                    float offset = (k - NumberOfIterations * 0.5);
                    float2 uv = i.uv.xy + float2(offset * step * 0.25, 0);
                    intensity += tex2D(_AdditionalTex, uv) / NumberOfIterations;
                }

                intensity = abs(intensity);
                return intensity;
            }
            ENDCG
        }

        GrabPass {}

        Pass
        {
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _AdditionalTex;

            float2 _MainTex_TexelSize;

            sampler2D _GrabTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                int NumberOfIterations = 12;

                //split texel size into smaller words
                float step = _MainTex_TexelSize.y;

                //and a final intensity that increments based on surrounding intensities.
                float4 intensity = 0.0;

                //for every iteration we need to do horizontally
                for (int k = 0; k < NumberOfIterations; ++k)
                {
                    //increase our output color by the pixels in the area
                    float offset = (k - NumberOfIterations * 0.5);
                    float2 uv = i.uv.xy + float2(0, offset * step * 0.25);
                    intensity += tex2D(_GrabTexture, uv) * offset / NumberOfIterations;
                }
                intensity = abs(intensity);

                return 0.1f * tex2D(_AdditionalTex, i.uv.xy) + lerp(tex2D(_MainTex, i.uv.xy), intensity, intensity.a);
            }
            ENDCG
        }
    }
}
