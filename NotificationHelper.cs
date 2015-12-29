using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clipstack
{
    // adapted from: http://stackoverflow.com/questions/2226920/how-to-monitor-clipboard-content-changes-in-c

    /// <summary>
    /// Provides notifications when the contents of the clipboard is updated.
    /// </summary>
    public sealed class NotificationHelper
    {
        public static void Initialize(Keys hotkey)
        {
            _form = new NotificationForm(hotkey);
        }

        public static void Loop()
        {
            Application.Run(_form);
        }
        /// <summary>
        /// Occurs when the contents of the clipboard is updated.
        /// </summary>
        public static event EventHandler ClipboardHandler;
        public static event EventHandler HotkeyHandler;
        private static NotificationForm _form;

        /// <summary>
        /// Raises the <see cref="ClipboardHandler"/> event.
        /// </summary>
        /// <param name="e">Event arguments for the event.</param>
        private static void OnClipboardUpdate(EventArgs e)
        {
            var handler = ClipboardHandler;
            if (handler != null)
            {
                handler(null, e);
            }
        }
        private static void OnHotkey(EventArgs e)
        {
            var handler = HotkeyHandler;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        /// <summary>
        /// Hidden form to recieve the WM_CLIPBOARDUPDATE message.
        /// </summary>
        private class NotificationForm : Form
        {
            public NotificationForm(Keys hotkey)
            {
                NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
                NativeMethods.AddClipboardFormatListener(Handle);

                NativeHotkey.RegisterHotKey(this, hotkey);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
                {
                    OnClipboardUpdate(null);
                }
                if (m.Msg == NativeMethods.WM_HOTKEY)
                {
                    OnHotkey(null);
                }
                base.WndProc(ref m);
            }

            protected override void OnFormClosed(FormClosedEventArgs e)
            {
                NativeHotkey.UnregisterHotKey(this);
                base.OnFormClosed(e);
            }
        }
    }

    internal static class NativeMethods
    {
        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        public const int WM_HOTKEY = 0x0312;
        public static IntPtr HWND_MESSAGE = new IntPtr(-3);

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        // See http://msdn.microsoft.com/en-us/library/ms633541%28v=vs.85%29.aspx
        // See http://msdn.microsoft.com/en-us/library/ms649033%28VS.85%29.aspx
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }
}
