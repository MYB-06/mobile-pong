Shader "Custom/trailshader"
{
    Properties
    {
        _Color ("Trail Color", Color) = (0.5, 0.8, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 3
        _FadePower ("Fade Power", Range(0.5, 5)) = 2
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        Blend SrcAlpha One
        ZWrite Off
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _GlowIntensity;
                float _FadePower;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float fade = pow(1.0 - IN.uv.x, _FadePower);
                
                float glow = fade * _GlowIntensity;
                
                half4 col = _Color * IN.color * glow;
                col.a *= fade * IN.color.a;
                
                return col;
            }
            ENDHLSL
        }
    }
}
