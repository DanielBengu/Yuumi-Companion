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
        private List<CardCanvas> _deckList;

        private readonly BLOverlay bLOverlay;

        public BLGameManager()
        {
            bLOverlay = new BLOverlay();
        }

        public void ManageGame()
        {
            _deckList = BLApi.GetDeckList();

            RefreshOverlay(_deckList);
        }

        private void RefreshOverlay(List<CardCanvas> deckList)
        {
            //TODO: CLEAR PREVIOUS DECK BEFORE UPDATING
            bLOverlay.RefreshCurrentDecklist(_deckList); 
        }

        
    }
}
