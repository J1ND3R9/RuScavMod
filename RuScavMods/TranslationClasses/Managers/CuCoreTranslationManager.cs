using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;


namespace RuScavMods.TranslationClasses.Managers
{
    internal class CuCoreTranslationManager
    {
        public static Dictionary<string, string> Items { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Buildings { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Moodles { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Others { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Logs { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Commands { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Liquids { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Titles { get; set; } = new Dictionary<string, string>();
        public static void Add(CuCoreTranslationFile file)
        {
            if (file == null) throw new Exception("Файл перевода пустой или не десериализован.");
            AddDictionary(file.Items, Items);
            AddDictionary(file.Buildings, Buildings);
            AddDictionary(file.Moodles, Moodles);
            AddDictionary(file.Others, Others);
            AddDictionary(file.Logs, Logs);
            AddDictionary(file.Commands, Commands);
            AddDictionary(file.Options, Options);
            AddDictionary(file.Liquids, Liquids);
            AddDictionary(file.Titles, Titles);
        }
        private static void AddDictionary<TKey, Value>(Dictionary<TKey, Value> source, Dictionary<TKey, Value> destination, Action<Value> beforeAdd = null)
        {
            if (source == null) return;
            foreach (var kvp in source)
            {
                beforeAdd?.Invoke(kvp.Value);
                destination[kvp.Key] = kvp.Value;
            }
        }

        public static void ApplyLocaleTranslation(Language lang)
        {
            if (Buildings.Count == 0 && Others.Count == 0) return;

            foreach (var building in Buildings)
            {
                if (lang.buildings.TryGetValue(building.Key, out _))
                {
                    if (!string.IsNullOrEmpty(building.Value))
                        lang.buildings[building.Key] = building.Value;
                }
            }
            foreach (var other in Others)
            {
                if (lang.other.TryGetValue(other.Key, out _))
                {
                    if (!string.IsNullOrEmpty(other.Value))
                        lang.other[other.Key] = other.Value;
                }
            }
        }

        public static void ApplyItemTranslation(Dictionary<string, ItemInfo> globalItems)
        {
            foreach (var item in Items)
            {
                if (globalItems.TryGetValue(item.Key, out var itemInfo))
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        itemInfo.fullName = item.Value;
                        if (Items.TryGetValue(item.Key + "dsc", out string description))
                            itemInfo.description = description;
                    }
                }
            }
        }
    }
}
