using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuumiCompanion.LOR_Overlay.Deserialization;

namespace YuumiCompanion.LOR_Overlay.Business_Layer
{
    internal class BLGameManager
    {
        public void ManageGame()
        {
            List<Card> decklist = BLApi.GetDeckList();

            RefreshOverlay(decklist);
        }

        private void RefreshOverlay(List<Card> decklist)
        {
            ListBox userCardList = (ListBox)Application.OpenForms["Form1"].Controls["UserCardList"];

            userCardList.DataSource = decklist.Select(card => card.name).ToList();
        }
    }
}
