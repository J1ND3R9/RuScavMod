using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(Locale))]
    [HarmonyPriority(Priority.Last)]
    internal class GetStringTranslationPatches
    {
        [HarmonyPatch(nameof(Locale.GetString))]
        [HarmonyPostfix]
        private static void GetStringPostfix(ref string __result, string str)
        {
            if (string.IsNullOrEmpty(str)) return;
            NativeTranslationManager.ApplyGetStringTranslation(ref __result, str);
        }
    }
}
