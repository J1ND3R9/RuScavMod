using BepInEx;
using CUCoreLib.Registries;
using HarmonyLib;
using RuScavMods.ModPatches.QoLUnknown.Camera;
using RuScavMods.ModPatches.QoLUnknown.SaveMenu;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }

        private static void RegisterPatches()
        {
            var harmony = new Harmony("zzz_com.Floydba.ruscavmods.cucoretranslation");

            CUCorePatches.Add(
                new PatchClassProcessor(harmony, typeof(LocaleLoaderPatch))
            );
        }
    }

    [HarmonyPatch]
    internal static class LocaleLoaderPatch
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
}
