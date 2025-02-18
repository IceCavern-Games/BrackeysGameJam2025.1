Shader "Hidden/ShadingPostProcess"
{
    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

    static const int RobertsCrossX[4] = { 1, 0, 0, -1 };
    static const int RobertsCrossY[4] = { 0, 1, -1, 0 };

    static const float DITHER_THRESHOLDS[16] =
    {
        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
    };

    float4 GetZBufferParam()
    {
        float n = _ProjectionParams.y;
        float f = _ProjectionParams.z;
        float oneOverF = _ProjectionParams.w;

        #if UNITY_REVERSED_Z
            return float4(f/n - 1, 1, 1/n - oneOverF, oneOverF);
        #else
            return float4(1 - f/n, f/n, 1/f - 1/n, 1/n);
        #endif
    }

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

            #pragma vertex VertWithView
            #pragma fragment Frag

            float _WhiteCutoff;
            float _NormalThreshold;
            float _DepthThreshold;
            float _DepthDistanceModulation;
            float _DepthModulationPower;
            float _DitherSize;

            sampler2D _DitherTexture;
            float4 _DitherTextureSize;


            struct VaryingsWithView
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord   : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float CalculateEdge(float2 uv, float2 offset)
            {
                float3 samples[4];
                samples[0] = SampleSceneNormals(uv - offset, sampler_PointClamp);
                samples[1] = SampleSceneNormals(uv + float2(offset.x, -offset.y), sampler_PointClamp);
                samples[2] = SampleSceneNormals(uv + float2(-offset.x, offset.y), sampler_PointClamp);
                samples[3] = SampleSceneNormals(uv + offset, sampler_PointClamp);

                float3 horizontal = samples[0] * RobertsCrossX[0];
                horizontal += samples[3] * RobertsCrossX[3];

                float3 vertical = samples[2] * RobertsCrossY[2];
                vertical += samples[1] * RobertsCrossY[1];

                return sqrt(dot(horizontal, horizontal) + dot(vertical, vertical));
            }

            float CalculateEdgeDepth(float2 uv, float2 offset, out float depth)
            {
                float samples[4];
                samples[0] = SampleSceneDepth(uv - offset);
                samples[1] = SampleSceneDepth(uv + float2(offset.x, -offset.y));
                samples[2] = SampleSceneDepth(uv + float2(-offset.x, offset.y));
                samples[3] = SampleSceneDepth(uv + offset);

                float horizontal = samples[0] * RobertsCrossX[0];
                horizontal += samples[3] * RobertsCrossX[3];

                float vertical = samples[2] * RobertsCrossY[2];
                vertical += samples[1] * RobertsCrossY[1];

                depth = Linear01Depth(max(max(max(samples[0], samples[1]), samples[2]), samples[3]), GetZBufferParam());

                return sqrt(dot(horizontal, horizontal) + dot(vertical, vertical));
            }

            float ModulateDepth(float2 uv, float3 viewDir)
            {
                float3 normal = SampleSceneNormals(uv);

                float modulationFactor = _DepthDistanceModulation * SampleSceneDepth(uv);

                float nDotV = 1 - dot(normal, viewDir);
                float threshold = saturate((nDotV - _DepthThreshold) / (1 - _DepthThreshold));
                modulationFactor *= threshold * _DepthModulationPower + 1;

                return modulationFactor;
            }

            float4 TriplanarMapping(sampler2D source, float3 normal, float3 position, float tile, float blend)
            {
                float3 uv = position * tile;
                float3 blendResult = pow(abs(normal), blend);
                float4 xPlane = tex2D(source, uv.zy);
                float4 yPlane = tex2D(source, uv.xz);
                float4 zPlane = tex2D(source, uv.xy);
                return xPlane * blendResult.x + yPlane * blendResult.y + zPlane * blendResult.z;
            }

            VaryingsWithView VertWithView(Attributes input)
            {
                VaryingsWithView output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

                output.viewDir = mul(unity_CameraInvProjection, pos.xyzz).xyz;
                output.viewDir.x *= -1;
                output.viewDir = mul(unity_CameraToWorld, output.viewDir).xyz;
                output.positionCS = pos;
                output.texcoord   = DYNAMIC_SCALING_APPLY_SCALEBIAS(uv);

                return output;
            }

            float4 Frag(VaryingsWithView input) : SV_TARGET
            {
                // do edge detection
                float modulationFactor = ModulateDepth(input.texcoord.xy, input.viewDir);
                _DepthThreshold *= modulationFactor;
                float3 edgeColor = float3(0, 0, 0);

                float2 offsetNormal = float2(1 / _CameraNormalsTexture_TexelSize.z, 1 / _CameraNormalsTexture_TexelSize.w);
                float2 offsetDepth = float2(1 / _CameraDepthTexture_TexelSize.z, 1 / _CameraDepthTexture_TexelSize.w);

                float depth;
                edgeColor.r = CalculateEdge(input.texcoord, offsetNormal) > _NormalThreshold ? 1 : 0;
                edgeColor.g = CalculateEdgeDepth(input.texcoord, offsetDepth, depth) > _DepthThreshold ? 1 : 0;
                edgeColor.b = edgeColor.r > 0 || edgeColor.g > 0 ? 1 : 0;

                float4 camColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                float luma = dot(camColor.rgb, float3(0.2126729, 0.7151522, 0.0721750));
                // dither cam color
                float3 worldPos = ComputeWorldSpacePosition(input.texcoord, SampleSceneDepth(input.texcoord), UNITY_MATRIX_I_VP);
                float3 ditherTex = TriplanarMapping(_DitherTexture, SampleSceneNormals(input.texcoord.xy), worldPos, _DitherSize, 1);
                //float3 ditherTex = tex2D(_DitherTexture, worldPos.xy * _DitherSize /* * _ScreenParams.xy*/ / _DitherTextureSize.xy);
                float4 dithered = float4(luma.xxx * ditherTex, 1);
                
                float edge = step(_WhiteCutoff, edgeColor.b);
                float ditheredFinal = step(_WhiteCutoff, dithered / (1 - luma));

                if (edge > 0)
                    return float4(edge.xxx, 1);
                
                
                return float4(ditheredFinal.xxx * camColor.rgb, 1);
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
