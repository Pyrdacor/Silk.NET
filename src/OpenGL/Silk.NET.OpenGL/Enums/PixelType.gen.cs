// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using Silk.NET.Core.Attributes;

#pragma warning disable 1591

namespace Silk.NET.OpenGL
{
    [NativeName("Name", "PixelType")]
    public enum PixelType : int
    {
        [NativeName("Name", "GL_BYTE")]
        Byte = 0x1400,
        [NativeName("Name", "GL_UNSIGNED_BYTE")]
        UnsignedByte = 0x1401,
        [NativeName("Name", "GL_SHORT")]
        Short = 0x1402,
        [NativeName("Name", "GL_UNSIGNED_SHORT")]
        UnsignedShort = 0x1403,
        [NativeName("Name", "GL_INT")]
        Int = 0x1404,
        [NativeName("Name", "GL_UNSIGNED_INT")]
        UnsignedInt = 0x1405,
        [NativeName("Name", "GL_FLOAT")]
        Float = 0x1406,
        [NativeName("Name", "GL_UNSIGNED_BYTE_3_3_2")]
        UnsignedByte332 = 0x8032,
        [NativeName("Name", "GL_UNSIGNED_BYTE_3_3_2_EXT")]
        UnsignedByte332Ext = 0x8032,
        [NativeName("Name", "GL_UNSIGNED_SHORT_4_4_4_4")]
        UnsignedShort4444 = 0x8033,
        [NativeName("Name", "GL_UNSIGNED_SHORT_4_4_4_4_EXT")]
        UnsignedShort4444Ext = 0x8033,
        [NativeName("Name", "GL_UNSIGNED_SHORT_5_5_5_1")]
        UnsignedShort5551 = 0x8034,
        [NativeName("Name", "GL_UNSIGNED_SHORT_5_5_5_1_EXT")]
        UnsignedShort5551Ext = 0x8034,
        [NativeName("Name", "GL_UNSIGNED_INT_8_8_8_8")]
        UnsignedInt8888 = 0x8035,
        [NativeName("Name", "GL_UNSIGNED_INT_8_8_8_8_EXT")]
        UnsignedInt8888Ext = 0x8035,
        [NativeName("Name", "GL_UNSIGNED_INT_10_10_10_2")]
        UnsignedInt1010102 = 0x8036,
        [NativeName("Name", "GL_UNSIGNED_INT_10_10_10_2_EXT")]
        UnsignedInt1010102Ext = 0x8036,
    }
}
