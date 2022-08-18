using UnityEngine;
using UnityEngine.Rendering;

namespace TestRenderPipeline.Runtime.Scripts
{
    public class CustomRenderPipeline : RenderPipeline
    {
        void Render(ScriptableRenderContext pContext, Camera pCamera)
        {
            
            //设置视图投影矩阵。该转换矩阵将摄像机的位置和方向（视图矩阵）与摄像机的透视或正投影（投影矩阵）结合在一起。
            pContext.SetupCameraProperties(pCamera);

            //命令缓冲区
            var _cmdBuffer = new CommandBuffer(){name = pCamera.name};
            var _cameraClearFlags = pCamera.clearFlags;
            
            //根据摄像机CameraClearFlags设置清楚参数
            _cmdBuffer.ClearRenderTarget((_cameraClearFlags & CameraClearFlags.Depth) != 0, 
                (_cameraClearFlags & CameraClearFlags.Color) != 0, 
                pCamera.backgroundColor);
            
            pContext.ExecuteCommandBuffer(_cmdBuffer);
            _cmdBuffer.Release();

            //绘制天空盒
            pContext.DrawSkybox(pCamera);


            pContext.Submit();
        }

        protected override void Render(ScriptableRenderContext pContext, Camera[] pCameras)
        {
            foreach (var camera in pCameras)
            {
                Render(pContext, camera);
            }
        }
    }
}