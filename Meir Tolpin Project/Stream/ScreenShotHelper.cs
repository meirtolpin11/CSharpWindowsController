using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Meir_Tolpin_Project.Stream
{
    static class ScreenShotHelper
    {

        /** External functions */
        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        /** Mouse cursor struct */
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        /** mouse position struct */
        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        const Int32 CURSOR_SHOWING = 0x00000001;


        /** Returning a screenShot of the screen with the mouse curser
         * input:
         *      bool CaptureMouse: True- image with the mouse, False - without the mouse
         * output:
         *      Bitmap of the screenShot
         */
        public static Bitmap CaptureScreen(bool CaptureMouse)
        {
            // creating a screensize bitmap
            Bitmap result = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
            try
            {
                using (Graphics g = Graphics.FromImage(result))
                {
                    // getting the screenshot 
                    g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                    if (CaptureMouse)
                    {
                        // getting the mouse postion 
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));
                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                // drowing the mouse on the picture
                                DrawIcon(g.GetHdc(), pci.ptScreenPos.x, pci.ptScreenPos.y, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                }
            }
            catch
            {
                // the picture is NULL type 
                result = null;
            }
            // returning the picture 
            return result;
        }
    }
}
