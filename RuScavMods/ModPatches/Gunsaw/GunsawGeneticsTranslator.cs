using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static Dictionary<string, string> Translations;
        private static readonly FieldInfo StringsField = AccessTools.Field(typeof(GunsawGenetics.Localization), "Strings");
        //private static readonly Dictionary<string, AbilityEntry[]> AbilityEntriesTranslation = new Dictionary<string, AbilityEntry[]>
        //{
        //    "Velvet", 
        //};
        public static void Translate()
        {
            if (Translations == null)
                FillTranslatedStrings();
            RegisterPatches();
            foreach (PatchClassProcessor processor in GGPatches)
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
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.gunsawgeneticstranslate");
            GGPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(LoadLanguagePatch)),
                new PatchClassProcessor(harmony, typeof(FindFontPatch))
            });
        }

        public static void ApplyTranslations()
        {
            Dictionary<string, string> strings = (Dictionary<string, string>)StringsField.GetValue(null);
            if (strings == null)
                return;

            foreach (KeyValuePair<string, string> pair in Translations)
            {
                strings[pair.Key] = pair.Value;
            }
        }
        private static void FillTranslatedStrings()
        {
            string translatePath = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "gunsaw.json");
            if (!File.Exists(translatePath)) return;
            string json = File.ReadAllText(translatePath);
            JObject jObject = JObject.Parse(json);
            Translations = jObject["translations"]?.ToObject<List<GunsawTranslationDef>>()?.ToDictionary(x => x.Key, x => x.Value) ?? throw new Exception("Секция translations не найдена или пуста.");

        }
    }

    [HarmonyPatch(typeof(GunsawGenetics.Localization))]
    internal static class LoadLanguagePatch
    {
        [HarmonyPatch(nameof(GunsawGenetics.Localization.LoadLanguage))]
        [HarmonyPostfix]
        static void Postfix()
        {
            GunsawGeneticsTranslator.ApplyTranslations();
        }
    }

    [HarmonyPatch(typeof(GunsawGenetics.SpeciesPickerUI))]
    internal static class FindFontPatch
    {
        private static TMP_FontAsset _russianFont;
        [HarmonyPatch("FindFont")]
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
