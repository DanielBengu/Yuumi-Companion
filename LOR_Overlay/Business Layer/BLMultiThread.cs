using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using TranparentOverlay_simpleExample.LOR_Overlay.Deserialization;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;

namespace TranparentOverlay_simpleExample.LOR_Overlay.Business_Layer
{
    internal class BLMultiThread
    {
        BackgroundWorker bwMaster;
        BackgroundWorker bwAPI;

        Random rand;

        Form Form1;

        public BLMultiThread()
        {
            bwMaster = new BackgroundWorker();
            bwAPI = new BackgroundWorker();
            rand = new Random(DateTime.Now.Millisecond);

            Form1 = Application.OpenForms["Form1"];
        }

        public void StartThreads()
        {
            BackgroundWorker masterWorker = new BackgroundWorker();
            BackgroundWorker APIWorker = new BackgroundWorker();
            APIWorker.DoWork += new DoWorkEventHandler(bw_DoWork_RetrieveData);
            masterWorker.DoWork += new DoWorkEventHandler(bw_DoWork_DrawLines);

            this.bwMaster = masterWorker;
            this.bwAPI = APIWorker;

            this.bwMaster.RunWorkerAsync();
            this.bwAPI.RunWorkerAsync();
        }

        private void bw_DoWork_DrawLines(object sender, DoWorkEventArgs e)
        {
            int numRedLines = 500;
            int numWhiteLines = 1000;

            Size fullSize = getFullScreenSize();
            Point topLeft = getTopLeft();

            using (Pen redPen = new Pen(Color.Red, 10f), whitePen = new Pen(Color.LavenderBlush, 10f))
            {
                using (Graphics formGraphics = Form1.CreateGraphics())
                {

                    while (true)
                    {

                        bool makeRedLines = true;

                        for (int i = 0; i < numRedLines + numWhiteLines; i++)
                        {
                            if (i > numRedLines)
                            {
                                makeRedLines = false;
                            }

                            //Choose points for random lines...but don't draw over the top 100 px of the screen so you can 
                            //still find the stop run button.
                            int pX = rand.Next(0, (-1 * topLeft.X) + fullSize.Width);
                            int pY = rand.Next(100, (-1 * topLeft.Y) + fullSize.Height);

                            int qX = rand.Next(0, (-1 * topLeft.X) + fullSize.Width);
                            int qY = rand.Next(100, (-1 * topLeft.Y) + fullSize.Height);

                            if (makeRedLines)
                            {
                                formGraphics.DrawLine(redPen, pX, pY, qX, qY);
                            }
                            else
                            {
                                formGraphics.DrawLine(whitePen, pX, pY, qX, qY);
                            }

                            Thread.Sleep(10);
                        }
                    }
                }
            }
        }
        private void bw_DoWork_RetrieveData(object sender, DoWorkEventArgs e)
        {
            BLCard blCard = new BLCard();
            blCard.UpdateCardData();
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
    }
}
