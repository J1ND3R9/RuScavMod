using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.Assertions.Must;

namespace RuScavMods
{
    [BepInPlugin("zzz_com.Floydba.ruscavmods", "RuScavMod", "1.0.2.1")]
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
            bool krokoshaMpInstalled = Chainloader.PluginInfos.ContainsKey("KrokoshaCasualtiesMP");
            if (qolModInstalled)
                QoLTranslator.Translate();
            if (krokoshaMpInstalled)
                KrokoshaMPTranslator.Translate();
        }
    }

    [HarmonyPatch(typeof(Item))]
    [HarmonyPriority(Priority.Last)]
    internal class ItemTranslationPatches
    {
        [HarmonyPatch(nameof(Item.SetupItems))]
        [HarmonyPostfix]
        private static void SetupItemsPostfix()
        {
            TranslationManager.ApplyItemTranslations();
            TranslationManager.ApplyBuildingTranslations();
            TranslationManager.ApplyLiquidTranslations();
        }
    }

    [HarmonyPatch(typeof(PlayerCamera))]
    [HarmonyPriority(Priority.Last)]
    internal class AlertTranslationPatches
    {
        [HarmonyPatch(nameof(PlayerCamera.DoAlert))]
        [HarmonyPrefix]
        private static void DoAlertPrefix(ref string text)
        {
            TranslationManager.ApplyAlertTranslations(ref text);
        }
    }

    [HarmonyPatch(typeof(Talker))]
    [HarmonyPriority(Priority.Last)]
    internal class TalkTranslationPatches
    {
        [HarmonyPatch(nameof(Talker.Talk), new[] { typeof(string), typeof(Limb), typeof(bool), typeof(bool) })]
        [HarmonyPrefix]
        private static void TalkPrefix(ref string text)
        {
            TranslationManager.ApplyTalkTranslations(ref text);
        }
    }

    [HarmonyPatch(typeof(Locale))]
    [HarmonyPriority(Priority.Last)]
    internal class GetStringTranslationPatches
    {
        [HarmonyPatch(nameof(Locale.GetString))]
        [HarmonyPostfix]
        private static void GetStringPostfix(ref string __result, string str)
        {
            TranslationManager.ApplyUITranslations(ref __result, str);
            TranslationManager.ApplyLiquidTranslationsGetString(ref __result, str);
        }
    }

    [HarmonyPatch(typeof(Locale))]
    [HarmonyPriority(Priority.Last)]
    internal class LocaleTranslationPatches
    {
        [HarmonyPatch(nameof(Locale.LoadLanguage))]
        [HarmonyPostfix]
        private static void LoadLanguagePostfix()
        {
            AddCustomQuotes();
            TranslationManager.ApplyRunSettingsTranslations();
        }
        private static readonly List<string> CustomQuotes = new List<string>
        {
            "Сосал?",
            "Упал - вставай\nВстал - упай\nУпай - чокопай\nНе суди, не гуди, не будай\nВада буди дабуди дабудай.",
            "В камине в 6 утра,",
            "фотография твоя...",
            "Чтобы поставить на паузу, скажите \"Бурмалда\".",
            "Экспи не будет счастлив, смирись.",
            "Это не Малевелон-Крик...",
            "nah, i'd win.",
            "ZA WARUDO!!!! TOKI WO TOMARE!!"
        };
        private static void AddCustomQuotes()
        {
            Language currentLang = Locale.currentLang;
            foreach (var quote in CustomQuotes)
                currentLang.pauseQuotes.Add(quote);
        }

        [HarmonyPatch(nameof(Locale.GetPauseQuote))]
        [HarmonyPostfix]
        private static void GetPauseQuotePostfix(ref string __result)
        {
            TranslationManager.ApplyQuoteTranslations(ref __result);
        }
    }

    [HarmonyPatch(typeof(TutorialHandler))]
    [HarmonyPriority(Priority.Last)]
    internal class TutorialTranslationPatches
    {
        [HarmonyPatch(nameof(TutorialHandler.HandleCourseSelect))]
        [HarmonyPostfix]
        private static void HandleCourseSelectPostfix(List<TextMeshProUGUI> ___courseTexts)
        {
            TranslationManager.ApplyTutorialTranslations(___courseTexts);
        }
    }
}