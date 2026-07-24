using BepInEx;
using CUCoreLib.Data;
using CUCoreLib.Registries;
using HarmonyLib;
using KrokoshaCasualtiesMP;
using RuScavMods.ModPatches.QoLUnknown.Camera;
using RuScavMods.ModPatches.QoLUnknown.SaveMenu;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            foreach (var processor in CUCorePatches)
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
            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.cucoretranslation");

            CUCorePatches.AddRange(new[] {
                new PatchClassProcessor(harmony, typeof(LocaleLoaderMergeSectionPatch)),
                new PatchClassProcessor(harmony, typeof(LocaleRegisterPatch)),
                new PatchClassProcessor(harmony, typeof(LocaleLoaderGetLocalizedTextPatch))
            });
        }

        private static void TranslateBuildings()
        {
            // fucking hardcode, please just use LOCALE FILES, U STUPID JFDSPOFJK0-239RU[SADOFJ=0WEFJSDF
            var field = AccessTools.Field(typeof(BuildingEntityRegistry), "RegisteredDefinitions");

            var defs = (Dictionary<string, CustomBuildingEntityDefinition>)field.GetValue(null);

            foreach (var pair in defs)
            {
                bool changed = false;

                if (CuCoreTranslationManager.CuCoreTranslations.TryGetValue(pair.Key, out var name))
                {
                    pair.Value.Name = name;
                    changed = true;
                }

                if (CuCoreTranslationManager.CuCoreTranslations.TryGetValue(pair.Key + "dsc", out var desc))
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
    internal static class LocaleLoaderMergeSectionPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(CUCoreTranslator.LocaleLoader, "MergeSection");

        [HarmonyPostfix]
        private static void Postfix(Dictionary<string, string> target)
        {
            foreach (var pair in CuCoreTranslationManager.CuCoreTranslations)
            {
                target[pair.Key] = pair.Value;
            }
        }
    }

    [HarmonyPatch(typeof(LocaleRegistry))]
    internal static class LocaleRegisterPatch
    {
        [HarmonyPatch(nameof(LocaleRegistry.Register), new[] { typeof(LocaleRegistry.LocaleCategory), typeof(string), typeof(string) })]
        [HarmonyPrefix]
        private static void Prefix(string key, ref string text)
        {
            if (CuCoreTranslationManager.CuCoreTranslations.TryGetValue(key, out var translation))
                text = translation;
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
        private static void Postfix(string key, ref string __result)
        {
            if (CuCoreTranslationManager.CuCoreTranslations.TryGetValue(key, out var value))
                __result = value;
        }
    }
}
