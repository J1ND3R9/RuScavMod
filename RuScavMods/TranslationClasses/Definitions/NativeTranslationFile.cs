using Newtonsoft.Json;
using System.Collections.Generic;

namespace RuScavMods.TranslationClasses.Definitions
{
    internal class NativeTranslationFile
    {
        public class NameDescription
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }

        public class ItemTranslation : NameDescription
        {
            [JsonProperty("description_pattern")]
            public List<string> DescriptionPattern { get; set; } = new List<string>();
            [JsonProperty("description_replace")]
            public List<string> DescriptionReplace { get; set; } = new List<string>();
        }

        [JsonProperty("items")]
        public Dictionary<string, ItemTranslation> Items { get; set; } = new Dictionary<string, ItemTranslation>();
        [JsonProperty("liquids")]
        public Dictionary<string, ItemTranslation> Liquids { get; set; } = new Dictionary<string, ItemTranslation>();
        [JsonProperty("moodles")]
        public Dictionary<string, string> Moodles { get; set; } = new Dictionary<string, string>();
        [JsonProperty("alerts")]
        public Dictionary<string, string> Alerts { get; set; } = new Dictionary<string, string>();
        [JsonProperty("talks")]
        public Dictionary<string, string> Talks { get; set; } = new Dictionary<string, string>();
        [JsonProperty("settings")]
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        [JsonProperty("runSettings")]
        public Dictionary<string, string> RunSettings { get; set; } = new Dictionary<string, string>();
        [JsonProperty("quotes")]
        public Dictionary<string, string> Quotes { get; set; } = new Dictionary<string, string>();
        [JsonProperty("tutorials")]
        public Dictionary<string, string> Tutorials { get; set; } = new Dictionary<string, string>();
        [JsonProperty("buildings")]
        public Dictionary<string, string> Buildings { get; set; } = new Dictionary<string, string>();
    }
}
