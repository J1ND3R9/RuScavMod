using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RuScavMods
{
    public class TranslationFile
    {
        public class ModuleTranslation
        {
            [JsonProperty("label")]
            public string Label { get; set; } = string.Empty;
            [JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }
        public class ItemTranslation
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;
            [JsonProperty("description_pattern")]
            public List<string> DescriptionPattern { get; set; } = new List<string>();
            [JsonProperty("description_replace")]
            public List<string> DescriptionReplace { get; set; } = new List<string>();

            [JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }

        public class AlertTranslation
        {
            [JsonProperty("regex")]
            public bool IsRegex { get; set; } = false;

            [JsonProperty("pattern")]
            public string Pattern { get; set; } = string.Empty;

            [JsonProperty("translation")]
            public string Translation { get; set; } = string.Empty;

            [JsonIgnore]
            public Regex CompiledRegex { get; set; }
        }

        public class TalkTranslation
        {
            [JsonProperty("regex")]
            public bool IsRegex { get; set; } = false;

            [JsonProperty("pattern")]
            public string Pattern { get; set; } = string.Empty;

            [JsonProperty("translation")]
            public string Translation { get; set; } = string.Empty;

            [JsonIgnore]
            public Regex CompiledRegex { get; set; }
        }

        public class BuildingTranslation
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            [JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }
        public class LiquidTranslation
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;
            [JsonProperty("description_pattern")]
            public List<string> DescriptionPattern { get; set; } = new List<string>();
            [JsonProperty("description_replace")]
            public List<string> DescriptionReplace { get; set; } = new List<string>();

            [JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }

        [JsonProperty("items")]
        public Dictionary<string, ItemTranslation> Items { get; set; } = new Dictionary<string, ItemTranslation>();
        [JsonProperty("alerts")]
        public Dictionary<string, AlertTranslation> Alerts { get; set; } = new Dictionary<string, AlertTranslation>();
        [JsonProperty("talks")]
        public Dictionary<string, TalkTranslation> Talks { get; set; } = new Dictionary<string, TalkTranslation>();
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
        [JsonProperty("modules")]
        public Dictionary<string, ModuleTranslation> Modules { get; set; } = new Dictionary<string, ModuleTranslation>();
        [JsonProperty("liquids")]
        public Dictionary<string, LiquidTranslation> Liquids { get; set; } = new Dictionary<string, LiquidTranslation>();

    }
}