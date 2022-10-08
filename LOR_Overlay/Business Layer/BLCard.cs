using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Windows.Forms;
using TranparentOverlay_simpleExample.LOR_Overlay.Deserialization;
using TransparentOverlay_simpleExample;
using Newtonsoft.Json;
using System.Net;
using System.ComponentModel;

namespace TranparentOverlay_simpleExample.LOR_Overlay.Business_Layer
{
    internal class BLCard
    {
        ListBox enemyCardList;
        PictureBox pictureBox;

        Form Form1;

        public BLCard()
        {
            Form1 = Application.OpenForms["Form1"];
            pictureBox = Form1.Controls["PictureBox1"] as PictureBox;
            enemyCardList = Form1.Controls["EnemyCardList"] as ListBox;
        }

        delegate void AddCardCallback();

        private List<Card> RetrieveData()
        {
            string json = new WebClient().DownloadString("https://localhost:7149/Card");

            return JsonConvert.DeserializeObject<List<Card>>(json);
        }

        public void UpdateCardData()
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (enemyCardList.InvokeRequired)
            {
                AddCardCallback d = new AddCardCallback(UpdateCardData);
                Form1.Invoke(d, new object[] { });
            }
            else
            {
                List<Card> cardList = RetrieveData();

                enemyCardList.DataSource = cardList.Select(card => card.name).ToList();
                pictureBox.ImageLocation = cardList.First().assets.First().gameAbsolutePath;
            }
        }
    }
}
