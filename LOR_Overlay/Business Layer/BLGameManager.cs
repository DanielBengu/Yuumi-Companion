using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuumiCompanion.LOR_Overlay.Model;

namespace YuumiCompanion.LOR_Overlay.Business_Layer
{
    internal class BLGameManager
    {
        public List<CardCanvas> DeckList { get; set; }

        public void ManageGame()
        {
            DeckList = BLApi.GetDeckList();

            RefreshOverlay();
        }

        private void RefreshOverlay()
        {
            //TODO: CLEAR PREVIOUS DECK BEFORE UPDATING
            BLOverlay.UpdateCurrentDecklist(DeckList); 
        }

        
    }
}
