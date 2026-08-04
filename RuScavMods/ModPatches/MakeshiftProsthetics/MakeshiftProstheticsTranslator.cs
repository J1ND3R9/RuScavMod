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

            //FieldInfo sourceField = AccessTools.Field(typeof(ModText), "Source");
            //Dictionary<string, string> source = (Dictionary<string, string>)sourceField.GetValue(null);

            

            //source.Clear();
            //foreach (KeyValuePair<string, string> kvp in Translations)
            //{
            //    source[kvp.Key] = kvp.Value;
            //    Plugin.Log.LogInfo($"[DICT: {Translations[kvp.Key]}] | [SOURCE: {source[kvp.Key]}]");
            //}
        }

        private static void FillTranslations()
        {
            MethodInfo method = AccessTools.Method(typeof(ModText), "LoadFileDictionary", new[] { typeof(string), typeof(Dictionary<string, string>) });
            string path = Path.Combine(Paths.PluginPath, "RuScavMods", "Translations", "nyaruqwq.makeshiftprosthetics.txt");
            object[] args = new object[] { path, Translations };
            int result = (int)method.Invoke(null, args);
        }

        private static void RegisterPatches()
        {
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.makeshiftprostheticstranslate");
            MakeshiftProstheticsPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(GetOverridePatch)),
            });
        }
    }

    [HarmonyPatch(typeof(ModText), nameof(ModText.Get))]
    class GetOverridePatch
    {
        static void Postfix(string key, ref string __result)
        {
            if (MakeshiftProstheticsTranslator.Translations.TryGetValue(key ?? "", out var value))
            {
                __result = value;
            }
        }
    }
}
