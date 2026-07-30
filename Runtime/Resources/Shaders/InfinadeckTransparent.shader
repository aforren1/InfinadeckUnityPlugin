// Infinadeck transparent cutout shader (reference-panel symbols).
// Unlit transparent: RGB = _MainTex * _Color; alpha is gated by a second texture's alpha
// (_CutTex.a > _Cutoff ? _MainTex.a : 0). _MainTex and _CutTex have independent tiling/offset
// (driven at runtime via Material.SetTextureOffset), so both _MainTex_ST and _CutTex_ST are used.
Shader "Transparent/Cutout/TransparentInf"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _CutTex("Cutout (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }

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
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_CutTex);  SAMPLER(sampler_CutTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _CutTex_ST;
                half4 _Color;
                float _Cutoff;
            CBUFFER_END

            // Stereo rendering (Single Pass Instanced) needs the instance/eye-index
            // macros, or the object only draws in one eye.
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uvMain : TEXCOORD0;
                float2 uvCut : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
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
