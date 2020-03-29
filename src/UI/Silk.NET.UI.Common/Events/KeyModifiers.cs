using System;

namespace Silk.NET.UI
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0x0000,
        Shift = 0x0001,
        Control = 0x0002,
        Alt = 0x0004,
    }
}