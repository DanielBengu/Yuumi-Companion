using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using YuumiCompanion.LOR_Overlay.Business_Layer;
using YuumiCompanion.LOR_Overlay.Deserialization;

namespace YuumiCompanion
{

    public partial class Form1 : Form
    {
        BLGameManager blGameManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BLOverlay blOverlay = new BLOverlay();
            blOverlay.StartupOverlay();

            blGameManager = new BLGameManager();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            GameData gd = BLApi.GetGameData();

            if (gd != null)
                blGameManager.ManageGame(gd.GameState);
        }
    }
}