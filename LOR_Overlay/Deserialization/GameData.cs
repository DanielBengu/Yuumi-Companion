using System;
using System.Collections.Generic;
using System.Text;

namespace YuumiCompanion.LOR_Overlay.Deserialization
{
    internal class GameData
    {
        public string PlayerName { get; set; }
        public string OpponentName { get; set; }
        public GameStateEnum GameState { get; set; }
        public object Rectangles { get; set; }


        public enum GameStateEnum
        {
            Menus,
            InProgress,
            NotFound,
        }
    }
}
