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

namespace Silk.NET.Direct3D12
{
    [NativeName("Name", "D3D12_DEVICE_REMOVED_EXTENDED_DATA")]
    public unsafe partial struct DeviceRemovedExtendedData
    {
        public DeviceRemovedExtendedData
        (
            DredFlags? flags = null,
            AutoBreadcrumbNode* pHeadAutoBreadcrumbNode = null
        ) : this()
        {
            if (flags is not null)
            {
                Flags = flags.Value;
            }

            if (pHeadAutoBreadcrumbNode is not null)
            {
                PHeadAutoBreadcrumbNode = pHeadAutoBreadcrumbNode;
            }
        }


        [NativeName("Type", "D3D12_DRED_FLAGS")]
        [NativeName("Type.Name", "D3D12_DRED_FLAGS")]
        [NativeName("Name", "Flags")]
        public DredFlags Flags;

        [NativeName("Type", "D3D12_AUTO_BREADCRUMB_NODE *")]
        [NativeName("Type.Name", "D3D12_AUTO_BREADCRUMB_NODE *")]
        [NativeName("Name", "pHeadAutoBreadcrumbNode")]
        public AutoBreadcrumbNode* PHeadAutoBreadcrumbNode;
    }
}
