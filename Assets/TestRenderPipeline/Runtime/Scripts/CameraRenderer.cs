using UnityEngine;
using UnityEngine.Rendering;

namespace TestRenderPipeline.Runtime.Scripts
{
    public partial class CameraRenderer
    {
        const string _buffName = "Render Camera";
        ScriptableRenderContext _scriptableRenderContext;
        Camera _camera;

        CullingResults _cullingResults;

        //命令缓冲区
        CommandBuffer _cmdBuffer = new() { name = _buffName };

        public void Render(ScriptableRenderContext pContext, Camera pCamera)
        {
            _scriptableRenderContext = pContext;
            _camera = pCamera;
            
            PrepareBuffer();
            //剔除之前调用一下
            PrepareForSceneWindow();
            
            if (!Cull())
            {
                return;
            }

            //设置视图投影矩阵。该转换矩阵将摄像机的位置和方向（视图矩阵）与摄像机的透视或正投影（投影矩阵）结合在一起。
            Setup();

            //绘制
            DrawVisibleGeometry();

            DrawUnsupportedShaders();
            
            DrawGizmos();
            // 指示图形 API 执行所有调度的命令\
            Submit();
        }

        void Setup()
        {
            _scriptableRenderContext.SetupCameraProperties(_camera);

            //根据摄像机CameraClearFlags设置清楚参数
            var _cameraClearFlags = _camera.clearFlags;
            _cmdBuffer.ClearRenderTarget((_cameraClearFlags <= CameraClearFlags.Depth) ,
                (_cameraClearFlags == CameraClearFlags.Color),
                _cameraClearFlags == CameraClearFlags.Color ? _camera.backgroundColor.linear : Color.clear);

            _cmdBuffer.BeginSample(SampleName);
            ExecuteBuffer();
        }

        void DrawVisibleGeometry()
        {
            // 基于当前摄像机，更新内置着色器变量的值
            var _shaderTagId = new ShaderTagId("SRPDefaultUnlit");

            // 基于当前摄像机，向 Unity 告知如何对几何体进行排序
            var sortingSettings = new SortingSettings(_camera) { criteria = SortingCriteria.CommonOpaque };

            //根据剔除结果渲染
            sortingSettings.criteria = SortingCriteria.CommonOpaque;
            var _drawingSettings = new DrawingSettings(_shaderTagId, sortingSettings);
            var _filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            _filteringSettings.renderQueueRange = RenderQueueRange.opaque;

            // 基于 LightMode 通道标签值，向 Unity 告知要绘制的几何体
            _scriptableRenderContext.DrawRenderers(_cullingResults, ref _drawingSettings, ref _filteringSettings);

            _scriptableRenderContext.DrawSkybox(_camera);


            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            _drawingSettings.sortingSettings = sortingSettings;
            _filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            // 基于 LightMode 通道标签值，向 Unity 告知要绘制的几何体
            _scriptableRenderContext.DrawRenderers(_cullingResults, ref _drawingSettings, ref _filteringSettings);
        }

        void Submit()
        {
            _cmdBuffer.EndSample(SampleName);
            ExecuteBuffer();
            _scriptableRenderContext.Submit();
        }

        void ExecuteBuffer()
        {
            _scriptableRenderContext.ExecuteCommandBuffer(_cmdBuffer);
            _cmdBuffer.Clear();
        }

        bool Cull()
        {
            // 从当前摄像机获取剔除参数
            if (_camera.TryGetCullingParameters(out var cullingParameters))
            {
                // 使用剔除参数执行剔除操作，并存储结果
                _cullingResults = _scriptableRenderContext.Cull(ref cullingParameters);

                return true;
            }

            return false;
        }
    }
}