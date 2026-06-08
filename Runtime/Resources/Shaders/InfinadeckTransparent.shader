// Infinadeck transparent cutout shader (reference-panel symbols).
// Unlit transparent: RGB = _MainTex * _Color; alpha is gated by a second texture's alpha
// (_CutTex.a > _Cutoff ? _MainTex.a : 0). _MainTex and _CutTex have independent tiling/offset
// (driven at runtime via Material.SetTextureOffset), so both _MainTex_ST and _CutTex_ST are used.
// Ported from a Built-in surface shader to SRP. Contains URP and HDRP SubShaders.
// NOTE: validate in-editor under both URP and HDRP (the original was lit/Lambert; this is unlit).
Shader "Transparent/Cutout/TransparentInf"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _CutTex("Cutout (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }

    // ----------------------------------------------------------------------------------
    // Universal Render Pipeline
    // ----------------------------------------------------------------------------------
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 200

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_CutTex);  SAMPLER(sampler_CutTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _CutTex_ST;
                half4 _Color;
                float _Cutoff;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uvMain : TEXCOORD0; float2 uvCut : TEXCOORD1; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uvMain = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uvCut  = TRANSFORM_TEX(IN.uv, _CutTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uvMain) * _Color;
                half ca = SAMPLE_TEXTURE2D(_CutTex, sampler_CutTex, IN.uvCut).a;
                c.a = (ca > _Cutoff) ? c.a : 0;
                return c;
            }
            ENDHLSL
        }
    }

    // ----------------------------------------------------------------------------------
    // High Definition Render Pipeline
    // ----------------------------------------------------------------------------------
    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

        Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode" = "ForwardOnly" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_CutTex);  SAMPLER(sampler_CutTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _CutTex_ST;
                half4 _Color;
                float _Cutoff;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uvMain : TEXCOORD0; float2 uvCut : TEXCOORD1; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uvMain = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uvCut  = TRANSFORM_TEX(IN.uv, _CutTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uvMain) * _Color;
                half ca = SAMPLE_TEXTURE2D(_CutTex, sampler_CutTex, IN.uvCut).a;
                c.a = (ca > _Cutoff) ? c.a : 0;
                return c;
            }
            ENDHLSL
        }
    }
}
