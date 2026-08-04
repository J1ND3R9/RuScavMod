using HarmonyLib;
using RshLib;
using RuScavMods.TranslationClasses.Definitions;
using RuScavMods.TranslationClasses.Managers;
using System;
using System.Collections.Generic;

namespace RuScavMods.ModPatches.RshCCL
{
    internal class RshCCLTranslator
    {
        private readonly static List<PatchClassProcessor> RshCCLPatches = new List<PatchClassProcessor>();
        public static void Translate()
        {
            RegisterPatches();
            foreach (PatchClassProcessor processor in RshCCLPatches)
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
            Harmony harmony = new Harmony("zzz_com.Floydba.ruscavmods.rshccltranslate");
            RshCCLPatches.AddRange(new[]
            {
                new PatchClassProcessor(harmony, typeof(RshItemAdapterPatch)),
            });
        }
    }
    [HarmonyPatch(typeof(RshItemAdapter))]
    internal static class RshItemAdapterPatch
    {
        [HarmonyPatch(nameof(RshItemAdapter.RegisterItem))]
        [HarmonyPrefix]
        private static void RegisterItemPrefix(string itemId, RshItem rshItem)
        {
            if (NativeTranslationManager.Items.TryGetValue(itemId, out NativeTranslationFile.ItemTranslation item))
            {
                if (!string.IsNullOrEmpty(item.Name))
                    rshItem.info.fullName = item.Name;
                if (!string.IsNullOrEmpty(item.Description))
                    rshItem.info.description = item.Description;
                NativeTranslationManager.Items.Remove(itemId);
            }
        }
    }
}
