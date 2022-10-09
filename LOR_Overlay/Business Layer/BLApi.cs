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

namespace YuumiCompanion.LOR_Overlay.Business_Layer
{
    internal class BLApi
    {
        private static string baseDragonPath = ConfigurationManager.AppSettings.GetValues("dataDragonPath").First();
        private static string baseLocalPath = ConfigurationManager.AppSettings.GetValues("localDataPath").First();

        private static string dragonCardPath = "/Card";

        private static string localCardPositionPath = "/positional-rectangles";
        private static string localDecklistPath = "/static-decklist";

        public static List<Card> GetDeckList()
        {
            try
            {
                string json = new WebClient().DownloadString(baseLocalPath + localDecklistPath);
                List<Card> result = ConvertObjectToDecklist(json);

                return result;
            }
            catch (WebException)
            {
                return null;
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }

        }

        public static GameData GetGameData()
        {
            try
            {
                string json = new WebClient().DownloadString(baseLocalPath + localCardPositionPath);

                return JsonConvert.DeserializeObject<GameData>(json);
            }
            catch (WebException)
            {
                return null;
            }
            catch(Exception)
            {
                throw new NotImplementedException();
            }
        }

        public static List<Card> GetAllCards()
        {
            try
            {
                string json = new WebClient().DownloadString(baseDragonPath + dragonCardPath);
                return JsonConvert.DeserializeObject<List<Card>>(json);
            }
            catch (WebException)
            {
                return null;
            }
            catch (Exception)
            {
                throw new NotImplementedException();
            }

        }

        public static List<Card> ConvertObjectToDecklist(string json)
        {
            List<Card> result = new List<Card>();
            List<Card> totalCardList = GetAllCards();
            Card card = new Card();

            //TODO: understand and optimize this part
            var ct = JObject.Parse(json).Children<JProperty>().Where(t => t.Name.Equals("CardsInDeck")).First().First().Value<JObject>();

            string[] cards = ct.Properties().Select(t => t.Name).ToArray();
            int[] copies = ct.Properties().Select(t => (int)t.Value).ToArray();

            for (int i = 0; i < cards.Count(); i++)
            {
                for (int j = 0; j < copies[i]; j++)
                {
                    card = totalCardList.Where(tcl => tcl.cardCode.Equals(cards[i])).FirstOrDefault();
                    if(card != null)
                        result.Add(card);
                }
            }

            return result;
        }
    }
}
