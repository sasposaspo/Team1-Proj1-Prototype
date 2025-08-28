using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public RTHandle source;

        private Material _material;
        private RTHandle tempRenderTarget;
        private RTHandle tempRenderTarget2;

        public CustomRenderPass(Material mat)
        {
            _material = mat;

            tempRenderTarget = RTHandles.Alloc("_TemporaryColourTexture");
            tempRenderTarget2 = RTHandles.Alloc("_TemporaryDepthTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Not used for now, but kept for clarity
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Reflection)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get();

                // ✅ Use cameraTargetDescriptor from renderingData
                var descriptor = renderingData.cameraData.cameraTargetDescriptor;

                // Ensure temporary RT matches the camera descriptor
                RenderingUtils.ReAllocateIfNeeded(ref tempRenderTarget, descriptor, name: "_TemporaryColourTexture");

                Blit(commandBuffer, source, tempRenderTarget, _material);
                Blit(commandBuffer, tempRenderTarget, source);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (tempRenderTarget != null)
                RTHandles.Release(tempRenderTarget);
            if (tempRenderTarget2 != null)
                RTHandles.Release(tempRenderTarget2);
        }
    }

    [System.Serializable]
    public class _Settings
    {
        public Material material = null;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public _Settings settings = new _Settings();

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = (Material)Resources.Load("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material);
        m_ScriptablePass.renderPassEvent = settings.renderPass;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.source = renderer.cameraColorTargetHandle; // Updated for RTHandle
        renderer.EnqueuePass(m_ScriptablePass);
    }
}