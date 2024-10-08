﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using YuumiCompanion.LOR_Overlay.Deserialization;
using static YuumiCompanion.Form1;
using YuumiCompanion.LOR_Overlay.Model;
using System.Linq;

namespace YuumiCompanion.LOR_Overlay.Business_Layer
{
    internal class BLOverlay
    {
        #region Startup Section

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, uint wMsg, UIntPtr wParam, IntPtr lParam); //used for maximizing the screen
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [Flags]
        enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,

            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            //#if(WINVER >= 0x0400)

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            //#endif /* WINVER >= 0x0400 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_LAYERED = 0x00080000,
            //#endif /* WIN32WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)

            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
            //#endif /* WINVER >= 0x0500 */

            //#if(WIN32WINNT >= 0x0500)

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
            //#endif /* WIN32WINNT >= 0x0500 */

        }

        public enum GetWindowLongConst
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2,
        }

        const int WM_SYSCOMMAND = 0x0112; //used for maximizing the screen.
        const int myWParam = 0xf120; //used for maximizing the screen.
        const int myLparam = 0x5073d; //used for maximizing the screen.


        int oldWindowLong;

        private void MaximizeEverything()
        {
            Form1.Location = getTopLeft();
            Form1.Size = getFullScreenSize();

            SendMessage(Form1.Handle, WM_SYSCOMMAND, (UIntPtr)myWParam, (IntPtr)myLparam);
        }

        /// <summary>
        /// Make the form (specified by its handle) a window that supports transparency.
        /// </summary>
        /// <param name="Handle">The window to make transparency supporting</param>
        public void SetFormTransparent(IntPtr Handle)
        {
            oldWindowLong = GetWindowLong(Handle, (int)GetWindowLongConst.GWL_EXSTYLE);
            SetWindowLong(Handle, (int)GetWindowLongConst.GWL_EXSTYLE, Convert.ToInt32(oldWindowLong | (uint)WindowStyles.WS_EX_LAYERED | (uint)WindowStyles.WS_EX_TRANSPARENT));
        }

        /// <summary>
        /// Makes the form change White to Transparent and clickthrough-able
        /// Can be modified to make the form translucent (with different opacities) and change the Transparency Color.
        /// </summary>
        public void SetTheLayeredWindowAttribute()
        {
            uint transparentColor = 4293981440;

            SetLayeredWindowAttributes(Form1.Handle, transparentColor, 125, 0x2);

            Form1.TransparencyKey = Color.LavenderBlush;
        }

        public void StartupOverlay()
        {
            MaximizeEverything();

            SetFormTransparent(Form1.Handle);

            SetTheLayeredWindowAttribute();
        }

        /// <summary>
        /// Finds the Size of all computer screens combined (assumes screens are left to right, not above and below).
        /// </summary>
        /// <returns>The width and height of all screens combined</returns>
        public static Size getFullScreenSize()
        {
            int height = int.MinValue;
            int width = 0;


            //take largest height
            height = Math.Max(Screen.PrimaryScreen.WorkingArea.Height, height);

            width += Screen.PrimaryScreen.Bounds.Width;

            /*
             * foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                //take largest height
                height = Math.Max(screen.WorkingArea.Height, height);

                width += screen.Bounds.Width;
            }*/

            return new Size(width, height);
        }

        /// <summary>
        /// Finds the top left pixel position (with multiple screens this is often not 0,0)
        /// </summary>
        /// <returns>Position of top left pixel</returns>
        public static Point getTopLeft()
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;

            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                minX = Math.Min(screen.WorkingArea.Left, minX);
                minY = Math.Min(screen.WorkingArea.Top, minY);
            }

            return new Point(minX, minY);
        }
        #endregion

        Form Form1;

        private const int startManaX = 20;
        private const int startCardX = 70;
        private const int startY = 10;

        private const int manaCardX = 50;
        private const int manaCardY = 30;

        private const int cardTextX = 150;
        private const int cardTextY = 30;

        public BLOverlay()
        {
            Form1 = Application.OpenForms["Form1"];
        }

        public void RefreshCurrentDecklist(List<CardCanvas> deckList, List<CardCanvas> currentDeck)
        {
            Label manaLabel;
            Label cardLabel;
            CardCanvas card;
            
            if(deckList != null)
            {
                if(deckList.Count > 0)
                {
                    for (int i = 0; i < deckList.Count; i++)
                    {
                        card = deckList.Skip(i).First();

                        manaLabel = new Label
                        {
                            Text = card.Card.cost.ToString(),
                            Location = new Point(startManaX, manaCardY * i + startY),
                            BackColor = Color.Orange,
                            Size = new Size(manaCardX, manaCardY),
                            Font = new Font("Yu Gothic", 15, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Name = "lblMana"
                        };

                        cardLabel = new Label
                        {
                            Text = card.Card.name + " x" + card.Quantity,
                            Location = new Point(startCardX, cardTextY * i + startY),
                            BackColor = Color.Gold,
                            Size = new Size(cardTextX, cardTextY),
                            Font = new Font("Yu Gothic", 8, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleRight,
                            Name = "lblCard"
                        };

                        Form1.Controls.Add(manaLabel);
                        Form1.Controls.Add(cardLabel);

                        currentDeck.Add(card);
                    }
                }
                
            }
        }

        public void ClearCurrentDecklist()
        {
            Control[] manaControls = Form1.Controls.Find("lblMana", false);
            Control[] cardControls = Form1.Controls.Find("lblCard", false);

            if(manaControls.Length > 0 || cardControls.Length > 0)
            {
                foreach (Control label in manaControls)
                {
                    Form1.Controls.Remove(label);
                }

                foreach (Control label in cardControls)
                {
                    Form1.Controls.Remove(label);
                }
            }


        }
    }
}
