using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static RuScavMods.TranslationFile;

namespace RuScavMods
{
    public static class TranslationManager
    {
        public static Dictionary<string, ItemTranslation> Items { get; } = new Dictionary<string, ItemTranslation>();
        public static Dictionary<string, AlertTranslation> Alerts { get; } = new Dictionary<string, AlertTranslation>();
        public static Dictionary<string, TalkTranslation> Talks { get; } = new Dictionary<string, TalkTranslation>();
        public static Dictionary<string, string> RunSettings { get; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Settings { get; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Quotes { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Tutorials { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> Buildings { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, ModuleTranslation> Modules { get; set; } = new Dictionary<string, ModuleTranslation>();
        public static Dictionary<string, LiquidTranslation> Liquids { get; set; } = new Dictionary<string, LiquidTranslation>();

        public static void AddDictionary<TKey, Value>(Dictionary<TKey, Value> source, Dictionary<TKey, Value> destination, Action<Value> beforeAdd = null)
        {
            if (source == null) return;
            foreach (var kvp in source)
            {
                beforeAdd?.Invoke(kvp.Value);
                destination[kvp.Key] = kvp.Value;
            }
        }
        public static void Add(TranslationFile translationFile)
        {
            if (translationFile == null) return;

            AddDictionary(translationFile.Items, Items);
            AddDictionary(translationFile.Settings, Settings);
            AddDictionary(translationFile.RunSettings, RunSettings);
            AddDictionary(translationFile.Quotes, Quotes);
            AddDictionary(translationFile.Tutorials, Tutorials);
            AddDictionary(translationFile.Buildings, Buildings);
            AddDictionary(translationFile.Modules, Modules);
            AddDictionary(translationFile.Liquids, Liquids);
            AddDictionary(translationFile.Alerts, Alerts, alert =>
            {
                if (alert.IsRegex && !string.IsNullOrEmpty(alert.Pattern))
                {
                    alert.CompiledRegex = new Regex(alert.Pattern, RegexOptions.Compiled);
                }
            });
            AddDictionary(translationFile.Talks, Talks, talk =>
            {
                if (talk.IsRegex && !string.IsNullOrEmpty(talk.Pattern))
                {
                    talk.CompiledRegex = new Regex(talk.Pattern, RegexOptions.Compiled);
                }
            });
        }

        public static void ApplyItemTranslations()
        {
            if (Items.Count == 0) return;
            var globalItems = Item.GlobalItems;
            if (globalItems == null || globalItems.Count == 0) return;
            foreach (var translation in Items)
            {
                if (globalItems.TryGetValue(translation.Key, out var itemInfo))
                {
                    if (!string.IsNullOrEmpty(translation.Value.Name))
                        itemInfo.fullName = translation.Value.Name;
                    if (!string.IsNullOrEmpty(translation.Value.Description))
                        itemInfo.description = translation.Value.Description;
                    else if (translation.Value.DescriptionPattern.Count > 0)
                    {
                        if (translation.Value.DescriptionPattern.Count != translation.Value.DescriptionReplace.Count)
                        {
                            Plugin.Log.LogWarning($"Количество description_pattern и description_replace не совпадает для \"{translation.Key}\".");
                            continue;
                        }
                        for (int i = 0; i < translation.Value.DescriptionPattern.Count; i++)
                        {
                            itemInfo.description = itemInfo.description.Replace(translation.Value.DescriptionPattern[i], translation.Value.DescriptionReplace[i]);
                        }
                    }
                }
            }
            return;
        }
        public static void ApplyBuildingTranslations()
        {
            if (Buildings.Count == 0) return;
            var currentLang = Locale.currentLang;
            if (currentLang == null) return;
            foreach (var translation in Buildings)
            {
                if (currentLang.buildings.TryGetValue(translation.Key, out _))
                {
                    if (!string.IsNullOrEmpty(translation.Value))
                        currentLang.buildings[translation.Key] = translation.Value;
                }
            }

        }
        public static void ApplyLiquidTranslations()
        {
            if (Liquids.Count == 0) return;
            var globalItems = Item.GlobalItems;
            if (globalItems == null || globalItems.Count == 0) return;
            foreach (var translation in Liquids)
            {
                var itemLiquidKey = translation.Key.Replace("dsc", "");
                if (globalItems.TryGetValue(itemLiquidKey, out var itemInfo))
                {
                    if (!(itemInfo is LiquidItemInfo liquidItemInfo)) continue;
                    if (liquidItemInfo.defaultContents == null) continue;
                    if (!string.IsNullOrEmpty(translation.Value.Description))
                        liquidItemInfo.description = translation.Value.Description;
                    else if (translation.Value.DescriptionPattern.Count > 0)
                    {
                        if (translation.Value.DescriptionPattern.Count != translation.Value.DescriptionReplace.Count)
                        {
                            Plugin.Log.LogWarning($"Количество description_pattern и description_replace не совпадает для \"{itemLiquidKey}\".");
                            continue;
                        }
                        for (int i = 0; i < translation.Value.DescriptionPattern.Count; i++)
                        {
                            liquidItemInfo.description = liquidItemInfo.description.Replace(translation.Value.DescriptionPattern[i], translation.Value.DescriptionReplace[i]);
                        }
                    }
                }
            }
        }
        public static void ApplyLiquidTranslationsGetString(ref string __result, string str)
        {
            if (Liquids.Count == 0) return;
            var globalItems = Item.GlobalItems;
            if (globalItems == null || globalItems.Count == 0) return;
            if (Liquids.TryGetValue(str, out var translation))
            {
                if (!string.IsNullOrEmpty(translation.Description))
                    __result = translation.Description;
                else if (translation.DescriptionPattern.Count > 0)
                {
                    if (translation.DescriptionPattern.Count != translation.DescriptionReplace.Count)
                    {
                        Plugin.Log.LogWarning($"Количество description_pattern и description_replace не совпадает для \"{str}\".");
                        return;
                    }
                    for (int i = 0; i < translation.DescriptionPattern.Count; i++)
                    {
                        __result = __result.Replace(translation.DescriptionPattern[i], translation.DescriptionReplace[i]);
                    }
                }

            }
            //foreach (var translation in Liquids)
            //{
            //    if (globalItems.TryGetValue(translation.Key, out var itemInfo))
            //    {
            //        if (!(itemInfo is LiquidItemInfo liquidItemInfo)) continue;
            //        if (liquidItemInfo.defaultContents == null) continue;
            //        if (!string.IsNullOrEmpty(translation.Value.Description))
            //            liquidItemInfo.description = translation.Value.Description;
            //        else if (translation.Value.DescriptionPattern.Count > 0)
            //        {
            //            if (translation.Value.DescriptionPattern.Count != translation.Value.DescriptionReplace.Count)
            //            {
            //                Plugin.Log.LogWarning($"Количество description_pattern и description_replace не совпадает для \"{translation.Key}\".");
            //                continue;
            //            }
            //            for (int i = 0; i < translation.Value.DescriptionPattern.Count; i++)
            //            {
            //                liquidItemInfo.description = liquidItemInfo.description.Replace(translation.Value.DescriptionPattern[i], translation.Value.DescriptionReplace[i]);
            //            }
            //        }
            //    }
            //}
        }
        public static void ApplyAlertTranslations(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (Alerts.Count == 0) return;

            foreach (var alert in Alerts.Values)
            {
                if (string.IsNullOrEmpty(alert.Pattern) || string.IsNullOrEmpty(alert.Translation)) continue;
                if (alert.IsRegex)
                {
                    var match = alert.CompiledRegex?.Match(text);
                    if (match != null && match.Success)
                    {
                        text = alert.Translation;
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            string groupValue = match.Groups[i].Value;
                            if (groupValue.Contains("feet"))
                                groupValue = groupValue.Replace("feet", "ног");
                            if (groupValue.Contains("arm"))
                                groupValue = groupValue.Replace("hands", "рук");
                            text = text.Replace($"${i}", groupValue);
                        }
                        return;
                    }
                }
                else
                {
                    if (text == alert.Pattern)
                    {
                        text = alert.Translation;
                        return;
                    }
                }
            }
        }
        public static void ApplyTalkTranslations(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (Talks.Count == 0) return;

            foreach (var talk in Talks.Values)
            {
                if (string.IsNullOrEmpty(talk.Pattern) || string.IsNullOrEmpty(talk.Translation)) continue;
                if (talk.IsRegex)
                {
                    var match = talk.CompiledRegex?.Match(text);
                    if (match != null && match.Success)
                    {
                        text = talk.Translation;
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            string groupValue = match.Groups[i].Value;
                            if (groupValue.Contains("feet"))
                                groupValue = groupValue.Replace("feet", "ног");
                            if (groupValue.Contains("arm"))
                                groupValue = groupValue.Replace("hands", "рук");
                            text = text.Replace($"${i}", groupValue);
                        }
                        return;
                    }
                }
                else
                {
                    if (text == talk.Pattern)
                    {
                        text = talk.Translation;
                        return;
                    }
                }
            }
        }
        public static void ApplyUITranslations(ref string __result, string str)
        {
            if (Settings.Count == 0) return; // Пока-что только для Settings, но можно расширить на другие словари при необходимости
            if (str == null) return;
            if (Settings.TryGetValue(str, out string text))
            {
                __result = text;
                return;
            }
        }
        public static void ApplyRunSettingsTranslations()
        {
            if (RunSettings.Count == 0) return;
            Language currentLang = Locale.currentLang;

            foreach (var translation in RunSettings)
            {
                if (currentLang.other.TryGetValue(translation.Key, out _))
                {
                    if (!string.IsNullOrEmpty(translation.Value))
                        currentLang.other[translation.Key] = translation.Value;
                }
            }
        }
        public static void ApplyQuoteTranslations(ref string __result)
        {
            if (Quotes.Count == 0) return;
            if (Quotes.TryGetValue(__result, out var quote))
            {
                __result = quote;
            }
        }
        public static void ApplyTutorialTranslations(List<TextMeshProUGUI> courseTexts)
        {
            if (courseTexts == null || courseTexts.Count == 0) return;
            foreach (var courseText in courseTexts)
            {
                if (TranslationManager.Tutorials.TryGetValue(courseText.text, out var tutorialName))
                {
                    courseText.text = tutorialName;
                }
            }
        }
    }
}

