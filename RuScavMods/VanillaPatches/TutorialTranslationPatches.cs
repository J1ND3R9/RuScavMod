using HarmonyLib;
using RuScavMods.TranslationClasses.Managers;
using System.Collections.Generic;
using TMPro;

namespace RuScavMods.Patches
{
    [HarmonyPatch(typeof(TutorialHandler))]
    [HarmonyPriority(Priority.Last)]
    internal class TutorialTranslationPatches
    {
        [HarmonyPatch(nameof(TutorialHandler.HandleCourseSelect))]
        [HarmonyPostfix]
        private static void HandleCourseSelectPostfix(List<TextMeshProUGUI> ___courseTexts)
        {
            if (___courseTexts == null || ___courseTexts.Count == 0) return;
            NativeTranslationManager.ApplyTutorialTranslation(___courseTexts);
        }
    }
}
