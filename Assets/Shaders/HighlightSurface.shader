Shader "Custom/HighlightSurfaceShader"
{
    Properties
    {
        _HighlightColor("Highlight Color", Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags{ "Highlight" = "On" }

        Pass
        {    
            ZWrite Off
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float4 _HighlightColor;

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            struct FragmentInput
            {
                float4 position : SV_POSITION;
            };

            FragmentInput vert(appdata_base v)
            {
                FragmentInput o;
                o.position = UnityObjectToClipPos(v.vertex);

                return o;
            }

            half4 frag(FragmentInput i) : COLOR
            {
                return _HighlightColor;
            }

            ENDCG
        }
    }
}
