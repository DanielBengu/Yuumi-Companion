using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YuumiCompanion.LOR_Overlay.Deserialization
{
    internal class Card
    {
        public string cardCode { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<CardGallery> assets { get; set; }
        public List<string> associatedCardRefs { get; set; }
        public List<string> regionRefs { get; set; }
        public int attack { get; set; }
        public int cost { get; set; }
        public int health { get; set; }
        public string descriptionRaw { get; set; }
        public string levelupDescriptionRaw { get; set; }
        public List<string> keywords { get; set; }

        /*[JsonProperty(PropertyName = "24h_volume_usd")]   //since in c# variable names cannot begin with a number, you will need to use an alternate name to deserialize
        public string volume_usd_24h { get; set; }*/
    }
}
