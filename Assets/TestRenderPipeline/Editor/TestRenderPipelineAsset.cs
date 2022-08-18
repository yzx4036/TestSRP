using TestRenderPipeline.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Rendering;

namespace TestRenderPipeline.Editor
{
    [CreateAssetMenu(fileName = "TestRenderPipelineAsset", menuName = "Rendering/TestRenderPipeline")]
    public class TestRenderPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new CustomRenderPipeline();
        }
    }
}
