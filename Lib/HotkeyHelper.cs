using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WINFORMS_VLCClient.Lib.Hotkeys
{
    public static class HotkeyHelper
    {
        public const int WM_HOTKEY = 0x0312;
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;
        public const uint MOD_NONE = 0x0000;

        public static bool RegisterHotkey(nint hWnd, int id, uint fsModifiers, uint key) =>
            RegisterHotKey(hWnd, id, fsModifiers, key);

        public static bool UnregisterHotkey(nint hWnd, int id) => UnregisterHotKey(hWnd, id);

        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
