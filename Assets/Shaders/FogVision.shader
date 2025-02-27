Shader "Custom/FogVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VisionRadius ("Vision Radius", Range(0, 1)) = 0.3
        _PlayerPos ("Player Position", Vector) = (0.5, 0.5, 0, 0)
        _FogColor ("Fog Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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

            sampler2D _MainTex;
            float _VisionRadius;
            float2 _PlayerPos;
            float4 _FogColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, _PlayerPos);
                float circle = smoothstep(_VisionRadius - 0.1, _VisionRadius, dist);
                return float4(_FogColor.rgb, circle * _FogColor.a);
            }
            ENDCG
        }
    }
} 