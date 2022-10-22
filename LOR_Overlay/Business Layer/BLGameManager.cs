using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YuumiCompanion.LOR_Overlay.Deserialization;
using YuumiCompanion.LOR_Overlay.Model;
using static YuumiCompanion.LOR_Overlay.Deserialization.GameData;

namespace YuumiCompanion.LOR_Overlay.Business_Layer
{
    internal class BLGameManager
    {
        private List<CardCanvas> _deckList;

        private readonly BLOverlay blOverlay;

        public BLGameManager()
        {
            blOverlay = new BLOverlay();
            _deckList = new List<CardCanvas>();
        }

        public void ManageGame(GameStateEnum gameState)
        {
            switch (gameState)
            {
                case GameStateEnum.Menus:
                case GameStateEnum.NotFound:
                    blOverlay.ClearCurrentDecklist();
                    break;

                case GameStateEnum.InProgress:
                    List<CardCanvas> deckList = BLApi.GetDeckList();
                    if (deckList != null)
                        RefreshOverlay(deckList);
                    break;
            } 
        }

        private void RefreshOverlay(List<CardCanvas> deckList)
        {
            if(_deckList != null && !(_deckList.ToString().Equals(deckList.ToString())))
            {
                blOverlay.ClearCurrentDecklist();
                _deckList.Clear();
            }
                
            blOverlay.RefreshCurrentDecklist(deckList, _deckList); 
        }
    }
}
