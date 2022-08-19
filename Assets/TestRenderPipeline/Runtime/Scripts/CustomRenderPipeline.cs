using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TestRenderPipeline.Runtime.Scripts
{
    public class CustomRenderPipeline : RenderPipeline
    {
        private Dictionary<int, Camera> _camerasDict = new Dictionary<int, Camera>();
        void Render(ScriptableRenderContext pContext, Camera pCamera)
        {
            //命令缓冲区
            var _cmdBuffer = new CommandBuffer(){name = pCamera.name};
            var _cameraClearFlags = pCamera.clearFlags;
            
            //根据摄像机CameraClearFlags设置清楚参数
            _cmdBuffer.ClearRenderTarget((_cameraClearFlags & CameraClearFlags.Depth) != 0, 
                (_cameraClearFlags & CameraClearFlags.Color) != 0, 
                pCamera.backgroundColor);
            
            pContext.ExecuteCommandBuffer(_cmdBuffer);
            _cmdBuffer.Release();
            
            //设置视图投影矩阵。该转换矩阵将摄像机的位置和方向（视图矩阵）与摄像机的透视或正投影（投影矩阵）结合在一起。
            pContext.SetupCameraProperties(pCamera);

            // 从当前摄像机获取剔除参数
            pCamera.TryGetCullingParameters(out var cullingParameters);
            // 使用剔除参数执行剔除操作，并存储结果
            CullingResults cullingResults = pContext.Cull(ref cullingParameters);

            // 基于当前摄像机，更新内置着色器变量的值
            var _shaderTagId = new ShaderTagId("SRPDefaultUnlit");

            // 基于当前摄像机，向 Unity 告知如何对几何体进行排序
            var sortingSettings = new SortingSettings(pCamera);
                
            //根据剔除结果渲染
            sortingSettings.criteria = SortingCriteria.CommonOpaque;
            var _drawingSettings = new DrawingSettings(_shaderTagId, sortingSettings);
            var _filteringSettings = FilteringSettings.defaultValue;
            _filteringSettings.renderQueueRange = RenderQueueRange.opaque;
            
            // 基于 LightMode 通道标签值，向 Unity 告知要绘制的几何体
            pContext.DrawRenderers(cullingResults, ref _drawingSettings, ref _filteringSettings);

            pContext.DrawSkybox(pCamera);


            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            _drawingSettings.sortingSettings = sortingSettings;
            _filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            // 基于 LightMode 通道标签值，向 Unity 告知要绘制的几何体
            pContext.DrawRenderers(cullingResults, ref _drawingSettings, ref _filteringSettings);

            // 指示图形 API 执行所有调度的命令\
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