﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using YuumiCompanion.LOR_Overlay.Business_Layer;

namespace YuumiCompanion
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BLOverlay blOverlay = new BLOverlay();
            blOverlay.StartupOverlay();

            BLMultiThread blMT = new BLMultiThread();
            blMT.StartThreads();
        }

    }
}