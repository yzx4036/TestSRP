using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TestRenderPipeline.Runtime.Scripts
{
    public partial class CameraRenderer
    {
        partial void DrawUnsupportedShaders();
        partial void DrawGizmos();
        partial void PrepareForSceneWindow();

#if UNITY_EDITOR

        private static Material _errorMaterial;

        private static ShaderTagId[] legecyShaderTagIds =
        {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM"),
        };

        partial void DrawUnsupportedShaders()
        {
            if (_errorMaterial == null)
            {
                _errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
            }

            var _drawingSettings = new DrawingSettings(legecyShaderTagIds[0], new SortingSettings(_camera))
                {overrideMaterial = _errorMaterial};
            for (int i = 1; i < legecyShaderTagIds.Length; i++)
            {
                _drawingSettings.SetShaderPassName(i, legecyShaderTagIds[i]);
            }

            var _filteringSettings = FilteringSettings.defaultValue;
            _scriptableRenderContext.DrawRenderers(_cullingResults, ref _drawingSettings, ref _filteringSettings);
        }

        partial void DrawGizmos()
        {
            if (Handles.ShouldRenderGizmos())
            {
                _scriptableRenderContext.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
                _scriptableRenderContext.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
            }
        }

        /// <summary>
        /// 摄像机渲染UI到Scene window
        /// </summary>
        partial void PrepareForSceneWindow()
        {
            if (_camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
            }
        }
    }
#endif
}