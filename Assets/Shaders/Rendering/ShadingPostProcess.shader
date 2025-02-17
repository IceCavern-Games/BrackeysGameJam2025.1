Shader "Hidden/ShadingPostProcess"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float3 grayscaleDot = float3(0.2126729, 0.7151522, 0.0721750);
    
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

            float _WhiteCutoff;

            float4 Frag(Varyings input) : SV_TARGET
            {
                float4 camColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float luma = dot(camColor.rgb, float3(0.2126729, 0.7151522, 0.0721750));

                float output = step(_WhiteCutoff, luma);
                
                return float4(output.xxx, 1);
            }

            ENDHLSL
        }

        Pass
        {
            Name "Copy to Color"
            
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag(Varyings input) : SV_TARGET
            {
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
            }

            ENDHLSL
        }
    }
}