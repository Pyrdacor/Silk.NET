// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Core.Attributes;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Loader;

#pragma warning disable 1591

namespace Silk.NET.XAudio
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [NativeName("Name", "XAUDIO2FX_VOLUMEMETER_LEVELS")]
    public unsafe partial struct FXVolumemeterLevels
    {
        public FXVolumemeterLevels
        (
            float* pPeakLevels = null,
            float* pRMSLevels = null,
            uint? channelCount = null
        ) : this()
        {
            if (pPeakLevels is not null)
            {
                PPeakLevels = pPeakLevels;
            }

            if (pRMSLevels is not null)
            {
                PRMSLevels = pRMSLevels;
            }

            if (channelCount is not null)
            {
                ChannelCount = channelCount.Value;
            }
        }


        [NativeName("Type", "float *")]
        [NativeName("Type.Name", "float *")]
        [NativeName("Name", "pPeakLevels")]
        public float* PPeakLevels;

        [NativeName("Type", "float *")]
        [NativeName("Type.Name", "float *")]
        [NativeName("Name", "pRMSLevels")]
        public float* PRMSLevels;

        [NativeName("Type", "UINT32")]
        [NativeName("Type.Name", "UINT32")]
        [NativeName("Name", "ChannelCount")]
        public uint ChannelCount;
    }
}
