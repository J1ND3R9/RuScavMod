using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuScavMods.ModPatches.AdditionalDescriptions;
using RuScavMods.ModPatches.QoLUnknown;
using RuScavMods.Patches;
using RuScavMods.TranslationClasses.Definitions;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using static BepInEx.BepInDependency;

namespace RuScavMods
{
    [BepInPlugin("zzz_com.Floydba.ruscavmods", "RuScavMod", "2.0.0.0")]
    [BepInDependency("org.bepinex.plugins.qol_unknown", DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        internal static bool IsCUCoreLibInstalled => Chainloader.PluginInfos.ContainsKey("net.cucorelib");
        internal static ManualLogSource Log;
        private readonly static List<PatchClassProcessor> VanillaPatches = new List<PatchClassProcessor>();

        private void Awake()
        {
            Plugin.Log = base.Logger;
            LoadTranslationFiles();
            ApplyVanillaPatches();
            ApplyModTranslations();
        }

        private void LoadTranslationFiles()
        {
            var pluginsInfo = Chainloader.PluginInfos;
            var pathTranslations = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations");
            foreach (var plugin in pluginsInfo)
            {
                var pluginId = plugin.Key;
                var translationFile = Path.Combine(pathTranslations, $"{pluginId}.json");
                if (File.Exists(translationFile))
                {
                    try
                    {
                        string json = File.ReadAllText(translationFile);
                        TranslationType? type = GetJsonType(json) ?? throw new Exception("GetJsonType вернул null!");
                        if (type == TranslationType.Custom)
                            continue;
                        if (type == TranslationType.Native)
                            LoadNativeTranslation(json);
                        else if (type == TranslationType.CuCore)
                            LoadCuCoreTranslation(json);
                    }
                    catch (Exception ex)
                    {
                        Log.LogError($"Произошла ошибка при загрузке перевода {plugin.Value.Metadata.Name}: {ex.Message}");
                    }
                }
            }
        }
        private void RegisterVanillaPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.vanillapatches");
            VanillaPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(AlertTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(GetStringTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(ItemTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(LocaleTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(MoodleManagerPatches)),
                new PatchClassProcessor(harmony, typeof(TalkTranslationPatches)),
                new PatchClassProcessor(harmony, typeof(TutorialTranslationPatches)),
            });


        }

        private void LoadNativeTranslation(string json)
        {
            var translationData = JsonConvert.DeserializeObject<NativeTranslationFile>(json) ?? throw new Exception("Файл не был десериализован.");
            NativeTranslationManager.Add(translationData);
        }
        private void LoadCuCoreTranslation(string json)
        {
            var translationData = JsonConvert.DeserializeObject<CuCoreTranslationFile>(json) ?? throw new Exception("Файл не был десериализован.");
            CuCoreTranslationManager.Add(translationData);
        }

        private TranslationType? GetJsonType(string json)
        {
            JObject obj = JObject.Parse(json);
            string type = obj["type"]?.ToString();
            if (type == null)
                return null;
            switch (type)
            {
                case "native":
                    return TranslationType.Native;
                case "custom":
                    return TranslationType.Custom;
                case "cucore":
                    return TranslationType.CuCore;
                default:
                    break;
            }
            return null;
        }
        private void ApplyVanillaPatches()
        {
            RegisterVanillaPatches();
            foreach (var processor in VanillaPatches)
            {
                try
                {
                    processor.Patch();
                    Log.LogError($"Патчим {processor}");

                }
                catch (Exception ex)
                {
                    Log.LogError($"Ошибка патча {processor}: {ex.Message}");
                }
            }
        }
        

        private void ApplyModTranslations()
        {
            try
            {
                if (Chainloader.PluginInfos.ContainsKey("org.bepinex.plugins.qol_unknown"))
                {
                    QoLTranslator.Translate();
                    Log.LogInfo("Переводим QoL: Unknown");
                }
                if (Chainloader.PluginInfos.ContainsKey("noah.casualtiesunknown.additionaldescriptions"))
                {
                    AdditionalDescriptionsTranslator.Translate();
                    Log.LogInfo("Переводим AdditionalDescriptions");

                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
            
        }
    }
}

//using BepInEx;
//using BepInEx.Bootstrap;
//using BepInEx.Logging;
//using HarmonyLib;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using RuScavMods.ReflectPatches;
//using RuScavMods.TranslationClasses.Managers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using TMPro;
//using UnityEngine.Assertions.Must;

//namespace RuScavMods
//{
//    [BepInPlugin("zzz_com.Floydba.ruscavmods", "RuScavMod", "2.0.0.0")] // Нужен рефактор
//    public class Plugin : BaseUnityPlugin
//    {
//        internal static ManualLogSource Log;
//        internal static bool IsCUCoreLibInstalled => Chainloader.PluginInfos.ContainsKey("net.cucorelib");
//        private void Awake()
//        {
//            Plugin.Log = base.Logger;
//            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods");
//            LoadTranslationFiles();
//            ApplyHarmonyPatches(harmony);
//            ApplyLateTranslation();
//        }
//        private void LoadTranslationFiles()
//        {
//            var pluginsInfo = Chainloader.PluginInfos;
//            foreach (var pluginInfo in pluginsInfo)
//            {
//                var pluginGuid = pluginInfo.Key;
//                var pathTranslations = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations");
//                var translationFilePath = Path.Combine(pathTranslations, $"{pluginGuid}.json");

//                if (File.Exists(translationFilePath))
//                {
//                    try
//                    {
//                        var translationContent = File.ReadAllText(translationFilePath);
//                        var translationData = JsonConvert.DeserializeObject<TranslationFile>(translationContent);
//                        if (translationData == null)
//                        {
//                            Log.LogError($"Файл перевода {pluginGuid} пустой или неверно форматирован.");
//                            continue;
//                        }
//                        TranslationManager.Add(translationData);
//                        Log.LogInfo($"Загружен файл перевода для мода {pluginInfo.Value.Metadata.Name}.");
//                    }
//                    catch (Exception ex)
//                    {
//                        Log.LogError($"Ошибка загрузки файла перевода {pluginGuid}. Ошибка: {ex.Message}");
//                    }
//                }

//                {
//                    if (IsCUCoreLibInstalled)
//                    {
//                        bool isCoreLibDependency = pluginInfo.Value.Dependencies.Any(p => p.DependencyGUID == "net.cucorelib");
//                        if (isCoreLibDependency)
//                        {
//                            var pathTranslationsCoreLib = Path.Combine(Paths.PluginPath, "RuScavMods", "CUCoreTranslations");
//                            var translationCoreLibFilePath = Path.Combine(pathTranslationsCoreLib, $"{pluginGuid}.json");
//                            if (File.Exists(translationCoreLibFilePath))
//                            {
//                                try
//                                {
//                                    var translationContent = File.ReadAllText(translationCoreLibFilePath);
//                                    var translationData = JsonConvert.DeserializeObject<TranslationFileCuCoreLib>(translationContent);
//                                    if (translationData == null)
//                                    {
//                                        Log.LogError($"Файл перевода {pluginGuid} пустой или неверно форматирован.");
//                                        continue;
//                                    }
//                                    TranslationCUCoreLibManager.Add(translationData);
//                                    Log.LogInfo($"Загружен файл перевода для мода {pluginInfo.Value.Metadata.Name}.");
//                                }
//                                catch (Exception ex)
//                                {
//                                    Log.LogError($"Ошибка загрузки файла перевода {pluginGuid}. Ошибка: {ex.Message}");
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        private void ApplyHarmonyPatches(Harmony harmony)
//        {
//            foreach (var type in typeof(Plugin).Assembly.GetTypes())
//            {
//                if (type.GetCustomAttributes(typeof(HarmonyPatch), false).Length == 0)
//                    continue;
//                try
//                {
//                    Log.LogInfo($"Патчим {type.FullName}");
//                    harmony.CreateClassProcessor(type).Patch();
//                }
//                catch (Exception ex)
//                {
//                    Log.LogError($"Ошибка патча {type.FullName}");
//                    Log.LogError(ex);
//                }
//            }
//        }
//        private void ApplyLateTranslation()
//        {
//            bool qolModInstalled = Chainloader.PluginInfos.ContainsKey("org.bepinex.plugins.qol_unknown");
//            bool krokoshaMpInstalled = Chainloader.PluginInfos.ContainsKey("KrokoshaCasualtiesMP");
//            bool gunsawGeneticsInstalled = Chainloader.PluginInfos.ContainsKey("GunsawGenetics");
//            if (qolModInstalled)
//                QoLTranslator.Translate();
//            if (krokoshaMpInstalled)
//                KrokoshaMPTranslator.Translate();
//            if (gunsawGeneticsInstalled)
//                GunsawGeneticsTranslator.Translate();
//        }
//    }

//    [HarmonyPatch(typeof(Item))]
//    [HarmonyPriority(Priority.Last)]
//    internal class ItemTranslationPatches
//    {
//        [HarmonyPatch(nameof(Item.SetupItems))]
//        [HarmonyPostfix]
//        private static void SetupItemsPostfix()
//        {
//            TranslationManager.ApplyItemTranslations();
//            TranslationManager.ApplyBuildingTranslations();
//            TranslationManager.ApplyLiquidTranslations();
//            if (Plugin.IsCUCoreLibInstalled)
//                TranslationCUCoreLibManager.ApplyItemTranslation();
//        }
//    }

//    [HarmonyPatch(typeof(PlayerCamera))]
//    [HarmonyPriority(Priority.Last)]
//    internal class AlertTranslationPatches
//    {
//        [HarmonyPatch(nameof(PlayerCamera.DoAlert))]
//        [HarmonyPrefix]
//        private static void DoAlertPrefix(ref string text)
//        {
//            TranslationManager.ApplyAlertTranslations(ref text);
//        }
//    }

//    [HarmonyPatch(typeof(Talker))]
//    [HarmonyPriority(Priority.Last)]
//    internal class TalkTranslationPatches
//    {
//        [HarmonyPatch(nameof(Talker.Talk), new[] { typeof(string), typeof(Limb), typeof(bool), typeof(bool) })]
//        [HarmonyPrefix]
//        private static void TalkPrefix(ref string text)
//        {
//            TranslationManager.ApplyTalkTranslations(ref text);
//        }
//    }

//    [HarmonyPatch(typeof(Locale))]
//    [HarmonyPriority(Priority.Last)]
//    internal class GetStringTranslationPatches
//    {
//        [HarmonyPatch(nameof(Locale.GetString))]
//        [HarmonyPostfix]
//        private static void GetStringPostfix(ref string __result, string str)
//        {
//            TranslationManager.ApplyUITranslations(ref __result, str);
//            TranslationManager.ApplyLiquidTranslationsGetString(ref __result, str);
//        }
//    }

//    [HarmonyPatch(typeof(Locale))]
//    [HarmonyPriority(Priority.Last)]
//    internal class LocaleTranslationPatches
//    {
//        [HarmonyPatch(nameof(Locale.LoadLanguage))]
//        [HarmonyPostfix]
//        private static void LoadLanguagePostfix()
//        {
//            AddCustomQuotes();
//            TranslationManager.ApplyRunSettingsTranslations();
//            if (Plugin.IsCUCoreLibInstalled)
//                TranslationCUCoreLibManager.ApplyTranslation();
//        }
//        private static readonly List<string> CustomQuotes = new List<string>
//        {
//            //"Упал - вставай\nВстал - упай\nУпай - чокопай\nНе суди, не гуди, не будай\nВада буди дабуди дабудай.",
//            //"В камине в 6 утра,",
//            //"фотография твоя...",
//            //"Чтобы поставить на паузу, скажите \"Бурмалда\".",
//            //"Экспи не будет счастлив, смирись.",
//            //"Это не Малевелон-Крик...",
//            //"nah, i'd win.",
//            //"ZA WARUDO!!!! TOKI WO TOMARE!!",
//            //"Послушай меня, я не хочу быть твоим другом. Я хочу быть твоей тенью.",
//            //"Мой любимый исполнитель - \"LAZZY2WICE\". Его треки отличаются агрессией, тяжелыми гитарными риффами и глубоким, мрачным басом. Мой любимый трек \"забракованный\".",
//            //"Моя любимая книга - \"Божественная комедия\" Данте Алигьери. Она рассказывает о путешествии через Ад, Чистилище и Рай, и я нахожу её очень вдохновляющей.",
//            #region "Также попробуйте" рекомендации
//            "Также попробуйте Rain World!",
//            "Также попробуйте The Binding of Isaac!",
//            "Также попробуйте Barotrauma!",
//            "Также попробуйте ULTRAKILL!",
//            "Также попробуйте Loop Hero!",
//            "Также попробуйте Euro Truck Simulator 2!",
//            "Также попробуйте Lobotomy Corporation!",
//            "Также попробуйте Noita!",
//            "Не пробуйте Dota 2.",
//            "Не пробуйте League of Legends.",
//            "Не пробуйте Counter-Strike 2.",
//            #endregion
//            #region Стихи
//            "«У Лукоморья дуб зелёный, золотая цепь на дубе том...»",
//            "«Я помню чудное мгновенье: передо мной явилась ты, как мимолетное виденье, как гений чистой красоты.»",
//            "«Мой дядя самых честных правил, когда не в шутку занемог, он уважать себя заставил и лучше выдумать не мог.»",
//            "«Я вас любил: любовь ещё, быть может, в душе моей угасла не совсем; но пусть она вас больше не тревожит; я не хочу печалить вас ничем.»",
//            "«Не жалею, не зову, не плачу, все пройдет, как с белых яблонь дым.»",
//            "«Ты жива ещё, моя старушка? Жив и я. Привет тебе, привет!»",
//            "«Если крикнет рать святая: \"Кинь ты Русь, живи в раю!\". Я скажу: \"Не надо рая, дайте родину мою\"»",
//            #endregion
//            #region Остальное
//            "О, тебе нравится Экспи? Тогда жду в личных сообщениях топ 100 фактов о нём.",
//            "Может быть по фентанилу?",
//            "Сосал?",
//            #endregion
//        };
//        private static void AddCustomQuotes()
//        {
//            Language currentLang = Locale.currentLang;
//            foreach (var quote in CustomQuotes)
//                currentLang.pauseQuotes.Add(quote);
//        }

//        [HarmonyPatch(nameof(Locale.GetPauseQuote))]
//        [HarmonyPostfix]
//        private static void GetPauseQuotePostfix(ref string __result)
//        {
//            TranslationManager.ApplyQuoteTranslations(ref __result);
//        }
//    }

//    [HarmonyPatch(typeof(TutorialHandler))]
//    [HarmonyPriority(Priority.Last)]
//    internal class TutorialTranslationPatches
//    {
//        [HarmonyPatch(nameof(TutorialHandler.HandleCourseSelect))]
//        [HarmonyPostfix]
//        private static void HandleCourseSelectPostfix(List<TextMeshProUGUI> ___courseTexts)
//        {
//            TranslationManager.ApplyTutorialTranslations(___courseTexts);
//        }
//    }

//    [HarmonyPatch(typeof(MoodleManager))]
//    [HarmonyPriority(Priority.Last)]
//    internal class MoodleManagerPatches
//    {
//        [HarmonyPatch(nameof(MoodleManager.AddMoodle))]
//        [HarmonyPrefix]
//        private static void AddMoodlePrefix(ref string name, ref string desc)
//        {
//            TranslationManager.ApplyMoodleTranslation(ref name, ref desc);
//        }
//    }
//}