using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TestRenderPipeline.Runtime.Scripts
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private CameraRenderer _cameraRenderer = new CameraRenderer();

        protected override void Render(ScriptableRenderContext pContext, Camera[] pCameras)
        {
            foreach (var camera in pCameras)
            {
                _cameraRenderer.Render(pContext, camera);
            }
        }
    }
}