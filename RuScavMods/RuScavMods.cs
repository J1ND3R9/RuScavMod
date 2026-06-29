using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
    }
    public static class TranslationManager
    {
        public static Dictionary<string, ItemTranslation> GlobalItems { get; } = new Dictionary<string, ItemTranslation>();
        public static Dictionary<string, AlertTranslation> GlobalAlerts { get; } = new Dictionary<string, AlertTranslation>();
        public static Dictionary<string, TalkTranslation> GlobalTalks { get; } = new Dictionary<string, TalkTranslation>();
        public static Dictionary<string, string> GlobalKeybinds { get; } = new Dictionary<string, string>();
        public static Dictionary<string, string> GlobalRunSettings { get; } = new Dictionary<string, string>();
        public static Dictionary<string, OptionTranslation> GlobalOptions { get; } = new Dictionary<string, OptionTranslation>();
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
       //     ▓▒                                                     
       //    ░░░░░░░░░                                               
       //    ▓░  ░ ░░░░░▒             ▒▒▒▒▒▒░▒░                      expie UwU
       //    ▒░░ ░░ ░ ░ ░░░░   ▒▒▒░░░░░░░ ░░░░░                      
       //    ▒░ ░ ░░░░░░ ░░░░░░ ░ ░ ░ ░ ░ ░ ░░                 время 7 часов утра      
       //    ▓░░░▒▒▒▒▒▒░ ░ ░ ░ ░░░ ░ ░ ░ ░ ░░░               а я рот ебал        
       //     ░▒    ▒█▓░░ ░ ░ ░ ░▒▒▒▒░░ ░░░░░                        реверс инженеринг
       //     ▓▒   ███▓░░░ ░░░░▒▓░  █▓░░░░░                       во все   
       //   ▓░░█▓▓████▓░ ░░ ░░▓▒   ░██▒░░░            щели               
       //   ▓░░█▓██░ ▒▒░░ ░ ░░░ ██████░▒▒▒▒                          
       //    ▒░▒░▓▓░ ▒▒ ░░ ░ ░▒ ▓█▓▒█▓░ ░▒                  я часа 4 пытался         
       //     ▓▒▓▓▒▒░░░ ░ ░░░░▒     █░░▒▒                            
       //       ▒▒░░ ░ ░ ░ ░ ░ ░▒▓█▓░▓              изменить не тот словарь                 
       //         ▓░░ ░░░ ░ ░ ░░░█                             чтобы за 3 минуты      
       //       ░▒▒▓░░▒▒░░░░░░ ░░░░▓                                 
       //  ▓▒░░▒░ ░░▒▓▒░▒▒▒▒░░▓▒░ ░ ░▒▒                              когда я понял что я не то делал
       // ▒░░░ ░ ░ ░░░▒▒▒▒▒▒▒░░░ ░ ░ ░░░▒▓▓▒░           сделать правильно             
       // ▓░ ░░ ░░░░░░▒▓░░░░░ ░ ░░░░░ ░ ░ ░ ░▒                       
       //▒░░░ ░░ ░░▒▒░░▒▓░░░ ░ ░ ░░▓░░ ░░ ░░░░▒▓▒▒▒                  
       //    ▓ ░ ░░▒▓▒░░▒▒░░░ ░░ ░ ▓▒░░ ░░░▒▓▒▒░░▒       ░▒  ▒       
       //             ▒░ ░░ ░░ ░ ░░▒█▒░░ ░░▒▓░░ ░▒▓▒░▒▒              
       //             ░░░ ░ ▒░░ ░ ░▒▓░▒▒▒░░░░ ░░ ░░ ▒░              
       //              ▒░░ ░▓▒░░ ░ ░█░ ░ ░░ ░░ ░░ ░░     ░▒          
       //              ▒░ ░░▓▓░░░░░ ░▓▓░░ ░░ ░░░░▓                   
       //            ░▒░░░░▒  ░░ ░░░▓▒░▒  ░░░░░░░░░▒░             убейте меня...   
       //             ░ ░                                                                                                                                                                                                      
    internal static class QoLTranslator
    {
        public static void TranslateOptions()
        {
            var log = Plugin.Log;
            try
            {
                var modOptionsMenuType = AccessTools.TypeByName("QoL_Unknown.ModOptionsMenu");
                var optionType = AccessTools.TypeByName("QoL_Unknown.ModOptionDefinition");

                var definitionsField = AccessTools.Field(modOptionsMenuType, "DefinitionsById");
                var dict = (IDictionary)definitionsField.GetValue(null);

                var labelField = AccessTools.Field(optionType, "Label");
                var descField = AccessTools.Field(optionType, "Description");
                foreach (var pair in TranslationManager.GlobalOptions)
                {
                    if (!dict.Contains(pair.Key))
                        continue;
                    object option = dict[pair.Key];

                    labelField.SetValue(option, pair.Value.Label);
                    descField.SetValue(option, pair.Value.Description);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Ошибка перевода настроек QoL_Unknown: " + ex.Message);
                return;
            }
        }
    }
}