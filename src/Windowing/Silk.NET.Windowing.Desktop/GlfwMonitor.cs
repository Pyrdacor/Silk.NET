// This file is part of Silk.NET.
// 
// You may modify and distribute Silk.NET under the terms
// of the MIT license. See the LICENSE file for details.

using System;
using System.Drawing;
using Silk.NET.GLFW;
using Silk.NET.Windowing.Common;

namespace Silk.NET.Windowing.Desktop
{
    internal unsafe class GlfwMonitor : IMonitor
    {
        public Monitor* Handle { get; }

        public GlfwMonitor(Monitor* monitor, int index)
        {
            Handle = monitor;
            Index = index;
        }

        public IWindow CreateWindow(WindowOptions opts) => opts.WindowState == WindowState.Fullscreen
            ? new GlfwWindow(opts, null, this)
            : throw new PlatformNotSupportedException
                ("On the GLFW backend, windows must be fullscreen in order to be created on a specific monitor.");

        public string Name => GlfwProvider.GLFW.Value.GetMonitorName(Handle);
        public int Index { get; }

        public Rectangle Bounds
        {
            get
            {
                GlfwProvider.GLFW.Value.GetMonitorWorkarea(Handle, out var x, out var y, out var w, out var h);
                return new Rectangle(x, y, w, h);
            }
        }

        public int RefreshRate => GlfwProvider.GLFW.Value.GetVideoMode(Handle)->RefreshRate;
    }
}