using HarmonyLib;
using MedicalBrutalismMod;
using RuScavMods.ModPatches.CUTarkovMedicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RuScavMods.ModPatches.FurReal
{
    internal class FurRealTranslator
    {
        public static readonly Type ChoicePrompt = AccessTools.TypeByName("FurReal.ChoicePrompt");
        private readonly static List<PatchClassProcessor> FRPatches = new List<PatchClassProcessor>();

        public static void Translate()
        {
            RegisterPatches();
            foreach (PatchClassProcessor processor in FRPatches)
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
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.furrealtranslating");
            FRPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(FRMakeButtonPatch)),
                new PatchClassProcessor(harmony, typeof(FRBuildPatch)),
            });
        }
    }

    [HarmonyPatch]
    internal static class FRMakeButtonPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(FurRealTranslator.ChoicePrompt, "MakeButton");
        [HarmonyPrefix]
        static void Prefix(ref string label)
        {
            label = label
                .Replace("Trim fur", "Подстричь")
                .Replace("Cancel", "Отмена")
                .Replace("Amputate", "Ампутации")
                .Replace("Surgery", "Хирургии");
        }
    }

    [HarmonyPatch]
    internal static class FRBuildPatch
    {
        static MethodBase TargetMethod() => AccessTools.Method(FurRealTranslator.ChoicePrompt, "Build");
        [HarmonyPostfix]
        static void Postfix()
        {
            GameObject titleObject = GameObject.Find("Panel/Title");
            if (titleObject == null) return;

            var tmp = titleObject.GetComponent<TextMeshProUGUI>();
            tmp.text = tmp.text.Replace("Use", "Использовать").Replace("on", "на").Replace("?", " для?");
        }
    }
}
