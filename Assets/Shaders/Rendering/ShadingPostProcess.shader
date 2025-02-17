Shader "Hidden/ShadingPostProcess"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    ENDHLSL
    
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        ZWrite Off Cull Off
        
        Pass
        {
            Name "ShadePass"
            
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_TARGET
            {
                return float4(1, 0, 0, 1);
            }

            ENDHLSL
        }
    }
}