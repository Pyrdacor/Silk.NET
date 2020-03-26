using System.Drawing;
using System;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class Context
    {
        private int _width = -1;
        private int _height = -1;
        private Rotation _rotation = Rotation.None;
        private Matrix4x4 _modelViewMatrix = Matrix4x4.Identity;
        private Matrix4x4 _unzoomedModelViewMatrix = Matrix4x4.Identity;
        private float _zoom = 0.0f;
        private Color _backgroundColor = Color.Gray;

        public Context(RenderDimensionReference dimensions)
        {
            // TODO: maybe support earlier versions later?
            // We need at least OpenGL 3.1 for instancing and shaders
            if (State.OpenGLVersionMajor < 3 || (State.OpenGLVersionMajor == 3 && State.OpenGLVersionMinor < 1))
                throw new NotSupportedException($"OpenGL version 3.1 is required for rendering. Your version is {State.OpenGLVersionMajor}.{State.OpenGLVersionMinor}.");

            State.Gl.ClearColor(_backgroundColor);

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

            _width = width;
            _height = height;

            SetRotation(_rotation, true);
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (Util.FloatEqual(value, _zoom) || value < 0.0f)
                    return;

                _zoom = value;

                ApplyMatrix();
            }
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value)
                    return;

                _backgroundColor = value;
                State.Gl.ClearColor(_backgroundColor);
            }
        }

        public void SetRotation(Rotation rotation, bool forceUpdate = false)
        {
            if (forceUpdate || rotation != _rotation)
            {
                _rotation = rotation;

                ApplyMatrix();
            }
        }

        void ApplyMatrix()
        {
            State.RestoreModelViewMatrix(_modelViewMatrix);
            State.PopModelViewMatrix();
            State.RestoreUnzoomedModelViewMatrix(_unzoomedModelViewMatrix);
            State.PopUnzoomedModelViewMatrix();

            if (_rotation == Rotation.None)
            {
                _modelViewMatrix = Matrix4x4.Identity;
            }
            else
            {
                var rotationDegree = 0.0f;

                switch (_rotation)
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

                var x = 0.5f * _width;
                var y = 0.5f * _height;
                const float deg2rad = (float)(Math.PI / 180.0);

                if (_rotation != Rotation.Deg180) // 90° or 270°
                {
                    float factor = (float)_height / (float)_width;
                    _modelViewMatrix =
                        Matrix4x4.CreateTranslation(x, y, 0.0f) *
                        Matrix4x4.CreateRotationZ(rotationDegree * deg2rad) *
                        Matrix4x4.CreateScale(factor, 1.0f / factor, 1.0f) *
                        Matrix4x4.CreateTranslation(-x, -y, 0.0f);
                }
                else // 180°
                {
                    _modelViewMatrix =
                        Matrix4x4.CreateTranslation(x, y, 0.0f) *
                        Matrix4x4.CreateRotationZ(rotationDegree * deg2rad) *
                        Matrix4x4.CreateTranslation(-x, -y, 0.0f);
                }
            }

            _unzoomedModelViewMatrix = _modelViewMatrix;

            State.PushUnzoomedModelViewMatrix(_unzoomedModelViewMatrix);

            if (!Util.FloatEqual(_zoom, 0.0f))
            {
                var x = 0.5f * _width;
                var y = 0.5f * _height;

                _modelViewMatrix = Matrix4x4.CreateTranslation(x, y, 0.0f) *
                    Matrix4x4.CreateScale(1.0f + _zoom * 0.5f, 1.0f + _zoom * 0.5f, 1.0f) *
                    Matrix4x4.CreateTranslation(-x, -y, 0.0f) *
                    _modelViewMatrix;
            }

            State.PushModelViewMatrix(_modelViewMatrix);
        }
    }
}
