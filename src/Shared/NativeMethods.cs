// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace SyncroSim.Epi
{
    internal static class NativeMethods
    {
        public const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
    }
}
