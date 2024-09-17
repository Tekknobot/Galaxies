Shader "Custom/Pixelation"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _PixelSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 pixelUV = floor(i.uv * _PixelSize) / _PixelSize;
                return tex2D(_MainTex, pixelUV);
            }
            ENDCG
        }
    }
}
