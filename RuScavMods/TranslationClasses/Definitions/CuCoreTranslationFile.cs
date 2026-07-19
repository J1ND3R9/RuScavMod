using Newtonsoft.Json;
using System.Collections.Generic;

namespace RuScavMods.TranslationClasses.Definitions
{
    public class CuCoreTranslationFile
    {
        [JsonProperty("item")]
        public Dictionary<string, string> Items { get; set; } = new Dictionary<string, string>();
        [JsonProperty("building")]
        public Dictionary<string, string> Buildings { get; set; } = new Dictionary<string, string>();
        [JsonProperty("moodle")]
        public Dictionary<string, string> Moodles { get; set; } = new Dictionary<string, string>();
        [JsonProperty("other")]
        public Dictionary<string, string> Others { get; set; } = new Dictionary<string, string>();
        [JsonProperty("log")]
        public Dictionary<string, string> Logs { get; set; } = new Dictionary<string, string>();
        [JsonProperty("command")]
        public Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();
        [JsonProperty("option")]
        public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
        [JsonProperty("liquid")]
        public Dictionary<string, string> Liquids { get; set; } = new Dictionary<string, string>();
        [JsonProperty("title")]
        public Dictionary<string, string> Titles { get; set; } = new Dictionary<string, string>();
        [JsonProperty("native_talks")]
        public Dictionary<string, string> NativeTalks { get; set; } = new Dictionary<string, string>();
    }
}
