using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using YuumiCompanion.LOR_Overlay.Deserialization;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using static YuumiCompanion.LOR_Overlay.Deserialization.GameData;
using System.Reflection;
using System.DirectoryServices.ActiveDirectory;
using Newtonsoft.Json.Linq;
using YuumiCompanion.LOR_Overlay.Model;

namespace YuumiCompanion.LOR_Overlay.Business_Layer
{
    internal class BLApi
    {
        private static string baseDragonPath = ConfigurationManager.AppSettings.GetValues("dataDragonPath").First();
        private static string baseLocalPath = ConfigurationManager.AppSettings.GetValues("localDataPath").First();

        private static string dragonCardPath = "/Card";

        private static string localCardPositionPath = "/positional-rectangles";
        private static string localDecklistPath = "/static-decklist";

        public static List<CardCanvas> GetDeckList()
        {
            try
            {
                string json = new WebClient().DownloadString(baseLocalPath + localDecklistPath);
                List<CardCanvas> result = ConvertObjectToDecklist(json);

                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static GameData GetGameData()
        {
            try
            {
                string json = new WebClient().DownloadString(baseLocalPath + localCardPositionPath);

                return JsonConvert.DeserializeObject<GameData>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<Card> GetListOfCards(string cardList)
        {
            try
            {
                WebClient client = new WebClient();
                client.QueryString.Add("requestType", "1");
                client.QueryString.Add("list", cardList);

                string json = client.DownloadString(baseDragonPath + dragonCardPath);
                return JsonConvert.DeserializeObject<List<Card>>(json);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static List<CardCanvas> ConvertObjectToDecklist(string json)
        {
            List<CardCanvas> result = new List<CardCanvas>();
            List<Card> cardList;
            string res = String.Empty;

            //TODO: understand and optimize this part
            var ct = JObject.Parse(json).Children<JProperty>().Where(t => t.Name.Equals("CardsInDeck")).First().First().Value<JObject>();

            if (ct == null)
                return null;

            string[] cards = ct.Properties().Select(t => t.Name).ToArray();
            int[] copies = ct.Properties().Select(t => (int)t.Value).ToArray();

            for (int i = 0; i < cards.Length; i++)
                for (int j = 0; j < copies[i]; j++)
                    res += cards[i] + ',';

            cardList = GetListOfCards(res);
            result.AddRange(cardList.GroupBy(c => c.cardCode).Select(group => new CardCanvas { Card = cardList.Find(c => c.cardCode.Equals(group.Key)), Quantity = group.Count()}));

            return result;
        }
    }
}
