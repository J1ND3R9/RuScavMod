using CUCoreLib.Data;
using CUCoreLib.Registries;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RuScavMods.ModPatches.CUCoreLib
{
    internal static class CUCoreTranslator
    {
        private readonly static List<PatchClassProcessor> CUCorePatches = new List<PatchClassProcessor>();
        public static readonly Type LocaleLoader = AccessTools.TypeByName("CUCoreLib.Helpers.LocaleLoader");

        public static void Translate()
        {
            RegisterPatches();
            foreach (PatchClassProcessor processor in CUCorePatches)
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
            TranslateBuildings();
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.cucoretranslation");

            CUCorePatches.AddRange(new[] {
                new PatchClassProcessor(harmony, typeof(LocaleLoaderMergeLocaleJsonPatch)),
                //new PatchClassProcessor(harmony, typeof(LocaleRegisterPatch)),
                new PatchClassProcessor(harmony, typeof(LocaleLoaderGetLocalizedTextPatch)),
                new PatchClassProcessor(harmony, typeof(MoodleRegistryAddMoodlePatch))
            });
        }

        private static void TranslateBuildings()
        {
            FieldInfo field = AccessTools.Field(typeof(BuildingEntityRegistry), "RegisteredDefinitions");

            Dictionary<string, CustomBuildingEntityDefinition> defs = (Dictionary<string, CustomBuildingEntityDefinition>)field.GetValue(null);

            foreach (KeyValuePair<string, CustomBuildingEntityDefinition> pair in defs)
            {
                bool changed = false;

                if (CuCoreTranslationManager.CuCoreTranslations["building"].TryGetValue(pair.Key, out string name))
                {
                    pair.Value.Name = name;
                    changed = true;
                }

                if (CuCoreTranslationManager.CuCoreTranslations["building"].TryGetValue(pair.Key + "dsc", out string desc))
                {
                    pair.Value.Description = desc;
                    changed = true;
                }

                if (changed)
                {
                    LocaleRegistry.Register("building", pair.Key, pair.Value.Name);
                    LocaleRegistry.Register("building", pair.Key + "dsc", pair.Value.Description);
                }
            }
        }
    }

    [HarmonyPatch]
    internal static class LocaleLoaderMergeLocaleJsonPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(CUCoreTranslator.LocaleLoader, "MergeLocaleJson");

        [HarmonyPrefix]
        private static void Prefix(ref JObject source)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> category in CuCoreTranslationManager.CuCoreTranslations)
            {
                if (source[category.Key] == null)
                    source[category.Key] = new JObject();

                JObject section = (JObject)source[category.Key];

                foreach (KeyValuePair<string, string> pair in category.Value)
                {
                    section[pair.Key] = pair.Value;
                }
            }
        }
    }

    [HarmonyPatch(typeof(MoodleRegistry))]
    internal static class MoodleRegistryAddMoodlePatch
    {
        [HarmonyPatch(nameof(MoodleRegistry.AddMoodle), new[] { typeof(int), typeof(Sprite), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(string), typeof(float) })]
        [HarmonyPostfix]
        private static void AddMoodlePostfix(ref string name, ref string description)
        {
            if (CuCoreTranslationManager.CuCoreTranslations["moodle"].TryGetValue(name, out string translatedName))
            {
                name = translatedName;
            }
            if (CuCoreTranslationManager.CuCoreTranslations["moodle"].TryGetValue(name + "dsc", out string translatedDescription))
            {
                description = translatedDescription;
            }
        }
    }

    // Fallback.
    // Некоторые версии CUCore/моды обходят MergeSection или вызывают его
    // до установки патча. Поэтому дополнительно подменяем результат здесь.

    [HarmonyPatch]
    internal static class LocaleLoaderGetLocalizedTextPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(CUCoreTranslator.LocaleLoader, "GetLocalizedText");

        [HarmonyPostfix]
        private static void Postfix(string category, string key, ref string __result)
        {
            if (category == null)
                return;

            if (CuCoreTranslationManager.CuCoreTranslations.TryGetValue(category, out Dictionary<string, string> dict) &&
                dict.TryGetValue(key, out string value))
            {
                __result = value;
            }
        }
    }
}
