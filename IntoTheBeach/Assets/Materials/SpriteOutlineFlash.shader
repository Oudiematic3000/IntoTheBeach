Shader "Custom/SpriteOutlineFlash"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 1
        _FlashAmount ("Flash Amount", Float) = 0
        _Alpha ("Alpha", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize; 
                float4 _OutlineColor;
                float _OutlineThickness;
                float _FlashAmount;
                float _Alpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 mainSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float mainAlpha = mainSample.a;

                float2 texel = _MainTex_TexelSize.xy * _OutlineThickness;

                float a1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( texel.x,  0      )).a;
                float a2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-texel.x,  0      )).a;
                float a3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( 0,       texel.y )).a;
                float a4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( 0,      -texel.y )).a;
                float a5 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( texel.x,  texel.y)).a;
                float a6 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-texel.x,  texel.y)).a;
                float a7 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2( texel.x, -texel.y)).a;
                float a8 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-texel.x, -texel.y)).a;

                float outlineMask = max(max(max(a1, a2), max(a3, a4)), max(max(a5, a6), max(a7, a8)));

                float outlineOnly = step(0.01, outlineMask - mainAlpha);

                float4 flashedColor = lerp(mainSample, float4(1,1,1,1), _FlashAmount);

                float4 maskedSprite = flashedColor * mainAlpha;

                float4 maskedOutline = _OutlineColor * outlineOnly;

                float4 finalColor = maskedSprite + maskedOutline;
                float finalAlpha = max(mainAlpha, outlineMask);

                finalColor *= IN.color;
                finalColor.a = finalAlpha * IN.color.a * _Alpha;

                return finalColor;
            }
            ENDHLSL
        }
    }
}