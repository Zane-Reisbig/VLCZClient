using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WINFORMS_VLCClient.Lib.FullScreen
{
    enum WindowLongEnum
    {
        GWL_STYLE = -16,
    }

    [Flags]
    enum WindowStyles : long
    {
        POPUP = 0x00800000L,
        WS_VISIBLE = 0x10000000L,
    }

    enum WindowZOrderBehavior
    {
        HWND_BOTTOM = 1,
        HWND_NOTOPMOST = -2,
        HWND_TOP = 0,
        HWND_TOPMOST = -1,
    }

    [Flags]
    enum SetWindowPosFlags : uint
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOREDRAW = 0x0008,
        SWP_NOACTIVATE = 0x0010,
        SWP_DRAWFRAME = 0x0020,
        SWP_FRAMECHANGED = 0x0020,
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOCOPYBITS = 0x0100,
        SWP_NOOWNERZORDER = 0x0200,
        SWP_NOREPOSITION = 0x0200,
        SWP_NOSENDCHANGING = 0x0400,
        SWP_DEFERERASE = 0x2000,
        SWP_ASYNCWINDOWPOS = 0x4000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public uint length;
        public uint flags;
        public uint showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
        public RECT rcDevice;
    }

    public static class FullscreenHelper
    {
        static Dictionary<nint, ManagedWindow> HandledWindows = [];

        public static void FullScreenWindow(nint hWnd)
        {
            var style = GetWindowLongPtr(hWnd, (int)WindowLongEnum.GWL_STYLE);

            WINDOWPLACEMENT placement;
            GetWindowPlacement(hWnd, out placement);

            HandledWindows[hWnd] = new(hWnd, placement, style);

            SetWindowLongPtr(
                hWnd,
                (int)WindowLongEnum.GWL_STYLE,
                (nint)WindowStyles.POPUP | (nint)WindowStyles.WS_VISIBLE
            );

            Screen currentScreen = Screen.FromHandle(hWnd);
            SetWindowPos(
                hWnd,
                (nint)WindowZOrderBehavior.HWND_TOP,
                currentScreen.Bounds.Left,
                currentScreen.Bounds.Top,
                currentScreen.Bounds.Right - currentScreen.Bounds.Left,
                currentScreen.Bounds.Bottom - currentScreen.Bounds.Top,
                (uint)SetWindowPosFlags.SWP_NOOWNERZORDER
                    | (uint)SetWindowPosFlags.SWP_FRAMECHANGED
                    | (uint)SetWindowPosFlags.SWP_SHOWWINDOW
            );

            RedrawWindow(
                hWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                RDW_INVALIDATE | RDW_ERASE | RDW_ALLCHILDREN | RDW_UPDATENOW
            );
        }

        public static void LeaveFullscreen(nint hWnd)
        {
            ManagedWindow window;
            if (!HandledWindows.TryGetValue(hWnd, out window!))
            {
                Debug.WriteLine($"WARN: Failed to get hwnd (\"{hWnd}\") from HandledWindows!");
                return;
            }

            // Restore the original window style directly (not a pointer)
            SetWindowLongPtr(hWnd, (int)WindowLongEnum.GWL_STYLE, window.prevStyle);

            // Marshal the placement struct to a pointer for SetWindowPlacement
            IntPtr prevPlacementPtr = Marshal.AllocHGlobal(Marshal.SizeOf<WINDOWPLACEMENT>());
            try
            {
                Marshal.StructureToPtr(window.prevLocation, prevPlacementPtr, false);
                SetWindowPlacement(hWnd, prevPlacementPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(prevPlacementPtr);
            }

            SetWindowPos(
                hWnd,
                (nint)WindowZOrderBehavior.HWND_NOTOPMOST,
                0,
                0,
                0,
                0,
                (uint)SetWindowPosFlags.SWP_NOMOVE
                    | (uint)SetWindowPosFlags.SWP_NOSIZE
                    | (uint)SetWindowPosFlags.SWP_NOZORDER
                    | (uint)SetWindowPosFlags.SWP_FRAMECHANGED
                    | (uint)SetWindowPosFlags.SWP_SHOWWINDOW
            );

            RedrawWindow(
                hWnd,
                IntPtr.Zero,
                IntPtr.Zero,
                RDW_INVALIDATE | RDW_ERASE | RDW_ALLCHILDREN | RDW_UPDATENOW
            );
            HandledWindows.Remove(hWnd);
        }

        public class ManagedWindow(nint hwnd, WINDOWPLACEMENT prevLocation, nint prevStyle)
        {
            public nint hwnd = hwnd;
            public WINDOWPLACEMENT prevLocation = prevLocation;
            public nint prevStyle = prevStyle;
        }

        [DllImport("user32.dll")]
        static extern nint GetWindowLongPtr(nint hwnd, int nIndex);

        [DllImport("user32.dll")]
        static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);

        [DllImport("user32.dll")]
        static extern void GetWindowPlacement(nint hwnd, out WINDOWPLACEMENT placement);

        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(nint hWnd, nint pointerToWindowPlacement);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
            nint hWnd,
            nint hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags
        );

        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_ERASE = 0x0004;
        private const uint RDW_ALLCHILDREN = 0x0080;
        private const uint RDW_UPDATENOW = 0x0100;

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(
            IntPtr hWnd,
            IntPtr lprcUpdate,
            IntPtr hrgnUpdate,
            uint flags
        );
    }
}
