using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static RuScavMods.TranslationFile;

namespace RuScavMods
{
    [BepInPlugin("zzz_com.Floydba.ruscavmods", "RuScavMod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private void Awake()
        {
            Plugin.Log = base.Logger;
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods");
            LoadTranslationFiles();
            ApplyHarmonyPatches(harmony);
            ApplyLateTranslation();
        }

        private void LoadTranslationFiles()
        {
            var pluginsInfo = Chainloader.PluginInfos;
            var pathTranslations = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations");
            foreach (var pluginInfo in pluginsInfo)
            {
                var pluginGuid = pluginInfo.Key;
                var translationFilePath = Path.Combine(pathTranslations, $"{pluginGuid}.json");
                if (File.Exists(translationFilePath))
                {
                    try
                    {
                        var translationContent = File.ReadAllText(translationFilePath);
                        var translationData = JsonConvert.DeserializeObject<TranslationFile>(translationContent);
                        if (translationData == null)
                        {
                            Log.LogError($"Файл перевода {pluginGuid} пустой или неверно форматирован.");
                            continue;
                        }
                        TranslationManager.Add(translationData);
                        Log.LogInfo($"Загружен файл перевода для мода {pluginInfo.Value.Metadata.Name}.");
                    }
                    catch (Exception ex)
                    {
                        Log.LogError($"Ошибка загрузки файла перевода {pluginGuid}. Ошибка: {ex.Message}");
                    }
                }
            }
        }

        private void ApplyHarmonyPatches(Harmony harmony)
        {
            foreach (var type in typeof(Plugin).Assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(HarmonyPatch), false).Length == 0)
                    continue;
                try
                {
                    Log.LogInfo($"Патчим {type.FullName}");
                    harmony.CreateClassProcessor(type).Patch();
                }
                catch (Exception ex)
                {
                    Log.LogError($"Ошибка патча {type.FullName}");
                    Log.LogError(ex);
                }
            }
        }
        private void ApplyLateTranslation()
        {
            bool qolModInstalled = Chainloader.PluginInfos.ContainsKey("org.bepinex.plugins.qol_unknown");
            if (qolModInstalled)
                QoLTranslator.TranslateOptions();
        }
    }

    public class TranslationFile
    {
        public class ItemTranslation
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

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
        public class OptionTranslation
        {
            [JsonProperty("label")]
            public string Label { get; set; } = string.Empty;

            [JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }

        [JsonProperty("items")]
        public Dictionary<string, ItemTranslation> Items { get; set; } = new Dictionary<string, ItemTranslation>();
        [JsonProperty("alerts")]
        public Dictionary<string, AlertTranslation> Alerts { get; set; } = new Dictionary<string, AlertTranslation>();
        [JsonProperty("talks")]
        public Dictionary<string, TalkTranslation> Talks { get; set; } = new Dictionary<string, TalkTranslation>();
        [JsonProperty("modKeyBind")]
        public Dictionary<string, string> Keybinds { get; set; } = new Dictionary<string, string>();
        [JsonProperty("runSettings")]
        public Dictionary<string, string> RunSettings { get; set; } = new Dictionary<string, string>();
        [JsonProperty("options")]
        public Dictionary<string, OptionTranslation> Options { get; set; } = new Dictionary<string, OptionTranslation>();
        [JsonProperty("quotes")]
        public Dictionary<string, string> Quotes { get; set; } = new Dictionary<string, string>();
        [JsonProperty("tutorials")]
        public Dictionary<string, string> Tutorials { get; set; } = new Dictionary<string, string>();
    }
    public static class TranslationManager
    {
        public static Dictionary<string, ItemTranslation> GlobalItems { get; } = new Dictionary<string, ItemTranslation>();
        public static Dictionary<string, AlertTranslation> GlobalAlerts { get; } = new Dictionary<string, AlertTranslation>();
        public static Dictionary<string, TalkTranslation> GlobalTalks { get; } = new Dictionary<string, TalkTranslation>();
        public static Dictionary<string, string> GlobalKeybinds { get; } = new Dictionary<string, string>();
        public static Dictionary<string, string> GlobalRunSettings { get; } = new Dictionary<string, string>();
        public static Dictionary<string, OptionTranslation> GlobalOptions { get; } = new Dictionary<string, OptionTranslation>();
        public static Dictionary<string, string> GlobalQuotes { get; set; } = new Dictionary<string, string>();
        public static Dictionary<string, string> GlobalTutorials { get; set; } = new Dictionary<string, string>();
        public static void Add(TranslationFile translationFile)
        {
            if (translationFile == null) return;

            if (translationFile.Items != null)
            {
                foreach (var kvp in translationFile.Items)
                {
                    GlobalItems[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.Alerts != null)
            {
                foreach (var kvp in translationFile.Alerts)
                {
                    var alert = kvp.Value;
                    if (alert.IsRegex && !string.IsNullOrEmpty(alert.Pattern))
                    {
                        alert.CompiledRegex = new Regex(alert.Pattern, RegexOptions.Compiled);
                    }
                    GlobalAlerts[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.Talks != null)
            {
                foreach (var kvp in translationFile.Talks)
                {
                    var talk = kvp.Value;
                    if (talk.IsRegex && !string.IsNullOrEmpty(talk.Pattern))
                    {
                        talk.CompiledRegex = new Regex(talk.Pattern, RegexOptions.Compiled);
                    }
                    GlobalTalks[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.Keybinds != null)
            {
                foreach (var kvp in translationFile.Keybinds)
                {
                    GlobalKeybinds[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.RunSettings != null)
            {
                foreach (var kvp in translationFile.RunSettings)
                {
                    GlobalRunSettings[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.Options != null)
            {
                foreach (var kvp in translationFile.Options)
                {
                    GlobalOptions[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.Quotes != null)
            {
                foreach (var kvp in translationFile.Quotes)
                {
                    GlobalQuotes[kvp.Key] = kvp.Value;
                }
            }

            if (translationFile.Tutorials != null)
            {
                foreach (var kvp in translationFile.Tutorials)
                {
                    GlobalTutorials[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Item), nameof(Item.SetupItems))]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalItemTranslationPatch
    {
        private static void Postfix()
        {
            if (TranslationManager.GlobalItems.Count == 0) return;

            foreach (var translation in TranslationManager.GlobalItems)
            {
                if (Item.GlobalItems.TryGetValue(translation.Key, out var itemInfo))
                {
                    if (!string.IsNullOrEmpty(translation.Value.Name))
                        itemInfo.fullName = translation.Value.Name;

                    if (!string.IsNullOrEmpty(translation.Value.Description))
                        itemInfo.description = translation.Value.Description;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.DoAlert))]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalAlertsTranslationPatch
    {
        private static void Prefix(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (TranslationManager.GlobalAlerts.Count == 0) return;

            foreach (var alert in TranslationManager.GlobalAlerts.Values)
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
                            text = text.Replace($"${i}", match.Groups[i].Value);
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
    }

    [HarmonyPatch(typeof(Talker), nameof(Talker.Talk), new[] { typeof(string), typeof(Limb), typeof(bool), typeof(bool) })]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalTalksTranslationPatch
    {
        private static void Prefix(ref string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (TranslationManager.GlobalTalks.Count == 0) return;

            foreach (var talk in TranslationManager.GlobalTalks.Values)
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
                            text = text.Replace($"${i}", match.Groups[i].Value);
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
    }

    [HarmonyPatch(typeof(Locale), nameof(Locale.GetString))]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalKeybindsTranslationPatch
    {
        private static void Postfix(ref string __result, string str)
        {
            if (TranslationManager.GlobalKeybinds.Count == 0) return;
            if (str == null) return;
            if (TranslationManager.GlobalKeybinds.TryGetValue(str, out string text))
            {
                __result = text;
                return;
            }
        }
    }
    [HarmonyPatch(typeof(Locale), nameof(Locale.LoadLanguage))]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalRunSettingsTranslationPatch
    {
        private static void Postfix()
        {
            if (TranslationManager.GlobalRunSettings.Count == 0) return;
            Language currentLang = Locale.currentLang;
            foreach (var translation in TranslationManager.GlobalRunSettings)
            {
                if (currentLang.other.TryGetValue(translation.Key, out _))
                {
                    if (!string.IsNullOrEmpty(translation.Value))
                        currentLang.other[translation.Key] = translation.Value;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Locale), nameof(Locale.GetPauseQuote))]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalPauseQuoteTranslationPatch
    {
        private static void Postfix(ref string __result)
        {
            if (TranslationManager.GlobalQuotes.Count == 0) return;
            if (TranslationManager.GlobalQuotes.TryGetValue(__result, out var quote))
            {
                __result = quote;
            }
        }
    }

    [HarmonyPatch(typeof(TutorialHandler), "Update")]
    [HarmonyPriority(Priority.Last)]
    internal class GlobalTutorialsTranslationPatch
    {
        private static void Postfix()
        {
            if (TutorialHandler.main == null) return;
            if (TranslationManager.GlobalTutorials.TryGetValue(TutorialHandler.main.courseText.text, out var tutorialName))
            {
                TutorialHandler.main.courseText.text = tutorialName;
            }
        }
    }
    //     ▓▒                                                     
    //    ░░░░░░░░░                                               
    //    ▓░  ░ ░░░░░▒             ▒▒▒▒▒▒░▒░   omg!!! hi expie!!!!!! what are u doing here?!??!?????
    //    ▒░░ ░░ ░ ░ ░░░░   ▒▒▒░░░░░░░ ░░░░░                      
    //    ▒░ ░ ░░░░░░ ░░░░░░ ░ ░ ░ ░ ░ ░ ░░    its not new layer, silly!!!! go back!!!!!!!                
    //    ▓░░░▒▒▒▒▒▒░ ░ ░ ░ ░░░ ░ ░ ░ ░ ░░░                    
    //     ░▒    ▒█▓░░ ░ ░ ░ ░▒▒▒▒░░ ░░░░░                     
    //     ▓▒   ███▓░░░ ░░░░▒▓░  █▓░░░░░                      
    //   ▓░░█▓▓████▓░ ░░ ░░▓▒   ░██▒░░░                        
    //   ▓░░█▓██░ ▒▒░░ ░ ░░░ ██████░▒▒▒▒                          
    //    ▒░▒░▓▓░ ▒▒ ░░ ░ ░▒ ▓█▓▒█▓░ ░▒                      
    //     ▓▒▓▓▒▒░░░ ░ ░░░░▒     █░░▒▒                            
    //       ▒▒░░ ░ ░ ░ ░ ░ ░▒▓█▓░▓                           
    //         ▓░░ ░░░ ░ ░ ░░░█                         
    //       ░▒▒▓░░▒▒░░░░░░ ░░░░▓                                 
    //  ▓▒░░▒░ ░░▒▓▒░▒▒▒▒░░▓▒░ ░ ░▒▒                              
    // ▒░░░ ░ ░ ░░░▒▒▒▒▒▒▒░░░ ░ ░ ░░░▒▓▓▒░                     
    // ▓░ ░░ ░░░░░░▒▓░░░░░ ░ ░░░░░ ░ ░ ░ ░▒                       
    //▒░░░ ░░ ░░▒▒░░▒▓░░░ ░ ░ ░░▓░░ ░░ ░░░░▒▓▒▒▒                  
    //    ▓ ░ ░░▒▓▒░░▒▒░░░ ░░ ░ ▓▒░░ ░░░▒▓▒▒░░▒       ░▒  ▒       
    //             ▒░ ░░ ░░ ░ ░░▒█▒░░ ░░▒▓░░ ░▒▓▒░▒▒              
    //             ░░░ ░ ▒░░ ░ ░▒▓░▒▒▒░░░░ ░░ ░░ ▒░              
    //              ▒░░ ░▓▒░░ ░ ░█░ ░ ░░ ░░ ░░ ░░     ░▒          
    //              ▒░ ░░▓▓░░░░░ ░▓▓░░ ░░ ░░░░▓                   
    //            ░▒░░░░▒  ░░ ░░░▓▒░▒  ░░░░░░░░░▒░        
    //             ░ ░                                                                                                                                                                                                      
    internal static class QoLTranslator
    {
        private static readonly Type ModOptionsMenu = AccessTools.TypeByName("QoL_Unknown.ModOptionsMenu");
        private static readonly Type ModOptionDefinition = AccessTools.TypeByName("QoL_Unknown.ModOptionDefinition");
        private static readonly Type ModCustomSettingDefinition = AccessTools.TypeByName("QoL_Unknown.ModCustomSettingDefinition");
        private static readonly Type QoLModuleManager = AccessTools.TypeByName("QoL_Unknown.QoLModuleManager");
        private static readonly Type QoLModuleDef = AccessTools.TypeByName("QoL_Unknown.QoLModuleDef");

        private static readonly IDictionary DefinitionsById = (IDictionary)AccessTools.Field(ModOptionsMenu, "DefinitionsById").GetValue(null);
        private static readonly IList CustomDefinitions = (IList)AccessTools.Field(ModOptionsMenu, "CustomDefinitions").GetValue(null);
        private static readonly IList Modules = (IList)AccessTools.Field(QoLModuleManager, "Modules").GetValue(null);

        private static readonly FieldInfo ModOptionDefinitionLabel = AccessTools.Field(ModOptionDefinition, "Label");
        private static readonly FieldInfo ModOptionDefinitionDescription = AccessTools.Field(ModOptionDefinition, "Description");
        private static readonly FieldInfo ModCustomSettingDefinitionId = AccessTools.Field(ModCustomSettingDefinition, "Id");
        private static readonly FieldInfo ModCustomSettingDefinitionLabel = AccessTools.Field(ModCustomSettingDefinition, "Label");
        private static readonly FieldInfo ModCustomSettingDefinitionDescription = AccessTools.Field(ModCustomSettingDefinition, "Description");
        private static readonly FieldInfo QoLModuleDefID = AccessTools.Field(QoLModuleDef, "ID");
        private static readonly FieldInfo QoLModuleDefName = AccessTools.Field(QoLModuleDef, "Name");
        private static readonly FieldInfo QoLModuleDefDescription = AccessTools.Field(QoLModuleDef, "Description");

        public static void TranslateOptions()
        {
            var log = Plugin.Log;
            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.qolunknowntranslate");
            try
            {
                TranslateDictionary(DefinitionsById, ModOptionDefinitionLabel, ModOptionDefinitionDescription);
                TranslateList(CustomDefinitions, ModCustomSettingDefinitionId, ModCustomSettingDefinitionLabel, ModCustomSettingDefinitionDescription);
                TranslateList(Modules, QoLModuleDefID, QoLModuleDefName, QoLModuleDefDescription);
                TranslateSaveSlots(harmony);
            }
            catch (Exception ex)
            {
                log.LogError($"Ошибка перевода настроек QoL_Unknown: " + ex.Message);
                return;
            }
        }
        private static void TranslateSaveSlots(Harmony harmony)
        {
            var saveSaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveSaveMenuPatcher");
            var saveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveMenuPatcher");
            if (saveSaveMenuPatcher == null || saveMenuPatcher == null) return;

            var refreshSlotsOriginal = AccessTools.Method(saveSaveMenuPatcher, "RefreshSlots");
            var buildSaveUIOriginal = AccessTools.Method(saveSaveMenuPatcher, "BuildSaveUI");
            var buildSlotTooltipOriginal = AccessTools.Method(saveMenuPatcher, "BuildSlotTooltip");

            harmony.Patch(
                refreshSlotsOriginal,
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.RefreshSlotsPostfix)));
            harmony.Patch(
                buildSaveUIOriginal,
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.BuildSaveUIPostfix)));
            harmony.Patch(
                buildSlotTooltipOriginal,
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.BuildSlotTooltipPostfix)));

        }

        private static void TranslateDictionary(
            IDictionary dict,
            FieldInfo labelField,
            FieldInfo descField
            )
        {
            foreach (var pair in TranslationManager.GlobalOptions)
            {
                if (!dict.Contains(pair.Key))
                    continue;
                var item = dict[pair.Key];

                labelField.SetValue(item, pair.Value.Label);
                descField.SetValue(item, pair.Value.Description);
            }
        }

        private static void TranslateList(
            IList list,
            FieldInfo idField,
            FieldInfo labelField,
            FieldInfo descField
            )
        {
            foreach (var item in list)
            {
                var id = (string)idField.GetValue(item);

                if (!TranslationManager.GlobalOptions.TryGetValue(id, out var translation))
                    continue;

                labelField.SetValue(item, translation.Label);
                descField.SetValue(item, translation.Description);
            }
        }

        private static class Patches
        {
            private static readonly Type SaveSaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveSaveMenuPatcher");
            private static readonly Type SaveMenuPatcher = AccessTools.TypeByName("QoL_Unknown.SaveMenuPatcher");
            private static readonly FieldInfo SlotContainerField = AccessTools.Field(SaveSaveMenuPatcher, "slotContainer");
            public static void RefreshSlotsPostfix()
            {
                Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
                if (slotContainer == null) return;

                for (int i = 1; i < slotContainer.childCount; i++)
                {
                    var info = slotContainer.GetChild(i)
                        ?.Find("Content")
                        ?.Find("Info");

                    if (info == null) continue;

                    TextMeshProUGUI textMesh = info.GetComponent<TextMeshProUGUI>();

                    textMesh.text = textMesh.text
                        .Replace("Slot", "Слот")
                        .Replace("Empty", "Пустой")
                        .Replace("m Deep", "м Глубина")
                        .Replace("m deep", "м глубина")
                        .Replace("Corrupt", "Повреждено")
                        .Replace("run time", "время забега");
                }
            }
            public static void BuildSaveUIPostfix()
            {
                Transform slotContainer = (Transform)SlotContainerField.GetValue(null);
                var header = slotContainer.Find("Header");

                if (header != null)
                {
                    var text = header.GetComponent<TextMeshProUGUI>();
                    text.text = "СЛОТЫ СОХРАНЕНИЙ";
                }
            }
            public static void BuildSlotTooltipPostfix(ref string tipName, ref string tipDesc)
            {
                tipName = "Продолжить";
                tipDesc = tipDesc
                    .Replace("Resume your", "Продолжить")
                    .Replace("m expedition with Experiment #", "м экспедицию Эксперимента #")
                    .Replace("Seed", "Сид")
                    .Replace("In-game time", "Время в игре")
                    .Replace("Unknown", "[УДАЛЕНО]")
                    .Replace("No expedition data in Slot", "Нет экспедиции в слоте");
            }
        }
    }
}