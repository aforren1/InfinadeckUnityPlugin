// Infinadeck reference-object shader.
// Two-pass unlit "see-through-walls" overlay:
//   * the "behind" pass (ZTest Greater) paints _Color1 where the object is occluded by scene geometry
//   * the "front"  pass (ZTest Less/LEqual) paints _Color2 where the object is directly visible
Shader "Unlit/InfRefObjShader"
{
    Properties
    {
        _Color1("Color1 (occluded)", Color) = (1,1,1,1)
        _Color2("Color2 (visible)", Color) = (1,1,1,1)
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
        float4 _Color1;
        float4 _Color2;
    CBUFFER_END

    struct Attributes { float4 positionOS : POSITION; };
    struct Varyings   { float4 positionHCS : SV_POSITION; };

    Varyings vert(Attributes IN)
    {
        Varyings OUT;
        OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
        return OUT;
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry+1" "RenderType" = "Opaque" }
        LOD 100

        // Behind scene geometry -> _Color1
        Pass
        {
            Name "RefObjOccluded"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            ZTest Greater
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag(Varyings IN) : SV_Target { return _Color1; }
            ENDHLSL
        }

        // In front (directly visible) -> _Color2
        Pass
        {
            Name "RefObjVisible"
            Tags { "LightMode" = "UniversalForward" }
            ZTest LEqual
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag(Varyings IN) : SV_Target { return _Color2; }
            ENDHLSL
        }
    }
}
