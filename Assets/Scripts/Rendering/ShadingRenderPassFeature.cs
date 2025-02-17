using UnityEngine;
using System;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class ShadingRenderPassFeature : ScriptableRendererFeature
{
    [Serializable]
    public class ShadingSettings
    {
        [Range(0, 1)] public float WhiteCutoff = 0.8f;
        [Range(0, 2)] public float NormalThreshold = 0.0f;
        [Range(0, 1)] public float DepthThreshold = 0.0f;
        public float DepthDistanceModulation = 1.0f;
        public float DepthModulationPower = 1.0f;
    }

    [SerializeField] private ShadingSettings _settings;
    [SerializeField] private Shader _shader;
    
    class CustomShadingRenderPass : ScriptableRenderPass
    {
        // Shader Properties
        private static readonly int _whiteCutoffId = Shader.PropertyToID("_WhiteCutoff");
        private static readonly int _NormalThresholdId = Shader.PropertyToID("_NormalThreshold");
        private static readonly int _DepthThresholdId = Shader.PropertyToID("_DepthThreshold");
        private static readonly int _DepthDistanceModulationId = Shader.PropertyToID("_DepthDistanceModulation");
        private static readonly int _DepthModulationPowerId = Shader.PropertyToID("_DepthModulationPower");

        
        // Pass Resources
        private Material _material;
        private ShadingSettings _settings;
        private RenderTextureDescriptor _rtDescriptor;
        
        public CustomShadingRenderPass(Material material, ShadingSettings settings)
        {
            _material = material;
            _settings = settings;
            _rtDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        }
        
        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
            internal TextureHandle _cameraColor;
        }

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        private void ExecutePass(PassData data, RasterGraphContext context)
        {
            _material.SetFloat(_whiteCutoffId, _settings.WhiteCutoff);
            _material.SetFloat(_NormalThresholdId, _settings.NormalThreshold);
            _material.SetFloat(_DepthThresholdId, _settings.DepthThreshold);
            _material.SetFloat(_DepthDistanceModulationId, _settings.DepthDistanceModulation);
            _material.SetFloat(_DepthModulationPowerId, _settings.DepthModulationPower);
            
            Blitter.BlitTexture(context.cmd, data._cameraColor, new Vector4(1, 1, 0, 0), _material, 0);
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "ShadePass";
            
            // Make use of frameData to access resources and camera data through the dedicated containers.
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            TextureHandle dst =
                UniversalRenderer.CreateRenderGraphTexture(renderGraph, _rtDescriptor, "_FinalColor", false);
            TextureHandle cameraColor = resourceData.activeColorTexture;

            // This adds a raster render pass to the graph, specifying the name and the data type that will be passed to the ExecutePass function.
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                // Use this scope to set the required inputs and outputs of the pass and to
                // setup the passData with the required properties needed at pass execution time.

                passData._cameraColor = cameraColor;
                
                // Setup pass inputs and outputs through the builder interface.
                // Eg:
                // builder.UseTexture(sourceTexture);
                // TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraData.cameraTargetDescriptor, "Destination Texture", false);
                
                // This sets the render target of the pass to the active color texture. Change it to your own render target as needed.
                builder.SetRenderAttachment(dst, 0);

                // Assigns the ExecutePass function to the render pass delegate. This will be called by the render graph when executing the pass.
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }

            RenderGraphUtils.BlitMaterialParameters blitParams = new (dst, cameraColor, _material, 1);
            renderGraph.AddBlitPass(blitParams, "Copy to Color");
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
        }

        // NOTE: This method is part of the compatibility rendering path, please use the Render Graph API above instead.
        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomShadingRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        if (_shader == null) return;

        Material material = new Material(_shader);
        m_ScriptablePass = new CustomShadingRenderPass(material, _settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_ScriptablePass == null) return;

        if (renderingData.cameraData.camera.cameraType != CameraType.Game) return;
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
