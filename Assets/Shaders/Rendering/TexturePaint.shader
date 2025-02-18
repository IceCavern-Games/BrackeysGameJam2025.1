Shader "Unlit/uvIsland"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD	   100
		ZTest  Off
		ZWrite Off
		Cull   Off

        Pass
        {
            Name "UVRemapPass"
            
            HLSLPROGRAM

            // #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #pragma vertex Vert
            #pragma fragment Frag

            sampler2D _MaskTex;
            float3 _PainterPosition;
            float _Radius;
            float _Hardness;
            float _Strength;
            float4 _PainterColor;
            float _PrepareUV;

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            float mask(float3 position, float3 center, float radius, float hardness)
            {
                float m = distance(center, position);
                return 1 - smoothstep(radius * hardness, radius, m);
            }

            Varyings Vert(Attributes input)
            {
                Varyings o;

                o.worldPos = mul(unity_ObjectToWorld, input.vertex);
                o.texcoord = input.texcoord;
                float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (input.texcoord.xy * float2(2, 2) - float2(1, 1));
                o.vertex = uv;
                
                return o;
            }

            float4 Frag(Varyings input) : SV_TARGET
            {
                if (_PrepareUV > 0)
                    return float4(0, 0, 1, 1);

                float4 color = tex2D(_MaskTex, input.texcoord);
                float f = mask(input.worldPos, _PainterPosition, _Radius, _Hardness);
                float edge = f * _Strength;
                return lerp(color, _PainterColor, edge);
            }
            
            ENDHLSL
        }
    }
}