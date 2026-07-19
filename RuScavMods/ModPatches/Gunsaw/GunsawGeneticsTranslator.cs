using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QoL_Unknown;
using RuScavMods.ModPatches.QoLUnknown;
using RuScavMods.ModPatches.QoLUnknown.Camera;
using RuScavMods.ModPatches.QoLUnknown.SaveMenu;
using RuScavMods.TranslationClasses.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.Gunsaw
{
    public class GunsawTranslationDef
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }

    }
    internal static class GunsawGeneticsTranslator
    {
        private readonly static List<PatchClassProcessor> GGPatches = new List<PatchClassProcessor>();
        public static readonly Type Localization = AccessTools.TypeByName("GunsawGenetics.Localization");
        public static readonly Type SpeciesPickerUI = AccessTools.TypeByName("GunsawGenetics.SpeciesPickerUI");
        public static Dictionary<string, string> Translations;
        private static readonly FieldInfo StringsField = AccessTools.Field(GunsawGeneticsTranslator.Localization, "Strings");
        public static void Translate()
        {
            if (Translations == null)
                FillTranslatedStrings();
            RegisterPatches();
            foreach (var processor in GGPatches)
            {
                try
                {
                    processor.Patch();
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogError($"Ошибка патча: {ex.Message}");
                }
            }
            ApplyTranslations();
        }

        private static void RegisterPatches()
        {
            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.gunsawgeneticstranslate");
            GGPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(LoadLanguagePatch)),
                new PatchClassProcessor(harmony, typeof(FindFontPatch))
            });
        }

        public static void ApplyTranslations()
        {
            var strings = (Dictionary<string, string>)StringsField.GetValue(null);
            if (strings == null)
                return;

            foreach (var pair in Translations)
            {
                strings[pair.Key] = pair.Value;
            }
        }
        private static void FillTranslatedStrings()
        {
            var translatePath = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "gunsaw.json");
            if (!File.Exists(translatePath)) return;
            var json = File.ReadAllText(translatePath);
            var jObject = JObject.Parse(json);
            Translations = jObject["translations"]?.ToObject<List<GunsawTranslationDef>>()?.ToDictionary(x => x.Key, x => x.Value) ?? throw new Exception("Секция translations не найдена или пуста.");

        }
    }

    [HarmonyPatch]
    internal static class LoadLanguagePatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(GunsawGeneticsTranslator.Localization, "LoadLanguage");
        [HarmonyPostfix]
        static void Postfix()
        {
            GunsawGeneticsTranslator.ApplyTranslations();
        }
    }

    [HarmonyPatch]
    internal static class FindFontPatch
    {
        private static TMP_FontAsset _russianFont;
        static MethodBase TargetMethod() => AccessTools.Method(GunsawGeneticsTranslator.SpeciesPickerUI, "FindFont");
        [HarmonyPostfix]
        static void Postfix(ref TMP_FontAsset ___gameFont)
        {
            if (_russianFont == null)
            {
                _russianFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                    .FirstOrDefault(f => f.name == "Retro GamingPix");
            }

            if (_russianFont != null)
                ___gameFont = _russianFont;

            Plugin.Log.LogInfo("Подменён шрифт для Gunsaw Genetics");
        }
    }
}
