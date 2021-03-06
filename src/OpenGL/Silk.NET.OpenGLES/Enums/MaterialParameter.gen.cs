// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using Silk.NET.Core.Attributes;

#pragma warning disable 1591

namespace Silk.NET.OpenGLES
{
    [NativeName("Name", "MaterialParameter")]
    public enum MaterialParameter : int
    {
        [NativeName("Name", "GL_AMBIENT")]
        Ambient = 0x1200,
        [NativeName("Name", "GL_DIFFUSE")]
        Diffuse = 0x1201,
        [NativeName("Name", "GL_SPECULAR")]
        Specular = 0x1202,
        [NativeName("Name", "GL_EMISSION")]
        Emission = 0x1600,
        [NativeName("Name", "GL_SHININESS")]
        Shininess = 0x1601,
        [NativeName("Name", "GL_AMBIENT_AND_DIFFUSE")]
        AmbientAndDiffuse = 0x1602,
        [NativeName("Name", "GL_COLOR_INDEXES")]
        ColorIndexes = 0x1603,
    }
}
