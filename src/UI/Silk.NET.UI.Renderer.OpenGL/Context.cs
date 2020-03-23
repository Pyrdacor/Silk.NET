﻿using System;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class Context
    {
        int width = -1;
        int height = -1;
        Rotation rotation = Rotation.None;
        Matrix4x4 modelViewMatrix = Matrix4x4.Identity;
        Matrix4x4 unzoomedModelViewMatrix = Matrix4x4.Identity;
        float zoom = 0.0f;

        public Context(RenderDimensionReference dimensions)
        {
            // TODO: maybe support earlier versions later?
            // We need at least OpenGL 3.1 for instancing and shaders
            if (State.OpenGLVersionMajor < 3 || (State.OpenGLVersionMajor == 3 && State.OpenGLVersionMinor < 1))
                throw new NotSupportedException($"OpenGL version 3.1 is required for rendering. Your version is {State.OpenGLVersionMajor}.{State.OpenGLVersionMinor}.");

            State.Gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            State.Gl.Enable(EnableCap.DepthTest);
            State.Gl.DepthFunc(DepthFunction.Lequal);

            State.Gl.Enable(EnableCap.Blend);
            State.Gl.BlendEquationSeparate(BlendEquationModeEXT.FuncAdd, BlendEquationModeEXT.FuncAdd);
            State.Gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.Zero);

            Resize(dimensions.Width, dimensions.Height);

            dimensions.DimensionsChanged += () => Resize(dimensions.Width, dimensions.Height);
        }

        public void Resize(int width, int height)
        {
            State.ClearMatrices();
            State.PushModelViewMatrix(Matrix4x4.Identity);
            State.PushUnzoomedModelViewMatrix(Matrix4x4.Identity);
            State.PushProjectionMatrix(Matrix4x4.CreateOrthographic(width, height, 0.0f, 1.0f));

            this.width = width;
            this.height = height;

            SetRotation(rotation, true);
        }

        public float Zoom
        {
            get => zoom;
            set
            {
                if (Util.FloatEqual(value, zoom) || value < 0.0f)
                    return;

                zoom = value;

                ApplyMatrix();
            }
        }

        public void SetRotation(Rotation rotation, bool forceUpdate = false)
        {
            if (forceUpdate || rotation != this.rotation)
            {
                this.rotation = rotation;

                ApplyMatrix();
            }
        }

        void ApplyMatrix()
        {
            State.RestoreModelViewMatrix(modelViewMatrix);
            State.PopModelViewMatrix();
            State.RestoreUnzoomedModelViewMatrix(unzoomedModelViewMatrix);
            State.PopUnzoomedModelViewMatrix();

            if (rotation == Rotation.None)
            {
                modelViewMatrix = Matrix4x4.Identity;
            }
            else
            {
                var rotationDegree = 0.0f;

                switch (rotation)
                {
                    case Rotation.Deg90:
                        rotationDegree = 90.0f;
                        break;
                    case Rotation.Deg180:
                        rotationDegree = 180.0f;
                        break;
                    case Rotation.Deg270:
                        rotationDegree = 270.0f;
                        break;
                    default:
                        break;
                }

                var x = 0.5f * width;
                var y = 0.5f * height;
                const float deg2rad = (float)(Math.PI / 180.0);

                if (rotation != Rotation.Deg180) // 90° or 270°
                {
                    float factor = (float)height / (float)width;
                    modelViewMatrix =
                        Matrix4x4.CreateTranslation(x, y, 0.0f) *
                        Matrix4x4.CreateRotationZ(rotationDegree * deg2rad) *
                        Matrix4x4.CreateScale(factor, 1.0f / factor, 1.0f) *
                        Matrix4x4.CreateTranslation(-x, -y, 0.0f);
                }
                else // 180°
                {
                    modelViewMatrix =
                        Matrix4x4.CreateTranslation(x, y, 0.0f) *
                        Matrix4x4.CreateRotationZ(rotationDegree * deg2rad) *
                        Matrix4x4.CreateTranslation(-x, -y, 0.0f);
                }
            }

            unzoomedModelViewMatrix = modelViewMatrix;

            State.PushUnzoomedModelViewMatrix(unzoomedModelViewMatrix);

            if (!Util.FloatEqual(zoom, 0.0f))
            {
                var x = 0.5f * width;
                var y = 0.5f * height;

                modelViewMatrix = Matrix4x4.CreateTranslation(x, y, 0.0f) *
                    Matrix4x4.CreateScale(1.0f + zoom * 0.5f, 1.0f + zoom * 0.5f, 1.0f) *
                    Matrix4x4.CreateTranslation(-x, -y, 0.0f) *
                    modelViewMatrix;
            }

            State.PushModelViewMatrix(modelViewMatrix);
        }
    }
}
