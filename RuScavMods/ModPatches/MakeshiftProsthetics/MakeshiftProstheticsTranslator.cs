using BepInEx;
using HarmonyLib;
using MakeshiftProsthetics;
using RuScavMods.ModPatches.CUTarkovMedicine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RuScavMods.ModPatches.MakeshiftProsthetics
{
    internal static class MakeshiftProstheticsTranslator
    {
        private readonly static List<PatchClassProcessor> MakeshiftProstheticsPatches = new List<PatchClassProcessor>();
        public static Dictionary<string, string> Translations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public static readonly Type CUCoreContentRegistry = AccessTools.TypeByName("MakeshiftProsthetics.CUCoreContentRegistry");

        public static void Translate()
        {
            FillTranslations();
            RegisterPatches();
            foreach (PatchClassProcessor processor in MakeshiftProstheticsPatches)
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
            RefreshTranslations();
        }

        public static void FillTranslations()
        {
            MethodInfo method = AccessTools.Method(typeof(ModText), "LoadFileDictionary", new[] { typeof(string), typeof(Dictionary<string, string>) });
            string path = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "nyaruqwq.makeshiftprosthetics.txt");
            object[] args = new object[] { path, Translations };
            int result = (int)method.Invoke(null, args);
        }

        private static void RefreshTranslations()
        {
            MethodInfo method = AccessTools.Method(CUCoreContentRegistry, "RegisterAllContent", new[] { typeof(string) });
            object[] args = new object[] { "Загрузка русификатора" };
            method.Invoke(null, args);
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.makeshiftprostheticstranslate");
            MakeshiftProstheticsPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(GetOverridePatch)),
                new PatchClassProcessor(harmony, typeof(ReloadOverridePatch)),
            });
        }
    }

    [HarmonyPatch(typeof(ModText), nameof(ModText.Get))]
    class GetOverridePatch
    {
        [HarmonyPostfix]
        static void Postfix(string key, ref string __result)
        {
            if (MakeshiftProstheticsTranslator.Translations.TryGetValue(key ?? "", out var value))
            {
                __result = value;
            }
        }
    }

    [HarmonyPatch(typeof(ModText), nameof(ModText.Reload))]
    class ReloadOverridePatch
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            MakeshiftProstheticsTranslator.FillTranslations();
        }
    }
}
