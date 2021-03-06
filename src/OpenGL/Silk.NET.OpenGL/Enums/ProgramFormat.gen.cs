// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using Silk.NET.Core.Attributes;

#pragma warning disable 1591

namespace Silk.NET.OpenGL
{
    [NativeName("Name", "ProgramFormat")]
    public enum ProgramFormat : int
    {
        [NativeName("Name", "GL_PROGRAM_FORMAT_ASCII_ARB")]
        ProgramFormatAsciiArb = 0x8875,
    }
}
