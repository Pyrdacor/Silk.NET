﻿using System;
using System.Numerics;
namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class ColorShader
    {
        static ColorShader colorShader = null;
        internal static readonly string DefaultFragmentOutColorName = "outColor";
        internal static readonly string DefaultPositionName = "position";
        internal static readonly string DefaultModelViewMatrixName = "mvMat";
        internal static readonly string DefaultProjectionMatrixName = "projMat";
        internal static readonly string DefaultColorName = "color";
        internal static readonly string DefaultZName = "z";
        internal static readonly string DefaultLayerName = "layer";

        internal ShaderProgram shaderProgram;
        readonly string fragmentOutColorName;
        readonly string modelViewMatrixName;
        readonly string projectionMatrixName;
        readonly string colorName;
        readonly string zName;
        readonly string positionName;
        readonly string layerName;

        // gl_FragColor is deprecated beginning in GLSL version 1.30
        protected static bool HasGLFragColor()
        {
            return State.GLSLVersionMajor == 1 && State.GLSLVersionMinor < 3;
        }

        protected static string GetFragmentShaderHeader()
        {
            string header = $"#version {State.GLSLVersionMajor}{State.GLSLVersionMinor}\n";

            header += "\n";
            header += "#ifdef GL_ES\n";
            header += " precision mediump float;\n";
            header += " precision highp int;\n";
            header += "#endif\n";
            header += "\n";
            
            if (!HasGLFragColor())
                header += $"out vec4 {DefaultFragmentOutColorName};\n";

            return header;
        }

        protected static string GetVertexShaderHeader()
        {
            return $"#version {State.GLSLVersionMajor}{State.GLSLVersionMinor}\n\n";
        }

        protected static string GetInName(bool fragment)
        {
            if (State.GLSLVersionMajor == 1 && State.GLSLVersionMinor < 3)
            {
                if (fragment)
                    return "varying";
                else
                    return "attribute";
            }
            else
                return "in";
        }

        protected static string GetOutName()
        {
            if (State.GLSLVersionMajor == 1 && State.GLSLVersionMinor < 3)
                return "varying";
            else
                return "out";
        }

        static readonly string[] ColorFragmentShader = new string[]
        {
            GetFragmentShaderHeader(),
            $"flat {GetInName(true)} vec4 pixelColor;",
            $"",
            $"void main()",
            $"{{",
            $"    {(HasGLFragColor() ? "gl_FragColor" : DefaultFragmentOutColorName)} = pixelColor;",
            $"}}"
        };

        static readonly string[] ColorVertexShader = new string[]
        {
            GetVertexShaderHeader(),
            $"{GetInName(false)} ivec2 {DefaultPositionName};",
            $"{GetInName(false)} uint {DefaultLayerName};",
            $"{GetInName(false)} uvec4 {DefaultColorName};",
            $"uniform float {DefaultZName};",
            $"uniform mat4 {DefaultProjectionMatrixName};",
            $"uniform mat4 {DefaultModelViewMatrixName};",
            $"flat {GetOutName()} vec4 pixelColor;",
            $"",
            $"void main()",
            $"{{",
            $"    vec2 pos = vec2(float({DefaultPositionName}.x) + 0.49f, float({DefaultPositionName}.y) + 0.49f);",
            $"    pixelColor = vec4({DefaultColorName}.r / 255.0f, {DefaultColorName}.g / 255.0f, {DefaultColorName}.b / 255.0f, {DefaultColorName}.a / 255.0f);",
            $"    ",
            $"    gl_Position = {DefaultProjectionMatrixName} * {DefaultModelViewMatrixName} * vec4(pos, 1.0f - {DefaultZName} - float({DefaultLayerName}) * 0.00001f, 1.0f);",
            $"}}"
        };

        public void UpdateMatrices(bool zoom)
        {
            if (State.CurrentModelViewMatrix != null)
            {
                if (zoom)
                    shaderProgram.SetInputMatrix(modelViewMatrixName, State.CurrentModelViewMatrix.Value.ToArray(), true);
                else
                    shaderProgram.SetInputMatrix(modelViewMatrixName, State.CurrentUnzoomedModelViewMatrix.Value.ToArray(), true);
            }
            else
            {
                shaderProgram.SetInputMatrix(modelViewMatrixName, Matrix4x4.Identity.ToArray(), true);
            }

            if (State.CurrentProjectionMatrix == null)
                throw new InvalidOperationException("No projection matrix is set.");

            shaderProgram.SetInputMatrix(projectionMatrixName, State.CurrentProjectionMatrix.Value.ToArray(), true);
        }

        public void Use()
        {
            if (shaderProgram != ShaderProgram.ActiveProgram)
                shaderProgram.Use();
        }

        ColorShader()
            : this(DefaultModelViewMatrixName, DefaultProjectionMatrixName, DefaultColorName, DefaultZName,
                  DefaultPositionName, DefaultLayerName, ColorFragmentShader, ColorVertexShader)
        {

        }

        protected ColorShader(string modelViewMatrixName, string projectionMatrixName, string colorName, string zName,
            string positionName, string layerName, string[] fragmentShaderLines, string[] vertexShaderLines)
        {
            fragmentOutColorName = (State.OpenGLVersionMajor > 2) ? DefaultFragmentOutColorName : "gl_FragColor";

            this.modelViewMatrixName = modelViewMatrixName;
            this.projectionMatrixName = projectionMatrixName;
            this.colorName = colorName;
            this.zName = zName;
            this.positionName = positionName;
            this.layerName = layerName;

            var fragmentShader = new Shader(Shader.Type.Fragment, string.Join("\n", fragmentShaderLines));
            var vertexShader = new Shader(Shader.Type.Vertex, string.Join("\n", vertexShaderLines));

            shaderProgram = new ShaderProgram(fragmentShader, vertexShader);

            shaderProgram.SetFragmentColorOutputName(fragmentOutColorName);
        }

        public ShaderProgram ShaderProgram => shaderProgram;

        public void SetZ(float z)
        {
            shaderProgram.SetInput(zName, z);
        }

        public static ColorShader Instance
        {
            get
            {
                if (colorShader == null)
                    colorShader = new ColorShader();

                return colorShader;
            }
        }
    }
}
