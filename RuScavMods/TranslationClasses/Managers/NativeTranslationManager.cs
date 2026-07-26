using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using TMPro;
using static RuScavMods.TranslationClasses.Definitions.NativeTranslationFile;
using static System.Net.Mime.MediaTypeNames;


namespace RuScavMods.TranslationClasses.Managers
{
    internal class NativeTranslationManager
    {
        public class RegexTranslation
        {
            public string Translation { get; set; } = string.Empty;
            public Regex CompiledRegex { get; set; } = null;
        }
        public static Dictionary<string, ItemTranslation> Items { get; set; } = new Dictionary<string, ItemTranslation>();
        public static Dictionary<string, ItemTranslation> Liquids { get; set; } = new Dictionary<string, ItemTranslation>();
        public static Dictionary<string, string> Moodles { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Alerts { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Talks { get; set; } = new Dictionary<string, string>();
        public static List<RegexTranslation> RegexAlerts { get; set; } = new List<RegexTranslation>();
        public static List<RegexTranslation> RegexTalks { get; set; } = new List<RegexTranslation>();
        public static List<RegexTranslation> RegexMoodles { get; set; } = new List<RegexTranslation>();
        public static Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> RunSettings { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Quotes { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Tutorials { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Buildings { get; set; } = new Dictionary<string, string>();
        public static void Add(NativeTranslationFile file)
        {
            if (file == null) throw new Exception("Файл перевода пустой или не десериализован.");
            AddDictionary(file.Items, Items);
            //AddDictionary(file.Liquids, Liquids);
            AddDictionaryRegex(file.Alerts, Alerts, RegexAlerts);
            AddDictionaryRegex(file.Talks, Talks, RegexTalks);
            AddDictionaryRegex(file.Moodles, Moodles, RegexMoodles);
            AddDictionary(file.Moodles, Moodles);
            AddDictionary(file.Settings, Settings);
            AddDictionary(file.RunSettings, RunSettings);
            AddDictionary(file.Quotes, Quotes);
            AddDictionary(file.Tutorials, Tutorials);
            AddDictionary(file.Buildings, Buildings);
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

        private static void AddDictionaryRegex(Dictionary<string, string> source, Dictionary<string, string> destination, List<RegexTranslation> regexList)
        {
            if (source == null) return;
            foreach (var kvp in source)
            {
                if (LooksLikeRegex(kvp.Key))
                {
                    RegexTranslation regexTranslation = new RegexTranslation
                    {
                        Translation = kvp.Value,
                        CompiledRegex = new Regex(kvp.Key, RegexOptions.Compiled)
                    };
                    regexList.Add(regexTranslation);
                }
                else
                    destination[kvp.Key] = kvp.Value;
            }
        }


        private static bool LooksLikeRegex(string pattern) => pattern.StartsWith("^") || pattern.Contains("(.+)");

        public static void ApplyItemTranslation(Dictionary<string, ItemInfo> globalItems)
        {
            foreach (var itemDef in Items)
            {
                if (globalItems.TryGetValue(itemDef.Key, out var itemInfo))
                {
                    var itemTranslation = itemDef.Value;
                    if (!string.IsNullOrEmpty(itemTranslation.Name)) itemInfo.fullName = itemTranslation.Name;
                    if (!string.IsNullOrEmpty(itemTranslation.Description)) itemInfo.description = itemTranslation.Description;
                    else if (itemTranslation.DescriptionPattern.Count > 0)
                    {
                        if (itemTranslation.DescriptionPattern.Count != itemTranslation.DescriptionReplace.Count)
                        {
                            Plugin.Log.LogWarning($"Количество description_pattern и description_replace не совпадает для \"{itemDef.Key}\".");
                            continue;
                        }
                        for (int i = 0; i < itemTranslation.DescriptionPattern.Count; i++)
                        {
                            itemInfo.description = itemInfo.description.Replace(itemTranslation.DescriptionPattern[i], itemTranslation.DescriptionReplace[i]);
                        }
                    }
                }
            }

            foreach (var liquidDef in Liquids)
            {
                if (globalItems.TryGetValue(liquidDef.Key.Replace("dsc", ""), out var liquidInfo))
                {
                    var liquidTranslation = liquidDef.Value;
                    if (!string.IsNullOrEmpty(liquidTranslation.Name)) liquidInfo.fullName = liquidTranslation.Name;
                    if (!string.IsNullOrEmpty(liquidTranslation.Description)) liquidInfo.description = liquidTranslation.Description;
                    else if (liquidTranslation.DescriptionPattern.Count > 0)
                    {
                        if (liquidTranslation.DescriptionPattern.Count != liquidTranslation.DescriptionReplace.Count)
                        {
                            Plugin.Log.LogWarning($"Количество description_pattern и description_replace не совпадает для \"{liquidDef.Key}\".");
                            continue;
                        }
                        for (int i = 0; i < liquidTranslation.DescriptionPattern.Count; i++)
                        {
                            liquidInfo.description = liquidInfo.description.Replace(liquidTranslation.DescriptionPattern[i], liquidTranslation.DescriptionReplace[i]);
                        }
                    }
                }
            }
        }

        public static void ApplyBuildingTranslation(Language lang)
        {
            foreach (var building in Buildings)
            {
                if (lang.buildings.TryGetValue(building.Key, out _))
                {
                    if (!string.IsNullOrEmpty(building.Value))
                        lang.buildings[building.Key] = building.Value;
                }
            }
        }

        public static void ApplyAlertTranslation(ref string text)
        {
            if (Alerts.TryGetValue(text, out var translatedText))
                text = translatedText;
            else
            {
                foreach (var regexAlert in RegexAlerts)
                    if (TranslateRegex(ref text, regexAlert)) return;
            }
        }

        public static void ApplyTalkTranslation(ref string text)
        {
            if (Talks.TryGetValue(text, out var translatedText))
                text = translatedText;
            else
            {
                foreach (var regexTalk in RegexTalks)
                    if (TranslateRegex(ref text, regexTalk)) return;
            }
        }
        private static bool TranslateRegex(ref string text, RegexTranslation regexTranslation)
        {
            if (regexTranslation.CompiledRegex != null)
            {
                var match = regexTranslation.CompiledRegex.Match(text);
                if (match != null && match.Success)
                {
                    text = regexTranslation.Translation;
                    for (int i = 1; i < match.Groups.Count; i++)
                    {
                        string groupValue = match.Groups[i].Value;
                        text = text.Replace($"${i}", groupValue);
                    }
                    return true;
                }
            }
            return false;
        }
        public static void ApplyLocaleTranslation(Language lang)
        {
            foreach (var translation in RunSettings)
            {
                if (lang.other.TryGetValue(translation.Key, out _))
                {
                    if (!string.IsNullOrEmpty(translation.Value))
                        lang.other[translation.Key] = translation.Value;
                }
            }

            foreach (var translation in Settings) // Бывает что ещё настройки заливают в other
            {
                if (lang.other.TryGetValue(translation.Key, out _))
                {
                    if (!string.IsNullOrEmpty(translation.Value))
                        lang.other[translation.Key] = translation.Value;
                }
            }
        }

        public static void ApplyGetStringTranslation(ref string __result, string str)
        {
            if (Settings.TryGetValue(str, out string text))
                __result = text;
        }
        public static void ApplyPauseQuoteTranslation(ref string __result)
        {
            if (Quotes.TryGetValue(__result, out var text))
                __result = text;
        }

        public static void ApplyTutorialTranslation(List<TextMeshProUGUI> courseTexts)
        {
            foreach (var courseText in courseTexts)
            {
                if (Tutorials.TryGetValue(courseText.text, out var tutorialName))
                    courseText.text = tutorialName;
            }
        }
        public static void ApplyMoodleTranslation(ref string name, ref string desc)
        {
            bool nameTranslated = false;
            bool descTranslated = false;

            if (Moodles.TryGetValue(name, out var translationName))
            {
                name = translationName;
                nameTranslated = true;
            }
            if (Moodles.TryGetValue(desc, out var translationDesc))
            {
                desc = translationDesc;
                descTranslated = true;
            }
            if (!nameTranslated || !descTranslated)
            {
                foreach (var regex in RegexMoodles)
                {
                    TranslateRegex(ref name, regex);
                    TranslateRegex(ref desc, regex);
                }
            }
        }

        public static void AddNativeTalks(Dictionary<string, string> talks)
        {
            foreach (var talk in talks)
            {
                if (LooksLikeRegex(talk.Key))
                {
                    RegexTranslation regexTranslation = new RegexTranslation
                    {
                        Translation = talk.Value,
                        CompiledRegex = new Regex(talk.Key, RegexOptions.Compiled)
                    };
                    RegexTalks.Add(regexTranslation);
                }
                else
                    Talks[talk.Key] = talk.Value;
            }
        }

        public static void AddNativeAlerts(Dictionary<string, string> alerts)
        {
            foreach (var alert in alerts)
            {
                if (LooksLikeRegex(alert.Key))
                {
                    RegexTranslation regexTranslation = new RegexTranslation
                    {
                        Translation = alert.Value,
                        CompiledRegex = new Regex(alert.Key, RegexOptions.Compiled)
                    };
                    RegexAlerts.Add(regexTranslation);
                }
                else
                    Alerts[alert.Key] = alert.Value;
            }
        }

        public static void AddMoodleAlerts(Dictionary<string, string> moodles)
        {
            foreach (var moodle in moodles)
            {
                if (LooksLikeRegex(moodle.Key))
                {
                    RegexTranslation regexTranslation = new RegexTranslation
                    {
                        Translation = moodle.Value,
                        CompiledRegex = new Regex(moodle.Key, RegexOptions.Compiled)
                    };
                    RegexMoodles.Add(regexTranslation);
                }
                else
                    Moodles[moodle.Key] = moodle.Value;
            }
        }
    }
}

