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

namespace Silk.NET.Direct3D11
{
    [NativeName("Name", "D3D11_AUTHENTICATED_QUERY_OUTPUT_ID_COUNT_OUTPUT")]
    public unsafe partial struct AuthenticatedQueryOutputIDCountOutput
    {
        public AuthenticatedQueryOutputIDCountOutput
        (
            AuthenticatedQueryOutput? output = null,
            void* deviceHandle = null,
            void* cryptoSessionHandle = null,
            uint? outputIDCount = null
        ) : this()
        {
            if (output is not null)
            {
                Output = output.Value;
            }

            if (deviceHandle is not null)
            {
                DeviceHandle = deviceHandle;
            }

            if (cryptoSessionHandle is not null)
            {
                CryptoSessionHandle = cryptoSessionHandle;
            }

            if (outputIDCount is not null)
            {
                OutputIDCount = outputIDCount.Value;
            }
        }


        [NativeName("Type", "D3D11_AUTHENTICATED_QUERY_OUTPUT")]
        [NativeName("Type.Name", "D3D11_AUTHENTICATED_QUERY_OUTPUT")]
        [NativeName("Name", "Output")]
        public AuthenticatedQueryOutput Output;

        [NativeName("Type", "HANDLE")]
        [NativeName("Type.Name", "HANDLE")]
        [NativeName("Name", "DeviceHandle")]
        public void* DeviceHandle;

        [NativeName("Type", "HANDLE")]
        [NativeName("Type.Name", "HANDLE")]
        [NativeName("Name", "CryptoSessionHandle")]
        public void* CryptoSessionHandle;

        [NativeName("Type", "UINT")]
        [NativeName("Type.Name", "UINT")]
        [NativeName("Name", "OutputIDCount")]
        public uint OutputIDCount;
    }
}
